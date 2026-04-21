using Nop.Core;

namespace Nop.Plugin.Misc.LogisticsReports.Domain;

public class ReportPeriod : BaseEntity
{
    public int ReportTemplateId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int PeriodTypeId { get; set; }
    public DateTime FromDateUtc { get; set; }
    public DateTime ToDateUtc { get; set; }
    public DateTime DeadlineUtc { get; set; }
    public int StatusId { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
