using Nop.Data;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class AggregationService : IAggregationService
{
    private readonly IRepository<UnitReport> _unitReportRepository;
    private readonly IRepository<UnitReportValue> _valueRepository;
    private readonly IRepository<UnitReportWorkflowLog> _workflowRepository;
    private readonly IRepository<ReportPeriod> _periodRepository;
    private readonly IRepository<Vendor> _vendorRepository;

    public AggregationService(
        IRepository<UnitReport> unitReportRepository,
        IRepository<UnitReportValue> valueRepository,
        IRepository<UnitReportWorkflowLog> workflowRepository,
        IRepository<ReportPeriod> periodRepository,
        IRepository<Vendor> vendorRepository)
    {
        _unitReportRepository = unitReportRepository;
        _valueRepository = valueRepository;
        _workflowRepository = workflowRepository;
        _periodRepository = periodRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task AggregateForUnitReportAsync(int unitReportId, int customerId)
    {
        var unitReport = await _unitReportRepository.GetByIdAsync(unitReportId) ?? throw new ArgumentException("Không tìm thấy báo cáo đơn vị");
        var period = await _periodRepository.GetByIdAsync(unitReport.ReportPeriodId) ?? throw new ArgumentException("Không tìm thấy kỳ báo cáo");
        if (period.IsLocked || period.StatusId == (int)ReportPeriodStatus.Closed || period.StatusId == (int)ReportPeriodStatus.Locked)
            throw new InvalidOperationException("Kỳ báo cáo đã khóa, không thể tổng hợp");

        if (period.StatusId != (int)ReportPeriodStatus.Open && period.StatusId != (int)ReportPeriodStatus.Aggregating)
            throw new InvalidOperationException("Kỳ báo cáo hiện không cho phép tổng hợp");

        if (unitReport.StatusId == (int)UnitReportStatus.Locked)
            throw new InvalidOperationException("Báo cáo đã khóa, không thể tổng hợp");

        var childVendorIds = await _vendorRepository.Table
            .Where(x => !x.Deleted && x.Active && x.ParentId == unitReport.VendorId)
            .Select(x => x.Id)
            .ToListAsync();

        var childReports = await _unitReportRepository.Table
            .Where(x => x.ReportPeriodId == unitReport.ReportPeriodId
                        && childVendorIds.Contains(x.VendorId)
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
