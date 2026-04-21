namespace Nop.Plugin.Misc.LogisticsReports.Models.Organizations;

public class CustomerOrganizationUnitModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int OrganizationUnitId { get; set; }
    public bool IsPrimary { get; set; }
    public bool CanInputReport { get; set; }
    public bool CanReviewChildReports { get; set; }
    public bool CanSubmitReport { get; set; }
    public bool CanLockReport { get; set; }
    public string? CustomerDisplay { get; set; }
    public string? OrganizationDisplay { get; set; }
    public IList<Shared.SelectItem> Customers { get; set; } = new List<Shared.SelectItem>();
    public IList<Shared.SelectItem> Organizations { get; set; } = new List<Shared.SelectItem>();
}
