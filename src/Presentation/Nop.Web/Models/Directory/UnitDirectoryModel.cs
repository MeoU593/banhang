namespace Nop.Web.Models.Directory;

public partial record UnitDirectoryModel
{
    public IList<UnitDirectoryVendorNodeModel> Vendors { get; set; } = new List<UnitDirectoryVendorNodeModel>();
    public IList<UnitDirectoryPersonModel> People { get; set; } = new List<UnitDirectoryPersonModel>();
    public UnitDirectoryPersonModel SelectedPerson { get; set; }
    public int SelectedVendorId { get; set; }
    public int SelectedPersonId { get; set; }
    public string SearchKeyword { get; set; } = string.Empty;
    public string SearchRank { get; set; } = string.Empty;
    public string SearchPositionTitle { get; set; } = string.Empty;
}

public partial record UnitDirectoryVendorNodeModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public bool Active { get; set; }
}

public partial record UnitDirectoryPersonModel
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MilitaryCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
}
