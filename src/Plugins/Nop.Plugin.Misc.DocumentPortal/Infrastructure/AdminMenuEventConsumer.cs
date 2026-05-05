using Nop.Core;
using Nop.Plugin.Misc.DocumentPortal.Services;
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
    private readonly IDocumentPortalService _documentPortalService;
    private readonly IWorkContext _workContext;

    public AdminMenuEventConsumer(
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IDocumentPortalService documentPortalService,
        IWorkContext workContext)
    {
        _localizationService = localizationService;
        _permissionService = permissionService;
        _documentPortalService = documentPortalService;
        _workContext = workContext;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var canAccessDocuments = await _documentPortalService.CanAccessDocumentManagementAsync(currentCustomer);
        var canManage = await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENTS);
        var canManageSettings = await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENT_SETTINGS);

        if (!canAccessDocuments && !canManageSettings)
            return;

        var root = new AdminMenuItem
        {
            SystemName = "DocumentPortal.Root",
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Menu.Root"),
            IconClass = "far fa-folder-open",
            Visible = true
        };

        if (canAccessDocuments)
        {
            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "DocumentPortal.Documents",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Menu.Documents"),
                Url = "~/Admin/DocumentPortalAdmin/List",
                IconClass = "far fa-file-alt"
            });
        }

        if (canManage)
        {
            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "DocumentPortal.Categories",
                Title = "Danh mục tài liệu",
                Url = "~/Admin/DocumentCategoryAdmin/List",
                IconClass = "fas fa-folder"
            });

            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "DocumentPortal.Types",
                Title = "Loại tài liệu",
                Url = "~/Admin/DocumentTypeAdmin/List",
                IconClass = "fas fa-layer-group"
            });

            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "DocumentPortal.Issuers",
                Title = "Cơ quan ban hành",
                Url = "~/Admin/DocumentIssuerAdmin/List",
                IconClass = "fas fa-building"
            });

            if (canManageSettings)
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
        else if (canManageSettings)
        {
            root.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "DocumentPortal.Settings",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Menu.Settings"),
                Url = "~/Admin/DocumentPortalAdmin/Settings",
                IconClass = "fas fa-cog"
            });
        }

        eventMessage.RootMenuItem.ChildNodes.Add(root);
    }
}
