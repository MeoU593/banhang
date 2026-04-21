using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class AggregationService : IAggregationService
{
    private readonly IRepository<UnitReport> _unitReportRepository;
    private readonly IRepository<UnitReportValue> _valueRepository;
    private readonly IRepository<UnitReportWorkflowLog> _workflowRepository;

    public AggregationService(
        IRepository<UnitReport> unitReportRepository,
        IRepository<UnitReportValue> valueRepository,
        IRepository<UnitReportWorkflowLog> workflowRepository)
    {
        _unitReportRepository = unitReportRepository;
        _valueRepository = valueRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task AggregateForUnitReportAsync(int unitReportId, int customerId)
    {
        var unitReport = await _unitReportRepository.GetByIdAsync(unitReportId) ?? throw new ArgumentException("Không tìm thấy báo cáo đơn vị");
        var childReports = await _unitReportRepository.Table
            .Where(x => x.ReportPeriodId == unitReport.ReportPeriodId
                        && x.ParentOrganizationUnitId == unitReport.OrganizationUnitId
                        && x.StatusId == (int)UnitReportStatus.Submitted)
            .ToListAsync();

        var targetValues = await _valueRepository.Table.Where(x => x.UnitReportId == unitReportId).ToListAsync();
        if (!targetValues.Any())
            return;

        var childIds = childReports.Select(x => x.Id).ToList();
        var childValues = await _valueRepository.Table.Where(x => childIds.Contains(x.UnitReportId)).ToListAsync();

        foreach (var target in targetValues)
        {
            var aggregate = childValues.Where(x => x.ReportIndicatorId == target.ReportIndicatorId).Sum(x => x.FinalValueNumber ?? 0);
            target.ChildAggregateValueNumber = aggregate;
            target.FinalValueNumber = (target.SelfValueNumber ?? 0) + (target.ChildAggregateValueNumber ?? 0) + (target.AdjustmentValueNumber ?? 0);
            target.SourceTypeId = (int)ValueSourceType.SystemAggregated;
            target.LastUpdatedByCustomerId = customerId;
            target.LastUpdatedOnUtc = DateTime.UtcNow;
        }

        unitReport.LastAggregatedOnUtc = DateTime.UtcNow;
        await _valueRepository.UpdateAsync(targetValues);
        await _unitReportRepository.UpdateAsync(unitReport);
        await _workflowRepository.InsertAsync(new UnitReportWorkflowLog
        {
            UnitReportId = unitReportId,
            Action = "Tổng hợp",
            FromStatusId = unitReport.StatusId,
            ToStatusId = unitReport.StatusId,
            ActionByCustomerId = customerId,
            ActionOnUtc = DateTime.UtcNow,
            Comment = $"Đã tổng hợp từ {childReports.Count} báo cáo đơn vị cấp dưới trực tiếp"
        });
    }
}
