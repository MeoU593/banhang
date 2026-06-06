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
        if (!canViewSystemWide && currentVendor == null)
            return;

        var rootItem = eventMessage.RootMenuItem.GetItemBySystemName("LogisticsReports.Root") ?? new AdminMenuItem
        {
            SystemName = "LogisticsReports.Root",
            Title = "Quản lý báo cáo",
            IconClass = "fas fa-chart-bar",
            Visible = true,
            ChildNodes = new List<AdminMenuItem>()
        };

        rootItem.Title = "Quản lý báo cáo";
        rootItem.IconClass = "fas fa-chart-bar";
        RemoveMenuItem(rootItem.ChildNodes, "VendorStaffAccess");

        if (canViewSystemWide)
        {
            var managementNodes = new List<AdminMenuItem>
            {
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

        if (!rootItem.ChildNodes.Any())
            return;

        if (!eventMessage.RootMenuItem.ChildNodes.Contains(rootItem))
        {
            var insertIndex = -1;
            for (var i = 0; i < eventMessage.RootMenuItem.ChildNodes.Count; i++)
            {
                if (eventMessage.RootMenuItem.ChildNodes[i].SystemName == "DocumentPortal.Root")
                {
                    insertIndex = i;
                    break;
                }
            }

            if (insertIndex < 0)
            {
                for (var i = 0; i < eventMessage.RootMenuItem.ChildNodes.Count; i++)
                {
                    if (eventMessage.RootMenuItem.ChildNodes[i].SystemName == "ProductManagement")
                    {
                        insertIndex = i;
                        break;
                    }
                }
            }

            eventMessage.RootMenuItem.ChildNodes.Insert(insertIndex >= 0 ? insertIndex + 1 : eventMessage.RootMenuItem.ChildNodes.Count, rootItem);
        }
    }

    private static void RemoveMenuItem(IList<AdminMenuItem> items, string systemName)
    {
        for (var i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].SystemName == systemName)
                items.RemoveAt(i);
            else if (items[i].ChildNodes?.Any() == true)
                RemoveMenuItem(items[i].ChildNodes, systemName);
        }
    }
}
