using Nop.Core;

namespace Nop.Plugin.Misc.LogisticsReports.Domain;

public class UnitReportWorkflowLog : BaseEntity
{
    public int UnitReportId { get; set; }
    public string Action { get; set; } = string.Empty;
    public int FromStatusId { get; set; }
    public int ToStatusId { get; set; }
    public int ActionByCustomerId { get; set; }
    public DateTime ActionOnUtc { get; set; }
    public string Comment { get; set; } = string.Empty;
}
