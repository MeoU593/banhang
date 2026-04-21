using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data;

[NopMigration("2026/04/20 14:00:00", "Misc.LogisticsReports ensure root organization hierarchy", MigrationProcessType.Update)]
public class EnsureRootOrganizationHierarchyMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();

        var vendors = vendorRepository.Table
            .Where(x => !x.Deleted)
            .ToList();

        var rootVendor = vendors.FirstOrDefault(x => x.Code == OrganizationDefaults.RootCode || x.Name == OrganizationDefaults.RootName);
        if (rootVendor == null)
        {
            rootVendor = CreateRootVendor();
            vendorRepository.InsertAsync(rootVendor).GetAwaiter().GetResult();
            vendors.Add(rootVendor);
        }

        var changedVendors = new List<Vendor>();

        if (ApplyRootDefaults(rootVendor))
            changedVendors.Add(rootVendor);

        var vendorById = vendors.ToDictionary(x => x.Id);
        foreach (var vendor in vendors.Where(x => x.Id != rootVendor.Id))
        {
            var changed = false;

            if (string.IsNullOrWhiteSpace(vendor.Code))
            {
                vendor.Code = $"ORG-{vendor.Id}";
                changed = true;
            }

            var normalizedParentId = NormalizeParentId(vendor, rootVendor.Id, vendorById);
            if (vendor.ParentId != normalizedParentId)
            {
                vendor.ParentId = normalizedParentId;
                changed = true;
            }

            if (changed)
                changedVendors.Add(vendor);
        }

        var hierarchyCache = new Dictionary<int, (int Level, string Path)>();
        foreach (var vendor in vendors)
        {
            var (level, path) = BuildHierarchy(vendor, rootVendor, vendorById, hierarchyCache);
            var changed = false;

            if (vendor.Level != level)
            {
                vendor.Level = level;
                changed = true;
            }

            if (!string.Equals(vendor.Path, path, StringComparison.Ordinal))
            {
                vendor.Path = path;
                changed = true;
            }

            if (changed)
                changedVendors.Add(vendor);
        }

        if (changedVendors.Any())
            vendorRepository.UpdateAsync(changedVendors.DistinctBy(x => x.Id).ToList()).GetAwaiter().GetResult();
    }

    public override void Down()
    {
    }

    private static Vendor CreateRootVendor()
    {
        return new Vendor
        {
            Name = OrganizationDefaults.RootName,
            Code = OrganizationDefaults.RootCode,
            ParentId = null,
            Level = 0,
            Path = OrganizationDefaults.RootPath,
            DisplayOrder = 0,
            Active = true,
            Deleted = false,
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

    private static bool ApplyRootDefaults(Vendor rootVendor)
    {
        var changed = false;

        if (!string.Equals(rootVendor.Name, OrganizationDefaults.RootName, StringComparison.Ordinal))
        {
            rootVendor.Name = OrganizationDefaults.RootName;
            changed = true;
        }

        if (!string.Equals(rootVendor.Code, OrganizationDefaults.RootCode, StringComparison.Ordinal))
        {
            rootVendor.Code = OrganizationDefaults.RootCode;
            changed = true;
        }

        if (rootVendor.ParentId.HasValue)
        {
            rootVendor.ParentId = null;
            changed = true;
        }

        if (rootVendor.Level != 0)
        {
            rootVendor.Level = 0;
            changed = true;
        }

        if (!string.Equals(rootVendor.Path, OrganizationDefaults.RootPath, StringComparison.Ordinal))
        {
            rootVendor.Path = OrganizationDefaults.RootPath;
            changed = true;
        }

        if (rootVendor.Deleted)
        {
            rootVendor.Deleted = false;
            changed = true;
        }

        if (!rootVendor.Active)
        {
            rootVendor.Active = true;
            changed = true;
        }

        return changed;
    }

    private static int NormalizeParentId(Vendor vendor, int rootVendorId, IReadOnlyDictionary<int, Vendor> vendorById)
    {
        if (!vendor.ParentId.HasValue)
            return rootVendorId;

        var parentId = vendor.ParentId.Value;
        if (parentId == vendor.Id)
            return rootVendorId;

        if (!vendorById.ContainsKey(parentId))
            return rootVendorId;

        if (IsInAncestorChain(vendor.Id, parentId, vendorById))
            return rootVendorId;

        return parentId;
    }

    private static bool IsInAncestorChain(int vendorId, int parentId, IReadOnlyDictionary<int, Vendor> vendorById)
    {
        var currentParentId = parentId;
        var visited = new HashSet<int>();

        while (true)
        {
            if (currentParentId == vendorId)
                return true;

            if (!visited.Add(currentParentId))
                return true;

            if (!vendorById.TryGetValue(currentParentId, out var parentVendor) || !parentVendor.ParentId.HasValue)
                return false;

            currentParentId = parentVendor.ParentId.Value;
        }
    }

    private static (int Level, string Path) BuildHierarchy(
        Vendor vendor,
        Vendor rootVendor,
        IReadOnlyDictionary<int, Vendor> vendorById,
        IDictionary<int, (int Level, string Path)> hierarchyCache)
    {
        if (hierarchyCache.TryGetValue(vendor.Id, out var cached))
            return cached;

        (int Level, string Path) result;
        if (vendor.Id == rootVendor.Id)
        {
            result = (0, OrganizationDefaults.RootPath);
        }
        else if (!vendor.ParentId.HasValue || !vendorById.TryGetValue(vendor.ParentId.Value, out var parentVendor))
        {
            result = (1, $"{OrganizationDefaults.RootPath}/{vendor.Code}");
        }
        else
        {
            var parentHierarchy = BuildHierarchy(parentVendor, rootVendor, vendorById, hierarchyCache);
            result = (parentHierarchy.Level + 1, $"{parentHierarchy.Path}/{vendor.Code}");
        }

        hierarchyCache[vendor.Id] = result;
        return result;
    }
}
