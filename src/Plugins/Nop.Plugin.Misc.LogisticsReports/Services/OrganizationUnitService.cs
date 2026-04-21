using Nop.Data;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class OrganizationUnitService : IOrganizationUnitService
{
    private readonly IRepository<Vendor> _vendorRepository;

    public OrganizationUnitService(IRepository<Vendor> vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<OrganizationUnit> EnsureRootUnitAsync()
        => ToOrganizationUnit(await EnsureRootVendorAsync());

    public async Task<IList<OrganizationUnit>> GetAllAsync(bool onlyActive = false)
    {
        await EnsureRootVendorAsync();

        var query = _vendorRepository.Table.Where(v => !v.Deleted);
        if (onlyActive)
            query = query.Where(x => x.Active);

        var vendors = await query.OrderBy(x => x.Path).ThenBy(x => x.DisplayOrder).ToListAsync();
        return vendors.Select(ToOrganizationUnit).ToList();
    }

    public async Task<OrganizationUnit?> GetByIdAsync(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null || vendor.Deleted)
            return null;

        return ToOrganizationUnit(vendor);
    }

    public async Task InsertAsync(OrganizationUnit entity)
    {
        var rootVendor = await EnsureRootVendorAsync();
        var normalizedParentId = await NormalizeParentIdAsync(entity.ParentId, 0, rootVendor.Id);

        var vendor = new Vendor
        {
            Name = entity.Name,
            Code = entity.Code,
            ParentId = normalizedParentId,
            DisplayOrder = entity.DisplayOrder,
            Active = entity.IsActive,
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

        var organizationModel = ToOrganizationUnit(vendor);
        vendor.Path = await BuildPathAsync(organizationModel);
        vendor.Level = organizationModel.Level;

        await _vendorRepository.InsertAsync(vendor);

        organizationModel = ToOrganizationUnit(vendor);
        vendor.Path = await BuildPathAsync(organizationModel);
        vendor.Level = organizationModel.Level;
        await _vendorRepository.UpdateAsync(vendor);
    }

    public async Task UpdateAsync(OrganizationUnit entity)
    {
        var rootVendor = await EnsureRootVendorAsync();
        var vendor = await _vendorRepository.GetByIdAsync(entity.Id);
        if (vendor == null || vendor.Deleted)
            return;

        var normalizedParentId = vendor.Id == rootVendor.Id
            ? null
            : await NormalizeParentIdAsync(entity.ParentId, entity.Id, rootVendor.Id);

        vendor.Name = entity.Name;
        vendor.Code = entity.Code;
        vendor.ParentId = normalizedParentId;
        vendor.DisplayOrder = entity.DisplayOrder;
        vendor.Active = entity.IsActive;

        var organizationModel = ToOrganizationUnit(vendor);
        organizationModel.Code = entity.Code;
        organizationModel.ParentId = normalizedParentId;

        vendor.Path = await BuildPathAsync(organizationModel);
        vendor.Level = organizationModel.Level;
        await _vendorRepository.UpdateAsync(vendor);
    }

    public async Task DeleteAsync(OrganizationUnit entity)
    {
        var vendor = await _vendorRepository.GetByIdAsync(entity.Id);
        if (vendor == null)
            return;

        vendor.Deleted = true;
        await _vendorRepository.UpdateAsync(vendor);
    }

    public async Task<IList<OrganizationUnit>> GetChildrenAsync(int? parentId)
    {
        var children = await _vendorRepository.Table
            .Where(x => !x.Deleted && x.ParentId == parentId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();

        return children.Select(ToOrganizationUnit).ToList();
    }

    public async Task<IList<OrganizationUnit>> GetDescendantsAsync(int organizationUnitId)
    {
        var root = await GetByIdAsync(organizationUnitId);
        if (root is null)
            return new List<OrganizationUnit>();

        var descendants = await _vendorRepository.Table
            .Where(x => !x.Deleted && (x.Path.StartsWith(root.Path + "/") || x.Id == organizationUnitId))
            .OrderBy(x => x.Path)
            .ToListAsync();

        return descendants.Select(ToOrganizationUnit).ToList();
    }

    public async Task<string> BuildPathAsync(OrganizationUnit entity)
    {
        var code = string.IsNullOrWhiteSpace(entity.Code) ? $"ORG-{entity.Id}" : entity.Code;

        if (!entity.ParentId.HasValue)
        {
            entity.Level = 0;
            if (string.Equals(code, OrganizationDefaults.RootCode, StringComparison.OrdinalIgnoreCase))
                return OrganizationDefaults.RootPath;

            entity.Level = 1;
            return $"{OrganizationDefaults.RootPath}/{code}";
        }

        var parent = await GetByIdAsync(entity.ParentId.Value);
        if (parent == null)
        {
            entity.Level = 1;
            return $"{OrganizationDefaults.RootPath}/{code}";
        }

        entity.Level = parent.Level + 1;
        return $"{parent.Path}/{code}";
    }

    private async Task<Vendor> EnsureRootVendorAsync()
    {
        var rootVendor = await _vendorRepository.Table
            .FirstOrDefaultAsync(x => !x.Deleted
                && (x.Code == OrganizationDefaults.RootCode || x.Name == OrganizationDefaults.RootName));

        if (rootVendor == null)
        {
            rootVendor = new Vendor
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

            await _vendorRepository.InsertAsync(rootVendor);
            return rootVendor;
        }

        var changed = false;
        if (rootVendor.ParentId.HasValue)
        {
            rootVendor.ParentId = null;
            changed = true;
        }

        if (!string.Equals(rootVendor.Code, OrganizationDefaults.RootCode, StringComparison.Ordinal))
        {
            rootVendor.Code = OrganizationDefaults.RootCode;
            changed = true;
        }

        if (!string.Equals(rootVendor.Name, OrganizationDefaults.RootName, StringComparison.Ordinal))
        {
            rootVendor.Name = OrganizationDefaults.RootName;
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

        if (changed)
            await _vendorRepository.UpdateAsync(rootVendor);

        return rootVendor;
    }

    private async Task<int?> NormalizeParentIdAsync(int? parentId, int currentUnitId, int rootVendorId)
    {
        if (!parentId.HasValue || parentId.Value == 0)
            return rootVendorId;

        if (parentId.Value == currentUnitId)
            return rootVendorId;

        var parent = await _vendorRepository.GetByIdAsync(parentId.Value);
        if (parent == null || parent.Deleted)
            return rootVendorId;

        return parent.Id;
    }

    private static OrganizationUnit ToOrganizationUnit(Vendor vendor)
    {
        return new OrganizationUnit
        {
            Id = vendor.Id,
            Code = vendor.Code ?? string.Empty,
            Name = vendor.Name,
            ParentId = vendor.ParentId,
            Level = vendor.Level,
            Path = vendor.Path ?? string.Empty,
            DisplayOrder = vendor.DisplayOrder,
            IsActive = vendor.Active,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
    }
}
