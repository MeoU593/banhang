using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.News.Public.Models;

/// <summary>
/// Represents the news item list model
/// </summary>
public record NewsItemListModel : BaseNopModel
{
    #region Properties

    public int WorkingLanguageId { get; set; }
    public int ActiveType { get; set; } = -1;
    public NewsPagingFilteringModel PagingFilteringContext { get; set; } = new();
    public List<NewsItemModel> NewsItems { get; set; } = [];

    #endregion
}