using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Public;

public class DocumentUploadModel
{
    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Summary { get; set; }
    public int? DocumentCategoryId { get; set; }
    public int? DocumentTypeId { get; set; }
    public int? IssuerId { get; set; }
    public DateTime? IssuedDate { get; set; }
    public IFormFile? File { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableTypes { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableIssuers { get; set; } = new List<SelectListItem>();
}
