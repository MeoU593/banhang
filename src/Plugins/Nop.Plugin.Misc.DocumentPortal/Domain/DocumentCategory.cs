using Nop.Core;

namespace Nop.Plugin.Misc.DocumentPortal.Domain;

public class DocumentCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public string? Description { get; set; }
    public bool Published { get; set; }
    public int DisplayOrder { get; set; }
}
