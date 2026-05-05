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

    public async Task<IViewComponentResult> InvokeAsync(int? postTypeId = null, int take = 4, string displayVariant = null)
    {
        if (take <= 0)
            take = 4;

        var model = _blogSettings.Enabled
            ? await _blogModelFactory.PrepareBlogPostListModelAsync(new BlogPagingFilteringModel
            {
                PageNumber = 1,
                PageSize = Math.Max(take * 5, 20),
                PostTypeId = postTypeId
            })
            : new BlogPostListModel();

        var posts = model.BlogPosts.AsEnumerable();

        model.BlogPosts = posts
            .OrderByDescending(post => post.CreatedOn)
            .Take(take)
            .ToList();

        ViewData["HomepagePostTypeId"] = postTypeId;
        ViewData["HomepagePostDisplayVariant"] = displayVariant;

        return View(model);
    }
}
