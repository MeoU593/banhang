using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
public class UnitReportAdminController : BaseAdminController
{
    private readonly IUnitReportService _unitReportService;
    private readonly IAggregationService _aggregationService;
    private readonly IReportPeriodService _reportPeriodService;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IRepository<ReportIndicator> _indicatorRepository;
    private readonly IWorkContext _workContext;
    private readonly IPermissionService _permissionService;

    public UnitReportAdminController(
        IUnitReportService unitReportService,
        IAggregationService aggregationService,
        IReportPeriodService reportPeriodService,
        IRepository<Vendor> vendorRepository,
        IRepository<ReportIndicator> indicatorRepository,
        IWorkContext workContext,
        IPermissionService permissionService)
    {
        _unitReportService = unitReportService;
        _aggregationService = aggregationService;
        _reportPeriodService = reportPeriodService;
        _vendorRepository = vendorRepository;
        _indicatorRepository = indicatorRepository;
        _workContext = workContext;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Index(int? reportPeriodId)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewSystemWideReports))
            return AccessDeniedView();

        var periods = await _reportPeriodService.GetAllAsync();
        var selectedPeriodId = reportPeriodId ?? periods.FirstOrDefault()?.Id;
        var items = selectedPeriodId.HasValue ? await _unitReportService.GetByPeriodAsync(selectedPeriodId.Value) : new List<UnitReport>();
        var organizations = await _vendorRepository.Table.Where(v => !v.Deleted).ToDictionaryAsync(x => x.Id, x => x.Name);

        var model = new UnitReportIndexModel
        {
            ReportPeriods = periods,
            SelectedReportPeriodId = selectedPeriodId,
            Items = items.Select(x => new UnitReportListItemModel
            {
                Id = x.Id,
                ReportPeriodId = x.ReportPeriodId,
                OrganizationUnitId = x.OrganizationUnitId,
                OrganizationUnitName = organizations.TryGetValue(x.OrganizationUnitId, out var name) ? name : $"Org #{x.OrganizationUnitId}",
                Status = ((Enums.UnitReportStatus)x.StatusId).ToString(),
                SubmittedOnUtc = x.SubmittedOnUtc,
                LastAggregatedOnUtc = x.LastAggregatedOnUtc
            }).ToList()
        };

        return View("~/Plugins/Misc.LogisticsReports/Views/UnitReportAdmin/Index.cshtml", model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewOwnUnitReports))
            return AccessDeniedView();

        var entity = await _unitReportService.GetByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(Index));

        await _unitReportService.InitializeValuesIfNeededAsync(id);
        var values = await _unitReportService.GetValuesAsync(id);
        var indicators = await _indicatorRepository.Table.ToDictionaryAsync(x => x.Id, x => x);
        var period = await _reportPeriodService.GetByIdAsync(entity.ReportPeriodId);
        var organizationName = (await _vendorRepository.GetByIdAsync(entity.OrganizationUnitId))?.Name ?? $"Org #{entity.OrganizationUnitId}";

        var model = new UnitReportEditModel
        {
            Id = entity.Id,
            ReportPeriodId = entity.ReportPeriodId,
            ReportPeriodName = period?.Name ?? string.Empty,
            OrganizationUnitId = entity.OrganizationUnitId,
            OrganizationUnitName = organizationName,
            Status = ((Enums.UnitReportStatus)entity.StatusId).ToString(),
            Values = values.Select(v => new UnitReportValueModel
            {
                Id = v.Id,
                ReportIndicatorId = v.ReportIndicatorId,
                IndicatorCode = indicators[v.ReportIndicatorId].Code,
                IndicatorName = indicators[v.ReportIndicatorId].Name,
                UnitOfMeasure = indicators[v.ReportIndicatorId].UnitOfMeasure,
                SelfValueNumber = v.SelfValueNumber,
                SelfValueText = v.SelfValueText,
                ChildAggregateValueNumber = v.ChildAggregateValueNumber,
                AdjustmentValueNumber = v.AdjustmentValueNumber,
                FinalValueNumber = v.FinalValueNumber,
                Note = v.Note
            }).ToList()
        };

        return View("~/Plugins/Misc.LogisticsReports/Views/UnitReportAdmin/Edit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Save(UnitReportEditModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.InputOwnUnitReports))
            return AccessDeniedView();

        var customer = await _workContext.GetCurrentCustomerAsync();
        var existing = await _unitReportService.GetValuesAsync(model.Id);
        var byId = existing.ToDictionary(x => x.Id, x => x);

        foreach (var item in model.Values)
        {
            if (!byId.TryGetValue(item.Id, out var entity))
                continue;

            entity.SelfValueNumber = item.SelfValueNumber;
            entity.SelfValueText = item.SelfValueText ?? string.Empty;
            entity.AdjustmentValueNumber = item.AdjustmentValueNumber;
            entity.Note = item.Note ?? string.Empty;
        }

        await _unitReportService.SaveValuesAsync(model.Id, byId.Values.ToList(), customer.Id);
        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Import(UnitReportEditModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.InputOwnUnitReports))
            return AccessDeniedView();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (model.ImportFile != null)
            await _unitReportService.ImportAsync(model.Id, model.ImportFile, customer.Id);

        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Aggregate(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewChildUnitReports))
            return AccessDeniedView();

        var customer = await _workContext.GetCurrentCustomerAsync();
        await _aggregationService.AggregateForUnitReportAsync(id, customer.Id);
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Submit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.SubmitOwnUnitReports))
            return AccessDeniedView();

        var customer = await _workContext.GetCurrentCustomerAsync();
        await _unitReportService.SubmitAsync(id, customer.Id);
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Return(int id, string reason)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ReturnChildUnitReports))
            return AccessDeniedView();

        if (string.IsNullOrWhiteSpace(reason))
            return RedirectToAction(nameof(Edit), new { id });

        var customer = await _workContext.GetCurrentCustomerAsync();
        await _unitReportService.ReturnAsync(id, customer.Id, reason);
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Lock(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.LockReportPeriods))
            return AccessDeniedView();

        var customer = await _workContext.GetCurrentCustomerAsync();
        await _unitReportService.LockAsync(id, customer.Id);
        return RedirectToAction(nameof(Edit), new { id });
    }
}

