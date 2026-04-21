namespace Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;

public class UnitReportValueModel
{
    public int Id { get; set; }
    public int ReportIndicatorId { get; set; }
    public string IndicatorCode { get; set; } = string.Empty;
    public string IndicatorName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal? SelfValueNumber { get; set; }
    public string SelfValueText { get; set; } = string.Empty;
    public decimal? ChildAggregateValueNumber { get; set; }
    public decimal? AdjustmentValueNumber { get; set; }
    public decimal? FinalValueNumber { get; set; }
    public string Note { get; set; } = string.Empty;
}
