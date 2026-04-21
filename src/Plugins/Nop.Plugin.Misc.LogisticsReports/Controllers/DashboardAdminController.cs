using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Models.Dashboard;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class DashboardAdminController : BasePluginController
{
    private readonly IDashboardService _dashboardService;
    private readonly IPermissionService _permissionService;

    public DashboardAdminController(IDashboardService dashboardService, IPermissionService permissionService)
    {
        _dashboardService = dashboardService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Index()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewSystemWideReports))
            return AccessDeniedView();

        var summary = await _dashboardService.BuildSummaryAsync();
        var model = new DashboardModel
        {
            TotalOrganizations = summary.TotalOrganizations,
            TotalTemplates = summary.TotalTemplates,
            TotalPeriods = summary.TotalPeriods,
            TotalUnitReports = summary.TotalUnitReports,
            SubmittedUnitReports = summary.SubmittedUnitReports,
            ReturnedUnitReports = summary.ReturnedUnitReports,
            LockedUnitReports = summary.LockedUnitReports
        };

        return View("~/Plugins/Misc.LogisticsReports/Views/DashboardAdmin/Index.cshtml", model);
    }
}

