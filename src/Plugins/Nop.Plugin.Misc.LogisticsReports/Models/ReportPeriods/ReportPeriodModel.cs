using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.LogisticsReports.Models.ReportPeriods;

public class ReportPeriodModel
{
    public int Id { get; set; }
    public int ReportTemplateId { get; set; }

    [Required]
    [StringLength(128)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(512)]
    public string Name { get; set; } = string.Empty;
    public int PeriodTypeId { get; set; }
    public DateTime FromDateUtc { get; set; }
    public DateTime ToDateUtc { get; set; }
    public DateTime DeadlineUtc { get; set; }
    public int StatusId { get; set; }
    public bool IsLocked { get; set; }
}
