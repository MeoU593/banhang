using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogCommentModel : BaseNopEntityModel
{
    public int ReplyToCommentId { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; }

    public string CustomerAvatarUrl { get; set; }

    public string Rank { get; set; }

    public string UnitName { get; set; }

    public string PositionTitle { get; set; }

    public string CommentText { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool AllowViewingProfiles { get; set; }
}
