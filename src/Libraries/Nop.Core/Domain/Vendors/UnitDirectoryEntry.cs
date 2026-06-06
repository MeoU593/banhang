namespace Nop.Core.Domain.Vendors;

public partial class UnitDirectoryEntry : BaseEntity
{
    public int VendorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MilitaryCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool Published { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}
