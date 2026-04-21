using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data;

[NopMigration("2026/04/20 12:00:00", "Misc.LogisticsReports migrate organization data to vendors", MigrationProcessType.Update)]
public class MigrateOrganizationDataToVendorMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var organizationRepository = EngineContext.Current.Resolve<IRepository<OrganizationUnit>>();
        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();
        var customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();
        var customerOrganizationRepository = EngineContext.Current.Resolve<IRepository<CustomerOrganizationUnit>>();
        var unitReportRepository = EngineContext.Current.Resolve<IRepository<UnitReport>>();

        var organizations = organizationRepository.Table
            .OrderBy(x => x.Level)
            .ThenBy(x => x.Id)
            .ToList();

        if (!organizations.Any())
            return;

        var vendors = vendorRepository.Table.ToList();
        var organizationToVendorMap = new Dictionary<int, int>();

        foreach (var organization in organizations)
        {
            var vendor = FindVendor(vendors, organization);
            if (vendor == null)
            {
                vendor = CreateVendor(organization);
                vendorRepository.InsertAsync(vendor).GetAwaiter().GetResult();
                vendors.Add(vendor);
            }

            organizationToVendorMap[organization.Id] = vendor.Id;
        }

        var vendorsToUpdate = new List<Vendor>();
        foreach (var organization in organizations)
        {
            if (!organizationToVendorMap.TryGetValue(organization.Id, out var vendorId))
                continue;

            var vendor = vendors.FirstOrDefault(x => x.Id == vendorId);
            if (vendor == null)
                continue;

            int? parentVendorId = null;
            if (organization.ParentId.HasValue && organizationToVendorMap.TryGetValue(organization.ParentId.Value, out var mappedParentId))
                parentVendorId = mappedParentId;

            if (!ApplyOrganizationValues(vendor, organization, parentVendorId))
                continue;

            vendorsToUpdate.Add(vendor);
        }

        if (vendorsToUpdate.Any())
            vendorRepository.UpdateAsync(vendorsToUpdate.DistinctBy(x => x.Id).ToList()).GetAwaiter().GetResult();

        RemapUnitReports(unitReportRepository, organizationToVendorMap);
        RemapCustomerOrganizations(customerOrganizationRepository, organizationToVendorMap);
        SyncCustomers(customerRepository, customerOrganizationRepository, organizationToVendorMap);
    }

    public override void Down()
    {
    }

    private static Vendor? FindVendor(IEnumerable<Vendor> vendors, OrganizationUnit organization)
    {
        if (!string.IsNullOrWhiteSpace(organization.Code))
        {
            var byCode = vendors.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.Code) &&
                x.Code.Equals(organization.Code, StringComparison.OrdinalIgnoreCase));
            if (byCode != null)
                return byCode;
        }

        var byId = vendors.FirstOrDefault(x => x.Id == organization.Id);
        if (byId != null)
            return byId;

        return vendors.FirstOrDefault(x => string.Equals(x.Name, organization.Name, StringComparison.OrdinalIgnoreCase));
    }

    private static Vendor CreateVendor(OrganizationUnit organization)
    {
        return new Vendor
        {
            Name = string.IsNullOrWhiteSpace(organization.Name) ? $"Organization #{organization.Id}" : organization.Name,
            Code = string.IsNullOrWhiteSpace(organization.Code) ? $"ORG-{organization.Id}" : organization.Code,
            ParentId = null,
            Level = organization.Level,
            Path = organization.Path,
            Active = organization.IsActive,
            Deleted = false,
            DisplayOrder = organization.DisplayOrder,
            Email = string.Empty,
            Description = string.Empty,
            AdminComment = string.Empty,
            MetaKeywords = string.Empty,
            MetaDescription = string.Empty,
            MetaTitle = string.Empty,
            PageSize = 6,
            AllowCustomersToSelectPageSize = true,
            PageSizeOptions = "6, 3, 9",
            PriceRangeFiltering = true,
            ManuallyPriceRange = true
        };
    }

    private static bool ApplyOrganizationValues(Vendor vendor, OrganizationUnit organization, int? parentVendorId)
    {
        var changed = false;

        var targetName = string.IsNullOrWhiteSpace(organization.Name) ? vendor.Name : organization.Name;
        if (!string.Equals(vendor.Name, targetName, StringComparison.Ordinal))
        {
            vendor.Name = targetName;
            changed = true;
        }

        var targetCode = string.IsNullOrWhiteSpace(organization.Code) ? vendor.Code : organization.Code;
        if (!string.Equals(vendor.Code, targetCode, StringComparison.Ordinal))
        {
            vendor.Code = targetCode;
            changed = true;
        }

        if (vendor.ParentId != parentVendorId)
        {
            vendor.ParentId = parentVendorId;
            changed = true;
        }

        if (vendor.Level != organization.Level)
        {
            vendor.Level = organization.Level;
            changed = true;
        }

        if (!string.Equals(vendor.Path, organization.Path, StringComparison.Ordinal))
        {
            vendor.Path = organization.Path;
            changed = true;
        }

        if (vendor.DisplayOrder != organization.DisplayOrder)
        {
            vendor.DisplayOrder = organization.DisplayOrder;
            changed = true;
        }

        if (vendor.Active != organization.IsActive)
        {
            vendor.Active = organization.IsActive;
            changed = true;
        }

        if (vendor.Deleted)
        {
            vendor.Deleted = false;
            changed = true;
        }

        return changed;
    }

    private static void RemapUnitReports(IRepository<UnitReport> unitReportRepository, IReadOnlyDictionary<int, int> organizationToVendorMap)
    {
        var reports = unitReportRepository.Table.ToList();
        var changedReports = new List<UnitReport>();

        foreach (var report in reports)
        {
            var changed = false;

            if (organizationToVendorMap.TryGetValue(report.OrganizationUnitId, out var vendorId) && vendorId != report.OrganizationUnitId)
            {
                report.OrganizationUnitId = vendorId;
                changed = true;
            }

            if (report.ParentOrganizationUnitId.HasValue
                && organizationToVendorMap.TryGetValue(report.ParentOrganizationUnitId.Value, out var parentVendorId)
                && parentVendorId != report.ParentOrganizationUnitId.Value)
            {
                report.ParentOrganizationUnitId = parentVendorId;
                changed = true;
            }

            if (changed)
                changedReports.Add(report);
        }

        if (changedReports.Any())
            unitReportRepository.UpdateAsync(changedReports).GetAwaiter().GetResult();
    }

    private static void RemapCustomerOrganizations(IRepository<CustomerOrganizationUnit> customerOrganizationRepository, IReadOnlyDictionary<int, int> organizationToVendorMap)
    {
        var mappings = customerOrganizationRepository.Table
            .OrderBy(x => x.CustomerId)
            .ThenByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Id)
            .ToList();

        var changedMappings = new List<CustomerOrganizationUnit>();
        foreach (var mapping in mappings)
        {
            if (!organizationToVendorMap.TryGetValue(mapping.OrganizationUnitId, out var vendorId) || vendorId == mapping.OrganizationUnitId)
                continue;

            mapping.OrganizationUnitId = vendorId;
            changedMappings.Add(mapping);
        }

        if (changedMappings.Any())
            customerOrganizationRepository.UpdateAsync(changedMappings).GetAwaiter().GetResult();

        var duplicates = mappings
            .GroupBy(x => x.CustomerId)
            .SelectMany(group => group
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.Id)
                .Skip(1))
            .ToList();

        if (duplicates.Any())
            customerOrganizationRepository.DeleteAsync(duplicates).GetAwaiter().GetResult();
    }

    private static void SyncCustomers(
        IRepository<Customer> customerRepository,
        IRepository<CustomerOrganizationUnit> customerOrganizationRepository,
        IReadOnlyDictionary<int, int> organizationToVendorMap)
    {
        var customersToUpdate = new List<Customer>();

        var customersWithVendor = customerRepository.Table.Where(x => x.VendorId > 0).ToList();
        foreach (var customer in customersWithVendor)
        {
            if (!organizationToVendorMap.TryGetValue(customer.VendorId, out var mappedVendorId) || mappedVendorId == customer.VendorId)
                continue;

            customer.VendorId = mappedVendorId;
            customersToUpdate.Add(customer);
        }

        var primaryMappings = customerOrganizationRepository.Table
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Id)
            .ToList()
            .GroupBy(x => x.CustomerId)
            .Select(group => group.First())
            .ToList();

        if (primaryMappings.Any())
        {
            var customerById = customerRepository.Table
                .Where(x => primaryMappings.Select(m => m.CustomerId).Contains(x.Id))
                .ToDictionary(x => x.Id);

            foreach (var mapping in primaryMappings)
            {
                if (!customerById.TryGetValue(mapping.CustomerId, out var customer))
                    continue;

                if (customer.VendorId == mapping.OrganizationUnitId)
                    continue;

                customer.VendorId = mapping.OrganizationUnitId;
                customersToUpdate.Add(customer);
            }
        }

        if (customersToUpdate.Any())
            customerRepository.UpdateAsync(customersToUpdate.DistinctBy(x => x.Id).ToList()).GetAwaiter().GetResult();
    }

}
