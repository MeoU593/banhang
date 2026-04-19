using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Public;

public class DocumentListPageModel
{
    public IList<DocumentListItemModel> Documents { get; set; } = new List<DocumentListItemModel>();
    public DocumentSearchModel Search { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool IsAuthenticated { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableTypes { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableIssuers { get; set; } = new List<SelectListItem>();
}
