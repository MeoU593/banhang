using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Areas.Admin.Models.Vendors;

public partial record UnitStaffModel
{
    public int SelectedVendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public bool IsSystemAdmin { get; set; }
    public bool CanSelectVendor { get; set; }
    public IList<SelectListItem> AvailableVendors { get; set; } = new List<SelectListItem>();
    public IList<UnitStaffItemModel> Staff { get; set; } = new List<UnitStaffItemModel>();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int FirstItemIndex { get; set; }
    public int LastItemIndex { get; set; }
    public IList<SelectListItem> AvailablePageSizes { get; set; } = new List<SelectListItem>();
}

public partial record UnitStaffItemModel
{
    public int CustomerId { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string MilitaryCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string RoleNames { get; set; } = string.Empty;
    public string CreatedOnText { get; set; } = string.Empty;
    public string LastLoginText { get; set; } = string.Empty;
    public bool Active { get; set; }
    public bool HasUnitAdminAccess { get; set; }
    public bool IsLeader { get; set; }
    public bool CanManageAccess { get; set; }
    public bool CanEditInfo { get; set; }
}

public partial record UnitStaffEditModel
{
    public int CustomerId { get; set; }
    public int VendorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MilitaryCode { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
