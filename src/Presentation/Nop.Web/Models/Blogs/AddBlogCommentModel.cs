using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Blogs;

public partial record AddBlogCommentModel : BaseNopEntityModel
{
    public int ReplyToCommentId { get; set; }

    [NopResourceDisplayName("Blog.Comments.CommentText")]
    public string CommentText { get; set; }

    public bool DisplayCaptcha { get; set; }
}
