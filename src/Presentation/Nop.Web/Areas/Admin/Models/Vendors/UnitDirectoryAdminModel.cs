using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Areas.Admin.Models.Vendors;

public partial record UnitDirectoryAdminModel
{
    public int VendorId { get; set; }
    public int SelectedVendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public bool IsSystemAdmin { get; set; }
    public bool CanSelectVendor { get; set; }
    public string SearchKeyword { get; set; } = string.Empty;
    public string SearchRank { get; set; } = string.Empty;
    public string SearchPositionTitle { get; set; } = string.Empty;
    public int SearchPublishedId { get; set; }
    public UnitDirectoryEntryModel Entry { get; set; } = new();
    public IList<SelectListItem> AvailableVendors { get; set; } = new List<SelectListItem>();
    public IList<UnitDirectoryEntryModel> Entries { get; set; } = new List<UnitDirectoryEntryModel>();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int FirstItemIndex { get; set; }
    public int LastItemIndex { get; set; }
    public IList<SelectListItem> AvailablePageSizes { get; set; } = new List<SelectListItem>();
}

public partial record UnitDirectoryEntryModel
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public int FilterVendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MilitaryCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool Published { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
