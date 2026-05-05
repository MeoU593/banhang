namespace Nop.Plugin.Misc.DocumentPortal.Models.Public;

public class AddDocumentCommentModel
{
    public int ReplyToCommentId { get; set; }

    public string? CommentText { get; set; }
}
