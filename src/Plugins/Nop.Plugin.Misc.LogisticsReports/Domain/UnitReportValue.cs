using Nop.Core;

namespace Nop.Plugin.Misc.LogisticsReports.Domain;

public class UnitReportValue : BaseEntity
{
    public int UnitReportId { get; set; }
    public int ReportIndicatorId { get; set; }
    public decimal? SelfValueNumber { get; set; }
    public string SelfValueText { get; set; } = string.Empty;
    public decimal? ChildAggregateValueNumber { get; set; }
    public decimal? AdjustmentValueNumber { get; set; }
    public decimal? FinalValueNumber { get; set; }
    public string FinalValueText { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public int SourceTypeId { get; set; }
    public int? LastUpdatedByCustomerId { get; set; }
    public DateTime? LastUpdatedOnUtc { get; set; }
}
