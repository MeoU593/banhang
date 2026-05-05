namespace Nop.Plugin.Misc.DocumentPortal.Models.Public;

public class DocumentCommentModel
{
    public int Id { get; set; }

    public int ReplyToCommentId { get; set; }

    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerAvatarUrl { get; set; }

    public string? Rank { get; set; }

    public string? UnitName { get; set; }

    public string? PositionTitle { get; set; }

    public string CommentText { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public bool AllowViewingProfiles { get; set; }
}
