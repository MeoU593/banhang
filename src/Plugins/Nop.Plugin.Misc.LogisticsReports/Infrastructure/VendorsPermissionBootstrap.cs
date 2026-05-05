using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Plugin.Misc.LogisticsReports.Infrastructure;

internal static class VendorsPermissionBootstrap
{
    internal static readonly string[] RequiredPermissionSystemNames =
    [
        StandardPermission.Security.ACCESS_ADMIN_PANEL,
        StandardPermission.Catalog.PRODUCTS_VIEW,
        StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE,
        StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT,
        StandardPermission.ContentManagement.BLOG_VIEW,
        StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE,
        StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW,
        StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE,
        "DocumentPortal.ManageDocuments",
        LogisticsReportsPermissionConfigManager.InputOwnUnitReports,
        LogisticsReportsPermissionConfigManager.ViewOwnUnitReports,
        LogisticsReportsPermissionConfigManager.ViewChildUnitReports,
        LogisticsReportsPermissionConfigManager.SubmitOwnUnitReports,
        LogisticsReportsPermissionConfigManager.ReturnChildUnitReports
    ];

    internal static void EnsureVendorRolePermissions(
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<PermissionRecord> permissionRecordRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionMappingRepository)
    {
        var vendorsRole = customerRoleRepository.Table.FirstOrDefault(x => x.SystemName == NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole == null)
            return;

        var permissionRecords = permissionRecordRepository.Table
            .Where(x => RequiredPermissionSystemNames.Contains(x.SystemName))
            .ToList();

        if (!permissionRecords.Any())
            return;

        var existingPermissionIds = permissionMappingRepository.Table
            .Where(x => x.CustomerRoleId == vendorsRole.Id)
            .Select(x => x.PermissionRecordId)
            .ToHashSet();

        var missingPermissionIds = permissionRecords
            .Select(x => x.Id)
            .Where(id => !existingPermissionIds.Contains(id))
            .ToList();

        foreach (var permissionId in missingPermissionIds)
        {
            permissionMappingRepository.InsertAsync(new PermissionRecordCustomerRoleMapping
            {
                CustomerRoleId = vendorsRole.Id,
                PermissionRecordId = permissionId
            }).GetAwaiter().GetResult();
        }
    }
}
