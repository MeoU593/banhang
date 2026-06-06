using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Localization;
using Nop.Services.Logging;
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
    private readonly ICustomerActivityService _customerActivityService;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;

    public UnitReportAdminController(
        IUnitReportService unitReportService,
        IAggregationService aggregationService,
        IReportPeriodService reportPeriodService,
        IRepository<Vendor> vendorRepository,
        IRepository<ReportIndicator> indicatorRepository,
        IWorkContext workContext,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        IPermissionService permissionService)
    {
        _unitReportService = unitReportService;
        _aggregationService = aggregationService;
        _reportPeriodService = reportPeriodService;
        _vendorRepository = vendorRepository;
        _indicatorRepository = indicatorRepository;
        _workContext = workContext;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Index(int? reportPeriodId)
    {
        var (_, currentVendor, canViewSystemWide, isLeader) = await GetAccessContextAsync();
        if (!canViewSystemWide && currentVendor == null)
            return AccessDeniedView();

        var periods = await _reportPeriodService.GetAllAsync();
        var selectedPeriodId = reportPeriodId ?? periods.FirstOrDefault()?.Id;
        var items = selectedPeriodId.HasValue
            ? await _unitReportService.GetByPeriodAsync(selectedPeriodId.Value)
            : new List<UnitReport>();

        if (!canViewSystemWide && currentVendor != null)
        {
            var directChildVendorIds = isLeader
                ? await _vendorRepository.Table
                    .Where(x => !x.Deleted && x.Active && x.ParentId == currentVendor.Id)
                    .Select(x => x.Id)
                    .ToListAsync()
                : new List<int>();

            items = items
                .Where(x => x.VendorId == currentVendor.Id || (isLeader && directChildVendorIds.Contains(x.VendorId)))
                .ToList();
        }

        var organizations = await _vendorRepository.Table.Where(v => !v.Deleted).ToDictionaryAsync(x => x.Id, x => x.Name);

        var model = new UnitReportIndexModel
        {
            ReportPeriods = periods,
            SelectedReportPeriodId = selectedPeriodId,
            Items = items.Select(x => new UnitReportListItemModel
            {
                Id = x.Id,
                ReportPeriodId = x.ReportPeriodId,
                VendorId = x.VendorId,
                VendorName = organizations.TryGetValue(x.VendorId, out var name) ? name : $"Đơn vị #{x.VendorId}",
                Status = ((Enums.UnitReportStatus)x.StatusId).ToString(),
                SubmittedOnUtc = x.SubmittedOnUtc,
                LastAggregatedOnUtc = x.LastAggregatedOnUtc
            }).ToList()
        };

        return View("~/Plugins/Misc.LogisticsReports/Views/UnitReportAdmin/Index.cshtml", model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var (customer, currentVendor, canViewSystemWide, isLeader) = await GetAccessContextAsync();
        if (!canViewSystemWide && currentVendor == null)
            return AccessDeniedView();

        var entity = await _unitReportService.GetByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(Index));

        var period = await _reportPeriodService.GetByIdAsync(entity.ReportPeriodId);
        if (period == null)
            return RedirectToAction(nameof(Index));

        var reportPeriod = period;

        var isOwnReport = currentVendor != null && entity.VendorId == currentVendor.Id;
        var isChildReport = currentVendor != null && await IsChildOfCurrentVendorAsync(entity, currentVendor.Id);

        if (!canViewSystemWide && !(isOwnReport || (isLeader && isChildReport)))
            return AccessDeniedView();

        await _unitReportService.InitializeValuesIfNeededAsync(id);
        var values = await _unitReportService.GetValuesAsync(id);
        var indicators = await _indicatorRepository.Table.ToDictionaryAsync(x => x.Id, x => x);
        var vendorName = (await _vendorRepository.GetByIdAsync(entity.VendorId))?.Name ?? $"Đơn vị #{entity.VendorId}";

        var canEditData = IsPeriodAcceptingUpdates(reportPeriod) && IsEditableStatus(entity.StatusId);

        var model = new UnitReportEditModel
        {
            Id = entity.Id,
            ReportPeriodId = entity.ReportPeriodId,
            ReportPeriodName = reportPeriod.Name,
            VendorId = entity.VendorId,
            VendorName = vendorName,
            Status = ((Enums.UnitReportStatus)entity.StatusId).ToString(),
            CanImport = canViewSystemWide ? canEditData : isOwnReport && canEditData,
            CanSave = canViewSystemWide ? canEditData : isOwnReport && canEditData,
            CanSubmit = canViewSystemWide ? canEditData : isOwnReport && canEditData,
            CanAggregate = IsPeriodAcceptingUpdates(reportPeriod) && entity.StatusId != (int)UnitReportStatus.Locked && (canViewSystemWide || (isOwnReport && isLeader)),
            CanReturn = IsPeriodAcceptingUpdates(reportPeriod) && entity.StatusId == (int)UnitReportStatus.Submitted && (canViewSystemWide || (isLeader && isChildReport)),
            CanLock = await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.LockReportPeriods, customer) && IsPeriodAcceptingUpdates(reportPeriod) && entity.StatusId == (int)UnitReportStatus.Submitted,
            Values = values.Select(v =>
            {
                indicators.TryGetValue(v.ReportIndicatorId, out var indicator);

                return new UnitReportValueModel
                {
                    Id = v.Id,
                    ReportIndicatorId = v.ReportIndicatorId,
                    IndicatorCode = indicator?.Code ?? $"#{v.ReportIndicatorId}",
                    IndicatorName = indicator?.Name ?? "Chỉ tiêu đã bị xóa",
                    UnitOfMeasure = indicator?.UnitOfMeasure ?? string.Empty,
                    SelfValueNumber = v.SelfValueNumber,
                    SelfValueText = v.SelfValueText,
                    ChildAggregateValueNumber = v.ChildAggregateValueNumber,
                    AdjustmentValueNumber = v.AdjustmentValueNumber,
                    FinalValueNumber = v.FinalValueNumber,
                    Note = v.Note
                };
            }).ToList()
        };

        return View("~/Plugins/Misc.LogisticsReports/Views/UnitReportAdmin/Edit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Save(UnitReportEditModel model)
    {
        var (customer, currentVendor, canViewSystemWide, isLeader) = await GetAccessContextAsync();
        if (!canViewSystemWide && currentVendor == null)
            return AccessDeniedView();

        var report = await _unitReportService.GetByIdAsync(model.Id);
        if (report == null)
            return RedirectToAction(nameof(Index));

        var period = await _reportPeriodService.GetByIdAsync(report.ReportPeriodId);
        if (period == null)
            return RedirectToAction(nameof(Index));

        if (!canViewSystemWide && report.VendorId != currentVendor!.Id)
            return AccessDeniedView();

        if (!IsPeriodAcceptingUpdates(period) || !IsEditableStatus(report.StatusId))
            return AccessDeniedView();

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

        try
        {
            await _unitReportService.SaveValuesAsync(model.Id, byId.Values.ToList(), customer.Id);
        }
        catch (InvalidOperationException)
        {
            return AccessDeniedView();
        }

        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Import(UnitReportEditModel model)
    {
        var (customer, currentVendor, canViewSystemWide, _) = await GetAccessContextAsync();
        if (!canViewSystemWide && currentVendor == null)
            return AccessDeniedView();

        var report = await _unitReportService.GetByIdAsync(model.Id);
        if (report == null)
            return RedirectToAction(nameof(Index));

        var period = await _reportPeriodService.GetByIdAsync(report.ReportPeriodId);
        if (period == null)
            return RedirectToAction(nameof(Index));

        if (!canViewSystemWide && report.VendorId != currentVendor!.Id)
            return AccessDeniedView();

        if (!IsPeriodAcceptingUpdates(period) || !IsEditableStatus(report.StatusId))
            return AccessDeniedView();

        if (model.ImportFile != null)
        {
            try
            {
                await _unitReportService.ImportAsync(model.Id, model.ImportFile, customer.Id);
            }
            catch (InvalidOperationException)
            {
                return AccessDeniedView();
            }
        }

        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Aggregate(int id)
    {
        var (customer, currentVendor, canViewSystemWide, isLeader) = await GetAccessContextAsync();
        if (!canViewSystemWide && currentVendor == null)
            return AccessDeniedView();

        var report = await _unitReportService.GetByIdAsync(id);
        if (report == null)
            return RedirectToAction(nameof(Index));

        var period = await _reportPeriodService.GetByIdAsync(report.ReportPeriodId);
        if (period == null)
            return RedirectToAction(nameof(Index));

        if (!canViewSystemWide && report.VendorId != currentVendor!.Id)
            return AccessDeniedView();

        if (!canViewSystemWide && !isLeader)
            return AccessDeniedView();

        if (!IsPeriodAcceptingUpdates(period) || report.StatusId == (int)UnitReportStatus.Locked)
            return AccessDeniedView();

        try
        {
            await _aggregationService.AggregateForUnitReportAsync(id, customer.Id);
        }
        catch (InvalidOperationException)
        {
            return AccessDeniedView();
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Submit(int id)
    {
        var (customer, currentVendor, canViewSystemWide, isLeader) = await GetAccessContextAsync();
        if (!canViewSystemWide && currentVendor == null)
            return AccessDeniedView();

        var report = await _unitReportService.GetByIdAsync(id);
        if (report == null)
            return RedirectToAction(nameof(Index));

        var period = await _reportPeriodService.GetByIdAsync(report.ReportPeriodId);
        if (period == null)
            return RedirectToAction(nameof(Index));

        if (!canViewSystemWide && report.VendorId != currentVendor!.Id)
            return AccessDeniedView();

        if (!IsPeriodAcceptingUpdates(period) || !IsEditableStatus(report.StatusId))
            return AccessDeniedView();

        try
        {
            await _unitReportService.SubmitAsync(id, customer.Id);
            await _customerActivityService.InsertActivityAsync(customer, NopActivityLogDefaults.SubmitUnitReport,
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.SubmitUnitReport"), report.Id), report);
        }
        catch (InvalidOperationException)
        {
            return AccessDeniedView();
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Return(int id, string returnReason)
    {
        var (customer, currentVendor, canViewSystemWide, isLeader) = await GetAccessContextAsync();
        if (!canViewSystemWide && currentVendor == null)
            return AccessDeniedView();

        var report = await _unitReportService.GetByIdAsync(id);
        if (report == null)
            return RedirectToAction(nameof(Index));

        var period = await _reportPeriodService.GetByIdAsync(report.ReportPeriodId);
        if (period == null)
            return RedirectToAction(nameof(Index));

        var isChildReport = currentVendor != null && await IsChildOfCurrentVendorAsync(report, currentVendor.Id);

        if (!canViewSystemWide && !isChildReport)
            return AccessDeniedView();

        if (!canViewSystemWide && !isLeader)
            return AccessDeniedView();

        if (!IsPeriodAcceptingUpdates(period) || report.StatusId != (int)UnitReportStatus.Submitted)
            return AccessDeniedView();

        if (string.IsNullOrWhiteSpace(returnReason))
            return RedirectToAction(nameof(Edit), new { id });

        try
        {
            await _unitReportService.ReturnAsync(id, customer.Id, returnReason);
        }
        catch (InvalidOperationException)
        {
            return AccessDeniedView();
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Lock(int id)
    {
        var (customer, _, _, _) = await GetAccessContextAsync();
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.LockReportPeriods, customer))
            return AccessDeniedView();

        var report = await _unitReportService.GetByIdAsync(id);
        if (report == null)
            return RedirectToAction(nameof(Index));

        var period = await _reportPeriodService.GetByIdAsync(report.ReportPeriodId);
        if (period == null)
            return RedirectToAction(nameof(Index));

        if (!IsPeriodAcceptingUpdates(period) || report.StatusId != (int)UnitReportStatus.Submitted)
            return AccessDeniedView();

        try
        {
            await _unitReportService.LockAsync(id, customer.Id);
        }
        catch (InvalidOperationException)
        {
            return AccessDeniedView();
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    private async Task<(Customer customer, Vendor? vendor, bool canViewSystemWide, bool isLeader)> GetAccessContextAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var canViewSystemWide = await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ViewSystemWideReports, customer);
        var isLeader = currentVendor != null && currentVendor.PmCustomerId == customer.Id;

        return (customer, currentVendor, canViewSystemWide, isLeader);
    }

    private async Task<bool> IsChildOfCurrentVendorAsync(UnitReport report, int currentVendorId)
    {
        var vendor = await _vendorRepository.GetByIdAsync(report.VendorId);
        return vendor != null && !vendor.Deleted && vendor.Active && vendor.ParentId == currentVendorId;
    }

    private static bool IsEditableStatus(int statusId)
    {
        return statusId == (int)UnitReportStatus.Draft
            || statusId == (int)UnitReportStatus.Imported
            || statusId == (int)UnitReportStatus.Returned;
    }

    private static bool IsPeriodClosed(ReportPeriod period)
    {
        return period.IsLocked || period.StatusId == (int)ReportPeriodStatus.Closed || period.StatusId == (int)ReportPeriodStatus.Locked;
    }

    private static bool IsPeriodAcceptingUpdates(ReportPeriod period)
    {
        if (IsPeriodClosed(period))
            return false;

        return period.StatusId == (int)ReportPeriodStatus.Open || period.StatusId == (int)ReportPeriodStatus.Aggregating;
    }
}
