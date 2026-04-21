using Nop.Core;

namespace Nop.Plugin.Misc.LogisticsReports.Domain;

public class CustomerOrganizationUnit : BaseEntity
{
    public int CustomerId { get; set; }
    public int OrganizationUnitId { get; set; }
    public bool IsPrimary { get; set; }
    public bool CanInputReport { get; set; }
    public bool CanReviewChildReports { get; set; }
    public bool CanSubmitReport { get; set; }
    public bool CanLockReport { get; set; }
}
