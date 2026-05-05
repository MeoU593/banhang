namespace Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;

public class UnitReportListItemModel
{
    public int Id { get; set; }
    public int ReportPeriodId { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedOnUtc { get; set; }
    public DateTime? LastAggregatedOnUtc { get; set; }
}
