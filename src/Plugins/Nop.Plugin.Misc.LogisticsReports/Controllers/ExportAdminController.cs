using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class ExportAdminController : BasePluginController
{
    private readonly IExportService _exportService;
    private readonly IPermissionService _permissionService;

    public ExportAdminController(IExportService exportService, IPermissionService permissionService)
    {
        _exportService = exportService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> UnitReportCsv(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewSystemWideReports))
            return AccessDeniedView();

        var bytes = await _exportService.ExportUnitReportCsvAsync(id);
        return File(bytes, "text/csv", $"unit-report-{id}.csv");
    }
}
