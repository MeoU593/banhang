namespace Nop.Plugin.Misc.LogisticsReports.Models.UnitProfile;

public record UnitProfileModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public bool CanEdit { get; set; }
    public int ChildUnitCount { get; set; }
    public int ProductCount { get; set; }
    public int StaffCount { get; set; }
}
