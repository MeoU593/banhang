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

    public AdminMenuConsumer(ILocalizationService localizationService, IPermissionService permissionService)
    {
        _localizationService = localizationService;
        _permissionService = permissionService;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewSystemWideReports)
            && !await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return;

        var rootItem = new AdminMenuItem
        {
            SystemName = "LogisticsReports.Root",
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.Root"),
            IconClass = "fas fa-chart-bar",
            Visible = true,
            ChildNodes = new List<AdminMenuItem>
            {
                new()
                {
                    SystemName = "LogisticsReports.Dashboard",
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.Dashboard"),
                    Url = eventMessage.GetMenuItemUrl("DashboardAdmin", "Index"),
                    Visible = true,
                    IconClass = "far fa-circle"
                },
                new()
                {
                    SystemName = "LogisticsReports.CustomerOrganizations",
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.CustomerOrganizations"),
                    Url = eventMessage.GetMenuItemUrl("CustomerOrganizationAdmin", "List"),
                    Visible = true,
                    IconClass = "far fa-circle"
                },
                new()
                {
                    SystemName = "LogisticsReports.Organizations",
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.LogisticsReports.Menu.Organizations"),
                    Url = eventMessage.GetMenuItemUrl("OrganizationAdmin", "List"),
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
            }
        };

        eventMessage.RootMenuItem.ChildNodes.Add(rootItem);
    }
}
