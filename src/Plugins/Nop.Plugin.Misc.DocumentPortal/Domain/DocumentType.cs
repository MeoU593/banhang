using Nop.Core;

namespace Nop.Plugin.Misc.DocumentPortal.Domain;

public class DocumentType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}
