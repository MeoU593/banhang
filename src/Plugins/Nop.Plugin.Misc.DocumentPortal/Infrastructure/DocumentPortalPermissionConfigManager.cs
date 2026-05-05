using Nop.Core.Domain.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.DocumentPortal.Infrastructure;

public class DocumentPortalPermissionConfigManager : IPermissionConfigManager
{
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        new("Admin area. Document Portal. Manage documents", DocumentPortalDefaults.Permissions.MANAGE_DOCUMENTS,
            nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new("Admin area. Document Portal. Manage settings", DocumentPortalDefaults.Permissions.MANAGE_DOCUMENT_SETTINGS,
            nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
    };
}
