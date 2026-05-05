namespace Nop.Plugin.Misc.DocumentPortal.Models.Public;

public class DocumentSearchModel
{
    public string? Q { get; set; }
    public int? CategoryId { get; set; }
    public int? TypeId { get; set; }
    public int? IssuerId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
