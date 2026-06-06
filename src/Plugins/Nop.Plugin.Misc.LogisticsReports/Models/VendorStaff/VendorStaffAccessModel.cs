namespace Nop.Plugin.Misc.LogisticsReports.Models.VendorStaff;

public class VendorStaffAccessModel
{
    public int CustomerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string RoleNames { get; set; } = string.Empty;
    public bool HasAccess { get; set; }
    public bool IsLeader { get; set; }
}
