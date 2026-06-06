namespace Nop.Web.Areas.Admin.Models.Vendors;

public partial record VendorDetailsModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public int ScopeUnitCount { get; set; }
    public bool CanEditVendor { get; set; }
    public bool IsSystemDashboard { get; set; }

    public int ProductCount { get; set; }
    public int NewsCount { get; set; }
    public int BlogDocumentCount { get; set; }
    public int PortalDocumentCount { get; set; }
    public int StaffCount { get; set; }
    public int ReportCount { get; set; }

    public string ChartLabelsJson { get; set; } = "[]";
    public string ProductSeriesJson { get; set; } = "[]";
    public string NewsSeriesJson { get; set; } = "[]";
    public string BlogDocumentSeriesJson { get; set; } = "[]";
    public string PortalDocumentSeriesJson { get; set; } = "[]";
    public string StaffSeriesJson { get; set; } = "[]";
    public string ReportSeriesJson { get; set; } = "[]";

    public IList<VendorDetailsListItemModel> Products { get; set; } = new List<VendorDetailsListItemModel>();
    public IList<VendorDetailsListItemModel> News { get; set; } = new List<VendorDetailsListItemModel>();
    public IList<VendorDetailsListItemModel> BlogDocuments { get; set; } = new List<VendorDetailsListItemModel>();
    public IList<VendorDetailsListItemModel> PortalDocuments { get; set; } = new List<VendorDetailsListItemModel>();
    public IList<VendorDetailsListItemModel> Reports { get; set; } = new List<VendorDetailsListItemModel>();
    public IList<VendorDetailsDirectoryItemModel> DirectoryEntries { get; set; } = new List<VendorDetailsDirectoryItemModel>();
    public IList<VendorDetailsStaffItemModel> Staff { get; set; } = new List<VendorDetailsStaffItemModel>();
}

public partial record VendorDetailsListItemModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public DateTime? CreatedOnUtc { get; set; }
    public string Url { get; set; } = string.Empty;
}

public partial record VendorDetailsSectionModel
{
    public string Title { get; set; } = string.Empty;
    public IList<VendorDetailsListItemModel> Items { get; set; } = new List<VendorDetailsListItemModel>();
}

public partial record VendorDetailsStaffItemModel
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleNames { get; set; } = string.Empty;
    public bool IsLeader { get; set; }
}

public partial record VendorDetailsDirectoryItemModel
{
    public string FullName { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MilitaryCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public bool Published { get; set; }
}
