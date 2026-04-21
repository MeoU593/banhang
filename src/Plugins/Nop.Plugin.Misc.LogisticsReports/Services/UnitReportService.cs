using Microsoft.AspNetCore.Http;
using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class UnitReportService : IUnitReportService
{
    private readonly IRepository<UnitReport> _unitReportRepository;
    private readonly IRepository<UnitReportValue> _valueRepository;
    private readonly IRepository<UnitReportWorkflowLog> _workflowRepository;
    private readonly IRepository<UnitReportImportHistory> _importHistoryRepository;
    private readonly IRepository<ReportPeriod> _periodRepository;
    private readonly IRepository<ReportIndicator> _indicatorRepository;
    private readonly IStorageService _storageService;
    private readonly IExcelImportService _excelImportService;

    public UnitReportService(
        IRepository<UnitReport> unitReportRepository,
        IRepository<UnitReportValue> valueRepository,
        IRepository<UnitReportWorkflowLog> workflowRepository,
        IRepository<UnitReportImportHistory> importHistoryRepository,
        IRepository<ReportPeriod> periodRepository,
        IRepository<ReportIndicator> indicatorRepository,
        IStorageService storageService,
        IExcelImportService excelImportService)
    {
        _unitReportRepository = unitReportRepository;
        _valueRepository = valueRepository;
        _workflowRepository = workflowRepository;
        _importHistoryRepository = importHistoryRepository;
        _periodRepository = periodRepository;
        _indicatorRepository = indicatorRepository;
        _storageService = storageService;
        _excelImportService = excelImportService;
    }

    public async Task<IList<UnitReport>> GetByPeriodAsync(int reportPeriodId)
        => await _unitReportRepository.Table.Where(x => x.ReportPeriodId == reportPeriodId).OrderBy(x => x.OrganizationUnitId).ToListAsync();

    public async Task<IList<UnitReport>> GetByParentOrganizationAsync(int reportPeriodId, int organizationUnitId)
        => await _unitReportRepository.Table.Where(x => x.ReportPeriodId == reportPeriodId && x.ParentOrganizationUnitId == organizationUnitId).ToListAsync();

    public async Task<UnitReport?> GetByIdAsync(int id) => await _unitReportRepository.GetByIdAsync(id);

    public async Task<IList<UnitReportValue>> GetValuesAsync(int unitReportId)
        => await _valueRepository.Table.Where(x => x.UnitReportId == unitReportId).OrderBy(x => x.ReportIndicatorId).ToListAsync();

    public async Task InitializeValuesIfNeededAsync(int unitReportId)
    {
        var unitReport = await GetByIdAsync(unitReportId) ?? throw new ArgumentException("Không tìm thấy báo cáo đơn vị");
        var existing = await _valueRepository.Table.Where(x => x.UnitReportId == unitReportId).AnyAsync();
        if (existing)
            return;

        var reportPeriod = await _periodRepository.GetByIdAsync(unitReport.ReportPeriodId) ?? throw new ArgumentException("Report period not found");
        var indicators = await _indicatorRepository.Table.Where(x => x.ReportTemplateId == reportPeriod.ReportTemplateId && x.IsActive).ToListAsync();
        if (!indicators.Any())
            return;

        await _valueRepository.InsertAsync(indicators.Select(i => new UnitReportValue
        {
            UnitReportId = unitReportId,
            ReportIndicatorId = i.Id,
            SourceTypeId = (int)ValueSourceType.None
        }).ToList());
    }

    public async Task SaveValuesAsync(int unitReportId, IList<UnitReportValue> values, int customerId)
    {
        var now = DateTime.UtcNow;
        foreach (var value in values)
        {
            value.LastUpdatedByCustomerId = customerId;
            value.LastUpdatedOnUtc = now;
            value.FinalValueNumber = (value.SelfValueNumber ?? 0) + (value.ChildAggregateValueNumber ?? 0) + (value.AdjustmentValueNumber ?? 0);
        }

        await _valueRepository.UpdateAsync(values);
    }

    public async Task SubmitAsync(int unitReportId, int customerId)
    {
        var entity = await GetByIdAsync(unitReportId) ?? throw new ArgumentException("Không tìm thấy báo cáo đơn vị");
        var fromStatus = entity.StatusId;
        entity.StatusId = (int)UnitReportStatus.Submitted;
        entity.SubmittedByCustomerId = customerId;
        entity.SubmittedOnUtc = DateTime.UtcNow;
        await _unitReportRepository.UpdateAsync(entity);
        await AddWorkflowAsync(entity.Id, fromStatus, entity.StatusId, customerId, "Submit", string.Empty);
    }

    public async Task ReturnAsync(int unitReportId, int customerId, string reason)
    {
        var entity = await GetByIdAsync(unitReportId) ?? throw new ArgumentException("Unit report not found");
        var fromStatus = entity.StatusId;
        entity.StatusId = (int)UnitReportStatus.Returned;
        entity.ReturnedReason = reason;
        await _unitReportRepository.UpdateAsync(entity);
        await AddWorkflowAsync(entity.Id, fromStatus, entity.StatusId, customerId, "Trả lại", reason);
    }

    public async Task LockAsync(int unitReportId, int customerId)
    {
        var entity = await GetByIdAsync(unitReportId) ?? throw new ArgumentException("Không tìm thấy báo cáo đơn vị");
        var fromStatus = entity.StatusId;
        entity.StatusId = (int)UnitReportStatus.Locked;
        entity.LockedByCustomerId = customerId;
        entity.LockedOnUtc = DateTime.UtcNow;
        await _unitReportRepository.UpdateAsync(entity);
        await AddWorkflowAsync(entity.Id, fromStatus, entity.StatusId, customerId, "Khóa", string.Empty);
    }

    public async Task<int> ImportAsync(int unitReportId, IFormFile file, int customerId)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Tệp nhập liệu đang rỗng");

        await using var stream = file.OpenReadStream();
        var storedPath = await _storageService.SaveImportAsync(file.FileName, stream);
        var result = await _excelImportService.ImportUnitReportAsync(unitReportId, storedPath, customerId);

        await _importHistoryRepository.InsertAsync(new UnitReportImportHistory
        {
            UnitReportId = unitReportId,
            FileName = file.FileName,
            StoredFilePath = storedPath,
            ImportedByCustomerId = customerId,
            ImportedOnUtc = DateTime.UtcNow,
            SuccessCount = result.successCount,
            ErrorCount = result.errorCount,
            ErrorLogPath = result.errorLog
        });

        var report = await GetByIdAsync(unitReportId) ?? throw new ArgumentException("Không tìm thấy báo cáo đơn vị");
        if (result.successCount > 0)
        {
            report.StatusId = (int)UnitReportStatus.Imported;
            await _unitReportRepository.UpdateAsync(report);
        }

        await AddWorkflowAsync(unitReportId, report.StatusId, report.StatusId, customerId, "Nhập liệu", $"Thành công: {result.successCount}; Lỗi: {result.errorCount}");
        return result.successCount;
    }

    private async Task AddWorkflowAsync(int unitReportId, int fromStatus, int toStatus, int customerId, string action, string comment)
    {
        await _workflowRepository.InsertAsync(new UnitReportWorkflowLog
        {
            UnitReportId = unitReportId,
            Action = action,
            FromStatusId = fromStatus,
            ToStatusId = toStatus,
            ActionByCustomerId = customerId,
            ActionOnUtc = DateTime.UtcNow,
            Comment = comment
        });
    }
}
