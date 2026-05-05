namespace Nop.Plugin.Misc.DocumentPortal.Models.Public;

public class DocumentDetailModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Code { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? CategoryName { get; set; }
    public string? TypeName { get; set; }
    public string? IssuerName { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string? PreviewUrl { get; set; }
    public string? DownloadUrl { get; set; }
    public bool HasContent { get; set; }
    public bool HasDownload { get; set; }
    public int? OwnerVendorId { get; set; }
    public string? OwnerVendorName { get; set; }
    public string? AccessScopeName { get; set; }
    public int ViewCount { get; set; }
    public int DownloadCount { get; set; }
    public bool CanDelete { get; set; }
    public bool CanAddComments { get; set; }
    public int NumberOfComments { get; set; }
    public AddDocumentCommentModel AddNewComment { get; set; } = new();
    public IList<DocumentCommentModel> Comments { get; set; } = new List<DocumentCommentModel>();
    public IList<DocumentListItemModel> RelatedDocuments { get; set; } = new List<DocumentListItemModel>();
}
