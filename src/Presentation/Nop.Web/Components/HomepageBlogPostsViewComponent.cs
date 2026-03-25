using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Components;

public partial class HomepageBlogPostsViewComponent : NopViewComponent
{
    protected readonly BlogSettings _blogSettings;
    protected readonly IBlogModelFactory _blogModelFactory;

    public HomepageBlogPostsViewComponent(BlogSettings blogSettings,
        IBlogModelFactory blogModelFactory)
    {
        _blogSettings = blogSettings;
        _blogModelFactory = blogModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = _blogSettings.Enabled
            ? await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel
            {
                PageNumber = 1,
                PageSize = 4
            })
            : new BlogPostListModel();

        return View(model);
    }
}
