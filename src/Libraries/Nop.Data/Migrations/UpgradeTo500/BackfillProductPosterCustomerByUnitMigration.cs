using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-07 09:30:00", "5.00", UpdateMigrationType.Data)]
public class BackfillProductPosterCustomerByUnitMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var productRepository = EngineContext.Current.Resolve<IRepository<Product>>();
        var customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();
        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();
        var customerRoleRepository = EngineContext.Current.Resolve<IRepository<CustomerRole>>();
        var customerRoleMappingRepository = EngineContext.Current.Resolve<IRepository<CustomerCustomerRoleMapping>>();
        var militaryProfileRepository = EngineContext.Current.Resolve<IRepository<CustomerMilitaryProfile>>();

        var products = productRepository.Table
            .Where(product => !product.Deleted)
            .ToList();

        if (!products.Any())
            return;

        var vendorsById = vendorRepository.Table
            .Where(vendor => !vendor.Deleted && vendor.Active)
            .ToDictionary(vendor => vendor.Id);

        var activeCustomers = customerRepository.Table
            .Where(customer => !customer.Deleted && customer.Active)
            .ToList();

        var activeCustomersById = activeCustomers.ToDictionary(customer => customer.Id);
        var activeCustomersByVendorId = activeCustomers
            .Where(customer => customer.VendorId > 0)
            .GroupBy(customer => customer.VendorId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var registeredRoleId = customerRoleRepository.Table
            .Where(role => role.SystemName == NopCustomerDefaults.RegisteredRoleName)
            .Select(role => role.Id)
            .FirstOrDefault();
        var vendorsRoleId = customerRoleRepository.Table
            .Where(role => role.SystemName == NopCustomerDefaults.VendorsRoleName)
            .Select(role => role.Id)
            .FirstOrDefault();

        var demoCustomersByVendorId = new Dictionary<int, Customer>();

        foreach (var product in products)
        {
            if (!ShouldAssignPoster(product, vendorsById, activeCustomersById))
                continue;

            var posterCustomerId = ResolveExistingPosterCustomerId(product, vendorsById, activeCustomersById, activeCustomersByVendorId);

            if (posterCustomerId <= 0 && product.VendorId > 0 && vendorsById.TryGetValue(product.VendorId, out var vendor))
            {
                var demoCustomer = EnsureDemoPosterCustomer(
                    vendor,
                    customerRepository,
                    customerRoleMappingRepository,
                    militaryProfileRepository,
                    registeredRoleId,
                    vendorsRoleId,
                    demoCustomersByVendorId);

                posterCustomerId = demoCustomer.Id;
                activeCustomersById[demoCustomer.Id] = demoCustomer;
                activeCustomersByVendorId[demoCustomer.VendorId] = [demoCustomer];
            }

            if (posterCustomerId <= 0)
                posterCustomerId = activeCustomers.FirstOrDefault()?.Id ?? 0;

            if (posterCustomerId <= 0 || product.CreatedByCustomerId == posterCustomerId)
                continue;

            product.CreatedByCustomerId = posterCustomerId;
            productRepository.UpdateAsync(product).GetAwaiter().GetResult();
        }
    }

    public override void Down()
    {
    }

    private static bool ShouldAssignPoster(Product product, IReadOnlyDictionary<int, Vendor> vendorsById, IReadOnlyDictionary<int, Customer> activeCustomersById)
    {
        if (product.CreatedByCustomerId <= 0)
            return true;

        if (!activeCustomersById.TryGetValue(product.CreatedByCustomerId, out var currentPoster))
            return true;

        if (product.VendorId <= 0 || !vendorsById.TryGetValue(product.VendorId, out var vendor))
            return false;

        return currentPoster.VendorId != product.VendorId &&
            vendor.PmCustomerId != product.CreatedByCustomerId &&
            vendor.ContactCustomerId != product.CreatedByCustomerId;
    }

    private static int ResolveExistingPosterCustomerId(
        Product product,
        IReadOnlyDictionary<int, Vendor> vendorsById,
        IReadOnlyDictionary<int, Customer> activeCustomersById,
        IReadOnlyDictionary<int, List<Customer>> activeCustomersByVendorId)
    {
        if (product.VendorId > 0 && vendorsById.TryGetValue(product.VendorId, out var vendor))
        {
            if (vendor.PmCustomerId.HasValue && activeCustomersById.ContainsKey(vendor.PmCustomerId.Value))
                return vendor.PmCustomerId.Value;

            if (vendor.ContactCustomerId.HasValue && activeCustomersById.ContainsKey(vendor.ContactCustomerId.Value))
                return vendor.ContactCustomerId.Value;

            if (activeCustomersByVendorId.TryGetValue(product.VendorId, out var unitCustomers))
                return unitCustomers.FirstOrDefault()?.Id ?? 0;
        }

        return 0;
    }

    private static Customer EnsureDemoPosterCustomer(
        Vendor vendor,
        IRepository<Customer> customerRepository,
        IRepository<CustomerCustomerRoleMapping> customerRoleMappingRepository,
        IRepository<CustomerMilitaryProfile> militaryProfileRepository,
        int registeredRoleId,
        int vendorsRoleId,
        IDictionary<int, Customer> demoCustomersByVendorId)
    {
        if (demoCustomersByVendorId.TryGetValue(vendor.Id, out var cachedCustomer))
            return cachedCustomer;

        var email = $"poster.vendor.{vendor.Id}@ttg.local";
        var username = $"poster_vendor_{vendor.Id}";
        var now = DateTime.UtcNow;

        var customer = customerRepository.Table.FirstOrDefault(existing => existing.Email == email || existing.Username == username);
        if (customer == null)
        {
            customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Username = username,
                Email = email,
                FirstName = "Cán bộ",
                LastName = vendor.Name,
                Gender = string.Empty,
                Company = vendor.Name,
                StreetAddress = string.Empty,
                StreetAddress2 = string.Empty,
                ZipPostalCode = string.Empty,
                City = string.Empty,
                County = string.Empty,
                Phone = string.Empty,
                Fax = string.Empty,
                VatNumber = string.Empty,
                TimeZoneId = string.Empty,
                CustomCustomerAttributesXML = string.Empty,
                EmailToRevalidate = string.Empty,
                AdminComment = "Tài khoản demo tự động tạo để hiển thị người đăng sản phẩm theo đơn vị.",
                VendorId = vendor.Id,
                Active = true,
                Deleted = false,
                LastIpAddress = string.Empty,
                CreatedOnUtc = now,
                LastActivityDateUtc = now,
                RegisteredInStoreId = 0,
                SystemName = string.Empty
            };

            customerRepository.InsertAsync(customer).GetAwaiter().GetResult();
        }
        else
        {
            customer.VendorId = vendor.Id;
            customer.Active = true;
            customer.Deleted = false;
            if (string.IsNullOrWhiteSpace(customer.FirstName))
                customer.FirstName = "Cán bộ";
            if (string.IsNullOrWhiteSpace(customer.LastName))
                customer.LastName = vendor.Name;
            if (string.IsNullOrWhiteSpace(customer.Company))
                customer.Company = vendor.Name;
            customer.LastActivityDateUtc = now;

            customerRepository.UpdateAsync(customer).GetAwaiter().GetResult();
        }

        EnsureRoleMapping(customer.Id, registeredRoleId, customerRoleMappingRepository);
        EnsureRoleMapping(customer.Id, vendorsRoleId, customerRoleMappingRepository);
        EnsureMilitaryProfile(customer, vendor, militaryProfileRepository, now);

        demoCustomersByVendorId[vendor.Id] = customer;
        return customer;
    }

    private static void EnsureRoleMapping(int customerId, int roleId, IRepository<CustomerCustomerRoleMapping> customerRoleMappingRepository)
    {
        if (roleId <= 0)
            return;

        var exists = customerRoleMappingRepository.Table.Any(mapping => mapping.CustomerId == customerId && mapping.CustomerRoleId == roleId);
        if (exists)
            return;

        customerRoleMappingRepository.InsertAsync(new CustomerCustomerRoleMapping
        {
            CustomerId = customerId,
            CustomerRoleId = roleId
        }).GetAwaiter().GetResult();
    }

    private static void EnsureMilitaryProfile(Customer customer, Vendor vendor, IRepository<CustomerMilitaryProfile> militaryProfileRepository, DateTime now)
    {
        var profile = militaryProfileRepository.Table.FirstOrDefault(existing => existing.CustomerId == customer.Id);
        if (profile == null)
        {
            profile = new CustomerMilitaryProfile
            {
                CustomerId = customer.Id,
                MilitaryCode = vendor.Code ?? string.Empty,
                Rank = "Đồng chí",
                UnitName = vendor.Name,
                PositionTitle = "Phụ trách đăng sản phẩm",
                CreatedOnUtc = now,
                UpdatedOnUtc = now
            };

            militaryProfileRepository.InsertAsync(profile).GetAwaiter().GetResult();
            return;
        }

        if (string.IsNullOrWhiteSpace(profile.MilitaryCode))
            profile.MilitaryCode = vendor.Code ?? string.Empty;
        if (string.IsNullOrWhiteSpace(profile.Rank))
            profile.Rank = "Đồng chí";
        if (string.IsNullOrWhiteSpace(profile.UnitName))
            profile.UnitName = vendor.Name;
        if (string.IsNullOrWhiteSpace(profile.PositionTitle))
            profile.PositionTitle = "Phụ trách đăng sản phẩm";
        profile.UpdatedOnUtc = now;

        militaryProfileRepository.UpdateAsync(profile).GetAwaiter().GetResult();
    }
}
