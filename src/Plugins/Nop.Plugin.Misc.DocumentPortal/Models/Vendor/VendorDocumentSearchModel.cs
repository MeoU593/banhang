using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Vendor;

public class VendorDocumentSearchModel : BaseSearchModel
{
    public string? SearchKeywords { get; set; }
    public int SearchCategoryId { get; set; }
    public int SearchTypeId { get; set; }
    public int SearchPublishedId { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableTypes { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailablePublishedOptions { get; set; } = new List<SelectListItem>();

    public bool IsVendorManager { get; set; }
    public string VendorName { get; set; } = string.Empty;
}
