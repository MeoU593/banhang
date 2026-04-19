using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Admin;

public record DocumentSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.List.SearchTitle")]
    public string? SearchTitle { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.List.SearchCode")]
    public string? SearchCode { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.List.SearchCategory")]
    public int SearchCategoryId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.List.SearchType")]
    public int SearchTypeId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.List.SearchIssuer")]
    public int SearchIssuerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.List.SearchPublished")]
    public int SearchPublishedId { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableTypes { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableIssuers { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailablePublishedOptions { get; set; } = new List<SelectListItem>();
}
