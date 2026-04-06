using Nop.Web.Framework.UI.Paging;

namespace Nop.Plugin.Misc.News.Public.Models;

/// <summary>
/// Represents a model to get news items
/// </summary>
public record NewsPagingFilteringModel : BasePageableModel
{
    /// <summary>
    /// Gets or sets the news item type filter (-1 = all, 0 = Tin tức, 1 = Công văn)
    /// </summary>
    public int Type { get; set; } = -1;
}