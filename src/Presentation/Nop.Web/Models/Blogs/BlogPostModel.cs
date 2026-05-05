using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
    public BlogPostModel()
    {
        Tags = new List<string>();
        Comments = new List<BlogCommentModel>();
        LatestNewsPosts = new List<BlogPostModel>();
        PopularNewsPosts = new List<BlogPostModel>();
        AddNewComment = new AddBlogCommentModel();
    }

    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }

    public string Title { get; set; }
    public string Body { get; set; }
    public string BodyOverview { get; set; }
    public int PostTypeId { get; set; }
    public int ThumbnailPictureId { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public string AuthorName { get; set; }
    public int PdfDownloadId { get; set; }
    public int DocDownloadId { get; set; }
    public Guid? PdfDownloadGuid { get; set; }
    public Guid? DocDownloadGuid { get; set; }
    public string PdfDownloadUrl { get; set; }
    public string DocDownloadUrl { get; set; }
    public bool AllowComments { get; set; }
    public bool PreventNotRegisteredUsersToLeaveComments { get; set; }
    public int NumberOfComments { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedOn { get; set; }

    public IList<string> Tags { get; set; }

    public IList<BlogPostModel> LatestNewsPosts { get; set; }
    public IList<BlogPostModel> PopularNewsPosts { get; set; }

    public IList<BlogCommentModel> Comments { get; set; }
    public AddBlogCommentModel AddNewComment { get; set; }
}
