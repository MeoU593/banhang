using Nop.Core;

namespace Nop.Plugin.Misc.DocumentPortal.Domain;

public class DocumentComment : BaseEntity
{
    public int DocumentId { get; set; }

    public int CustomerId { get; set; }

    public string CommentText { get; set; } = string.Empty;

    public bool IsApproved { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}
