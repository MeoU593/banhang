using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Admin;

public record DocumentModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Title")]
    public string Title { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Slug")]
    public string? Slug { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Code")]
    public string? Code { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Summary")]
    public string? Summary { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Content")]
    public string? Content { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Keywords")]
    public string? Keywords { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.DocumentCategory")]
    public int? DocumentCategoryId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.DocumentType")]
    public int? DocumentTypeId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Issuer")]
    public int? IssuerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.IssuedDate")]
    public DateTime? IssuedDate { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.EffectiveDate")]
    public DateTime? EffectiveDate { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Download")]
    public int? DownloadId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Thumbnail")]
    public int? ThumbnailPictureId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.AllowDownload")]
    public bool AllowDownload { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.ShowOnHomepage")]
    public bool ShowOnHomepage { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Document.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public int ViewCount { get; set; }
    public int DownloadCount { get; set; }
    public DateTime? CreatedOnUtc { get; set; }
    public DateTime? UpdatedOnUtc { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableTypes { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableIssuers { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableCustomerRoles { get; set; } = new List<SelectListItem>();
    public IList<int> SelectedCustomerRoleIds { get; set; } = new List<int>();
}
