using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Models.ReportTemplates;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
public class ReportTemplateAdminController : BaseAdminController
{
    private readonly IReportTemplateService _reportTemplateService;
    private readonly IStorageService _storageService;
    private readonly IPermissionService _permissionService;

    public ReportTemplateAdminController(IReportTemplateService reportTemplateService, IStorageService storageService, IPermissionService permissionService)
    {
        _reportTemplateService = reportTemplateService;
        _storageService = storageService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        var items = await _reportTemplateService.GetAllAsync();
        return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/List.cshtml", items);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEdit.cshtml", new ReportTemplateModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReportTemplateModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEdit.cshtml", model);

        var filePath = string.Empty;
        if (model.TemplateFile != null && model.TemplateFile.Length > 0)
        {
            await using var stream = model.TemplateFile.OpenReadStream();
            filePath = await _storageService.SaveTemplateAsync(model.TemplateFile.FileName, stream);
        }

        await _reportTemplateService.InsertAsync(new ReportTemplate
        {
            Code = model.Code,
            Name = model.Name,
            Description = model.Description,
            PeriodTypeId = model.PeriodTypeId,
            TemplateStoredFileName = filePath,
            Version = model.Version,
            IsActive = model.IsActive,
            EffectiveFromUtc = model.EffectiveFromUtc,
            EffectiveToUtc = model.EffectiveToUtc
        });

        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        var entity = await _reportTemplateService.GetByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEdit.cshtml", new ReportTemplateModel
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            PeriodTypeId = entity.PeriodTypeId,
            Version = entity.Version,
            IsActive = entity.IsActive,
            EffectiveFromUtc = entity.EffectiveFromUtc,
            EffectiveToUtc = entity.EffectiveToUtc,
            ExistingTemplateFileName = entity.TemplateStoredFileName
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ReportTemplateModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEdit.cshtml", model);

        var entity = await _reportTemplateService.GetByIdAsync(model.Id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        var previousTemplatePath = entity.TemplateStoredFileName;
        var newTemplatePath = string.Empty;
        if (model.TemplateFile != null && model.TemplateFile.Length > 0)
        {
            await using var stream = model.TemplateFile.OpenReadStream();
            newTemplatePath = await _storageService.SaveTemplateAsync(model.TemplateFile.FileName, stream);
            entity.TemplateStoredFileName = newTemplatePath;
        }

        entity.Code = model.Code;
        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.PeriodTypeId = model.PeriodTypeId;
        entity.Version = model.Version;
        entity.IsActive = model.IsActive;
        entity.EffectiveFromUtc = model.EffectiveFromUtc;
        entity.EffectiveToUtc = model.EffectiveToUtc;
        await _reportTemplateService.UpdateAsync(entity);

        if (!string.IsNullOrWhiteSpace(newTemplatePath) && !string.Equals(previousTemplatePath, newTemplatePath, StringComparison.OrdinalIgnoreCase))
            await _storageService.DeleteAsync(previousTemplatePath);

        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Indicators(int reportTemplateId)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        ViewBag.ReportTemplateId = reportTemplateId;
        var items = await _reportTemplateService.GetIndicatorsAsync(reportTemplateId);
        return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/Indicators.cshtml", items);
    }

    public async Task<IActionResult> CreateIndicator(int reportTemplateId)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEditIndicator.cshtml", new ReportIndicatorModel { ReportTemplateId = reportTemplateId, IsActive = true, DataTypeId = (int)IndicatorDataType.Decimal, InputModeId = (int)IndicatorInputMode.Imported, AggregateMethodId = (int)AggregateMethod.Sum });
    }

    [HttpPost]
    public async Task<IActionResult> CreateIndicator(ReportIndicatorModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEditIndicator.cshtml", model);

        await _reportTemplateService.InsertIndicatorAsync(new ReportIndicator
        {
            ReportTemplateId = model.ReportTemplateId,
            Code = model.Code,
            Name = model.Name,
            ParentIndicatorId = model.ParentIndicatorId,
            DisplayOrder = model.DisplayOrder,
            UnitOfMeasure = model.UnitOfMeasure,
            DataTypeId = model.DataTypeId,
            InputModeId = model.InputModeId,
            AggregateMethodId = model.AggregateMethodId,
            FormulaExpression = model.FormulaExpression,
            ExcelSheetName = model.ExcelSheetName,
            ExcelCellAddress = model.ExcelCellAddress,
            IsRequired = model.IsRequired,
            IsActive = model.IsActive
        });

        return RedirectToAction(nameof(Indicators), new { reportTemplateId = model.ReportTemplateId });
    }

    public async Task<IActionResult> EditIndicator(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        var entity = await _reportTemplateService.GetIndicatorByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEditIndicator.cshtml", new ReportIndicatorModel
        {
            Id = entity.Id,
            ReportTemplateId = entity.ReportTemplateId,
            Code = entity.Code,
            Name = entity.Name,
            ParentIndicatorId = entity.ParentIndicatorId,
            DisplayOrder = entity.DisplayOrder,
            UnitOfMeasure = entity.UnitOfMeasure,
            DataTypeId = entity.DataTypeId,
            InputModeId = entity.InputModeId,
            AggregateMethodId = entity.AggregateMethodId,
            FormulaExpression = entity.FormulaExpression,
            ExcelSheetName = entity.ExcelSheetName,
            ExcelCellAddress = entity.ExcelCellAddress,
            IsRequired = entity.IsRequired,
            IsActive = entity.IsActive
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditIndicator(ReportIndicatorModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageReportTemplates))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/ReportTemplateAdmin/CreateOrEditIndicator.cshtml", model);

        var entity = await _reportTemplateService.GetIndicatorByIdAsync(model.Id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        entity.Code = model.Code;
        entity.Name = model.Name;
        entity.ParentIndicatorId = model.ParentIndicatorId;
        entity.DisplayOrder = model.DisplayOrder;
        entity.UnitOfMeasure = model.UnitOfMeasure;
        entity.DataTypeId = model.DataTypeId;
        entity.InputModeId = model.InputModeId;
        entity.AggregateMethodId = model.AggregateMethodId;
        entity.FormulaExpression = model.FormulaExpression;
        entity.ExcelSheetName = model.ExcelSheetName;
        entity.ExcelCellAddress = model.ExcelCellAddress;
        entity.IsRequired = model.IsRequired;
        entity.IsActive = model.IsActive;
        await _reportTemplateService.UpdateIndicatorAsync(entity);

        return RedirectToAction(nameof(Indicators), new { reportTemplateId = entity.ReportTemplateId });
    }
}

