using Nop.Core;

namespace Nop.Plugin.Misc.LogisticsReports.Domain;

public class UnitReport : BaseEntity
{
    public int ReportPeriodId { get; set; }
    public int VendorId { get; set; }
    public int? ParentVendorId { get; set; }
    public int StatusId { get; set; }
    public int CurrentVersion { get; set; }
    public DateTime? SubmittedOnUtc { get; set; }
    public int? SubmittedByCustomerId { get; set; }
    public string ReturnedReason { get; set; } = string.Empty;
    public DateTime? LockedOnUtc { get; set; }
    public int? LockedByCustomerId { get; set; }
    public DateTime? LastAggregatedOnUtc { get; set; }
}
