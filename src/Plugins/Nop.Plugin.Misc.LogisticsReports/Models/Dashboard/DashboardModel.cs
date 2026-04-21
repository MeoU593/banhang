namespace Nop.Plugin.Misc.LogisticsReports.Models.Dashboard;

public class DashboardModel
{
    public int TotalOrganizations { get; set; }
    public int TotalTemplates { get; set; }
    public int TotalPeriods { get; set; }
    public int TotalUnitReports { get; set; }
    public int SubmittedUnitReports { get; set; }
    public int ReturnedUnitReports { get; set; }
    public int LockedUnitReports { get; set; }
}
