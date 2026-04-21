using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;

public class UnitReportIndexModel
{
    public IList<ReportPeriod> ReportPeriods { get; set; } = new List<ReportPeriod>();
    public IList<UnitReportListItemModel> Items { get; set; } = new List<UnitReportListItemModel>();
    public int? SelectedReportPeriodId { get; set; }
}
