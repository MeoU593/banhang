using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog post search model
/// </summary>
public partial record BlogPostSearchModel : BaseSearchModel
{
    #region Ctor

    public BlogPostSearchModel()
    {
        AvailableStores = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchStore")]
    public int SearchStoreId { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public string SearchTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}
