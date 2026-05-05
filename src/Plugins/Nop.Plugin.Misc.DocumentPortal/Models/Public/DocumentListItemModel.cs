namespace Nop.Plugin.Misc.DocumentPortal.Models.Public;

public class DocumentListItemModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Code { get; set; }
    public string? Summary { get; set; }
    public string? CategoryName { get; set; }
    public string? TypeName { get; set; }
    public string? IssuerName { get; set; }
    public DateTime? IssuedDate { get; set; }
    public int DownloadCount { get; set; }
    public bool AllowDownload { get; set; }
    public int? UploadedByCustomerId { get; set; }
    public int? OwnerVendorId { get; set; }
    public string? OwnerVendorName { get; set; }
    public string? AccessScopeName { get; set; }
    public bool CanDelete { get; set; }
}
