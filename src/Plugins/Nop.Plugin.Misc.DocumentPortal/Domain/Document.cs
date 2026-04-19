using Nop.Core;

namespace Nop.Plugin.Misc.DocumentPortal.Domain;

public class Document : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Code { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? Keywords { get; set; }

    public int? DocumentCategoryId { get; set; }
    public int? DocumentTypeId { get; set; }
    public int? IssuerId { get; set; }

    public DateTime? IssuedDate { get; set; }
    public DateTime? EffectiveDate { get; set; }

    public int? DownloadId { get; set; }
    public int? ThumbnailPictureId { get; set; }

    public bool Published { get; set; }
    public bool ShowOnHomepage { get; set; }
    public bool AllowDownload { get; set; }
    public int ViewCount { get; set; }
    public int DownloadCount { get; set; }
    public int DisplayOrder { get; set; }
    public bool Deleted { get; set; }

    public int? UploadedByCustomerId { get; set; }
    public int? UploadedByVendorId { get; set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}
