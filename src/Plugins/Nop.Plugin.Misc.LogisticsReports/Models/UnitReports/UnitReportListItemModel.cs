namespace Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;

public class UnitReportListItemModel
{
    public int Id { get; set; }
    public int ReportPeriodId { get; set; }
    public int OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedOnUtc { get; set; }
    public DateTime? LastAggregatedOnUtc { get; set; }
}
