using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.LogisticsReports.Infrastructure;

public class AdminMenuConsumer : IConsumer<AdminMenuCreatedEvent>
{
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IWorkContext _workContext;

    public AdminMenuConsumer(
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
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var canViewSystemWide = await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewSystemWideReports, currentCustomer);
        var canManageDocuments = await _permissionService.AuthorizeAsync("DocumentPortal.ManageDocuments", currentCustomer);
        var isUnitLeader = currentVendor != null && currentVendor.PmCustomerId == currentCustomer.Id;

        if (!canViewSystemWide && currentVendor == null)
            return;

        var rootItem = new AdminMenuItem
        {
            SystemName = "LogisticsReports.Root",
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.Root"),
            IconClass = "fas fa-chart-bar",
            Visible = true,
            ChildNodes = new List<AdminMenuItem>()
        };

        if (canViewSystemWide)
        {
            var managementNodes = new List<AdminMenuItem>
            {
                new()
                {
                    SystemName = "LogisticsReports.Units",
                    Title = "Quản lý đơn vị",
                    Url = eventMessage.GetMenuItemUrl("Vendor", "List"),
                    Visible = true,
                    IconClass = "far fa-circle"
                },
                new()
                {
                    SystemName = "LogisticsReports.Templates",
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.Templates"),
                    Url = eventMessage.GetMenuItemUrl("ReportTemplateAdmin", "List"),
                    Visible = true,
                    IconClass = "far fa-circle"
                },
                new()
                {
                    SystemName = "LogisticsReports.Periods",
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.Periods"),
                    Url = eventMessage.GetMenuItemUrl("ReportPeriodAdmin", "List"),
                    Visible = true,
                    IconClass = "far fa-circle"
                },
                new()
                {
                    SystemName = "LogisticsReports.UnitReports",
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.UnitReports"),
                    Url = eventMessage.GetMenuItemUrl("UnitReportAdmin", "Index"),
                    Visible = true,
                    IconClass = "far fa-circle"
                }
            };

            foreach (var node in managementNodes)
                rootItem.ChildNodes.Add(node);
        }
        else
        {
            rootItem.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "LogisticsReports.UnitReports",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.UnitReports"),
                Url = eventMessage.GetMenuItemUrl("UnitReportAdmin", "Index"),
                Visible = true,
                IconClass = "far fa-circle"
            });
        }

        if (!canViewSystemWide)
        {
            rootItem.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "LogisticsReports.Products",
                Title = "Danh mục hàng hóa",
                Url = eventMessage.GetMenuItemUrl("Product", "List"),
                Visible = true,
                IconClass = "far fa-circle"
            });

            rootItem.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "LogisticsReports.News",
                Title = "Tin tức / Công văn",
                Url = eventMessage.GetMenuItemUrl("Blog", "BlogPosts"),
                Visible = true,
                IconClass = "far fa-circle"
            });

            if (canManageDocuments || isUnitLeader)
            {
                rootItem.ChildNodes.Add(new AdminMenuItem
                {
                    SystemName = "LogisticsReports.Documents",
                    Title = "Văn bản tài liệu",
                    Url = eventMessage.GetMenuItemUrl("DocumentPortalAdmin", "List"),
                    Visible = true,
                    IconClass = "far fa-circle"
                });
            }
        }

        if (isUnitLeader)
        {
            rootItem.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = "LogisticsReports.StaffAccess",
                Title = "Cấp quyền nhân viên",
                Url = eventMessage.GetMenuItemUrl("VendorStaffAccess", "Index"),
                Visible = true,
                IconClass = "far fa-circle"
            });
        }

        if (!rootItem.ChildNodes.Any())
            return;

        eventMessage.RootMenuItem.ChildNodes.Add(rootItem);
    }
}
