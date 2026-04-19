using Nop.Core;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.DocumentPortal.Infrastructure;

public class AdminMenuEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IWorkContext _workContext;

    public AdminMenuEventConsumer(
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IWorkContext workContext)
    {
        _localizationService = localizationService;
        _permissionService = permissionService;
        _workContext = workContext;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var canManage = await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENTS);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var isVendorEmployee = customer.VendorId > 0;

        if (!canManage && !isVendorEmployee)
            return;

        var root = new AdminMenuItem
        {
            SystemName = "DocumentPortal.Root",
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Menu.Root"),
            IconClass = "far fa-folder-open",
            Visible = true
        };

        if (canManage)
        {
            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "DocumentPortal.Documents",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Menu.Documents"),
                Url = "~/Admin/DocumentPortalAdmin/List",
                IconClass = "far fa-file-alt"
            });

            if (await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENT_SETTINGS))
            {
                root.ChildNodes.Add(new AdminMenuItem
                {
                    SystemName = "DocumentPortal.Settings",
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Menu.Settings"),
                    Url = "~/Admin/DocumentPortalAdmin/Settings",
                    IconClass = "fas fa-cog"
                });
            }
        }

        if (isVendorEmployee)
        {
            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "DocumentPortal.VendorDocuments",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Menu.VendorDocuments"),
                Url = "~/Admin/DocumentPortalVendor/List",
                IconClass = "fas fa-building"
            });
        }

        eventMessage.RootMenuItem.ChildNodes.Add(root);
    }
}
