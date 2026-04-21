using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Models.ReportPeriods;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
public class ReportPeriodAdminController : BaseAdminController
{
    private readonly IReportPeriodService _reportPeriodService;
    private readonly IPermissionService _permissionService;

    public ReportPeriodAdminController(IReportPeriodService reportPeriodService, IPermissionService permissionService)
    {
        _reportPeriodService = reportPeriodService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportPeriods))
            return AccessDeniedView();

        var items = await _reportPeriodService.GetAllAsync();
        return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/List.cshtml", items);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportPeriods))
            return AccessDeniedView();

        return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", new ReportPeriodModel { FromDateUtc = DateTime.UtcNow.Date, ToDateUtc = DateTime.UtcNow.Date, DeadlineUtc = DateTime.UtcNow.Date, StatusId = (int)ReportPeriodStatus.Open });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReportPeriodModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportPeriods))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", model);

        if (model.FromDateUtc > model.ToDateUtc)
        {
            ModelState.AddModelError(nameof(model.ToDateUtc), "Đến ngày phải lớn hơn hoặc bằng từ ngày.");
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", model);
        }

        if (model.DeadlineUtc < model.FromDateUtc)
        {
            ModelState.AddModelError(nameof(model.DeadlineUtc), "Hạn nộp phải lớn hơn hoặc bằng từ ngày.");
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", model);
        }

        var entity = new ReportPeriod
        {
            ReportTemplateId = model.ReportTemplateId,
            Code = model.Code,
            Name = model.Name,
            PeriodTypeId = model.PeriodTypeId,
            FromDateUtc = model.FromDateUtc,
            ToDateUtc = model.ToDateUtc,
            DeadlineUtc = model.DeadlineUtc,
            StatusId = model.StatusId,
            IsLocked = model.IsLocked,
            CreatedOnUtc = DateTime.UtcNow
        };
        await _reportPeriodService.InsertAsync(entity);
        await _reportPeriodService.EnsureUnitReportsCreatedAsync(entity.Id);
        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportPeriods))
            return AccessDeniedView();

        var entity = await _reportPeriodService.GetByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", new ReportPeriodModel
        {
            Id = entity.Id,
            ReportTemplateId = entity.ReportTemplateId,
            Code = entity.Code,
            Name = entity.Name,
            PeriodTypeId = entity.PeriodTypeId,
            FromDateUtc = entity.FromDateUtc,
            ToDateUtc = entity.ToDateUtc,
            DeadlineUtc = entity.DeadlineUtc,
            StatusId = entity.StatusId,
            IsLocked = entity.IsLocked
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ReportPeriodModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportPeriods))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", model);

        if (model.FromDateUtc > model.ToDateUtc)
        {
            ModelState.AddModelError(nameof(model.ToDateUtc), "Đến ngày phải lớn hơn hoặc bằng từ ngày.");
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", model);
        }

        if (model.DeadlineUtc < model.FromDateUtc)
        {
            ModelState.AddModelError(nameof(model.DeadlineUtc), "Hạn nộp phải lớn hơn hoặc bằng từ ngày.");
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportPeriodAdmin/CreateOrEdit.cshtml", model);
        }

        var entity = await _reportPeriodService.GetByIdAsync(model.Id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        entity.ReportTemplateId = model.ReportTemplateId;
        entity.Code = model.Code;
        entity.Name = model.Name;
        entity.PeriodTypeId = model.PeriodTypeId;
        entity.FromDateUtc = model.FromDateUtc;
        entity.ToDateUtc = model.ToDateUtc;
        entity.DeadlineUtc = model.DeadlineUtc;
        entity.StatusId = model.StatusId;
        entity.IsLocked = model.IsLocked;
        await _reportPeriodService.UpdateAsync(entity);
        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> GenerateUnitReports(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportPeriods))
            return AccessDeniedView();

        await _reportPeriodService.EnsureUnitReportsCreatedAsync(id);
        return RedirectToAction(nameof(List));
    }
}

