using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostListModel : BaseNopModel
{
    public BlogPostListModel()
    {
        PagingFilteringContext = new BlogPagingFilteringModel();
        BlogPosts = new List<BlogPostModel>();
        LatestDocumentPosts = new List<BlogPostModel>();
        AvailableVendors = new List<SelectListItem>();
        AvailableAuthors = new List<SelectListItem>();
    }

    public int WorkingLanguageId { get; set; }
    public int TotalNewsCount { get; set; }
    public int TotalDocumentCount { get; set; }
    public int NewsPostingUnitCount { get; set; }
    public int DocumentPostingUnitCount { get; set; }
    public BlogPagingFilteringModel PagingFilteringContext { get; set; }
    public IList<BlogPostModel> BlogPosts { get; set; }
    public IList<BlogPostModel> LatestDocumentPosts { get; set; }
    public IList<SelectListItem> AvailableVendors { get; set; }
    public IList<SelectListItem> AvailableAuthors { get; set; }
}
