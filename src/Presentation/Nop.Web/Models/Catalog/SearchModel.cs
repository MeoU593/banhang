using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Models.Catalog;

public partial record SearchModel : BaseNopModel
{
    public SearchModel()
    {
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
        AvailableVendors = new List<SelectListItem>();
        CatalogProductsModel = new CatalogProductsModel();
        ProductResults = new List<ProductOverviewModel>();
        BlogResults = new List<BlogPostSearchResultModel>();
        DocumentResults = new List<DocumentSearchResultModel>();
        UnitResults = new List<UnitSearchResultModel>();
        TopBlogTags = new List<string>();
        TopDocumentKeywords = new List<string>();
    }

    /// <summary>
    /// Query string
    /// </summary>
    [NopResourceDisplayName("Search.SearchTerm")]
    public string q { get; set; }

    /// <summary>
    /// Category ID
    /// </summary>
    [NopResourceDisplayName("Search.Category")]
    public int cid { get; set; }

    [NopResourceDisplayName("Search.IncludeSubCategories")]
    public bool isc { get; set; }

    /// <summary>
    /// Manufacturer ID
    /// </summary>
    [NopResourceDisplayName("Search.Manufacturer")]
    public int mid { get; set; }

    /// <summary>
    /// Vendor ID
    /// </summary>
    [NopResourceDisplayName("Search.Vendor")]
    public int vid { get; set; }

    /// <summary>
    /// A value indicating whether to search in descriptions
    /// </summary>
    [NopResourceDisplayName("Search.SearchInDescriptions")]
    public bool sid { get; set; }

    /// <summary>
    /// A value indicating whether to search in product tags
    /// </summary>
    [NopResourceDisplayName("Search.SearchInTags")]
    public bool sit { get; set; }

    /// <summary>
    /// A value indicating whether "advanced search" is enabled
    /// </summary>
    [NopResourceDisplayName("Search.AdvancedSearch")]
    public bool advs { get; set; }

    /// <summary>
    /// A value indicating whether "allow search by vendor" is enabled
    /// </summary>
    public bool asv { get; set; }

    public CatalogProductsModel CatalogProductsModel { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }
    public IList<SelectListItem> AvailableManufacturers { get; set; }
    public IList<SelectListItem> AvailableVendors { get; set; }

    public string Type { get; set; }

    public int ProductPage { get; set; } = 1;
    public int UnitPage { get; set; } = 1;
    public int BlogPage { get; set; } = 1;
    public int DocumentPage { get; set; } = 1;

    public int ProductPageSize { get; set; } = 4;
    public int UnitPageSize { get; set; } = 4;
    public int BlogPageSize { get; set; } = 5;
    public int DocumentPageSize { get; set; } = 5;

    public int ProductTotalCount { get; set; }
    public int UnitTotalCount { get; set; }
    public int BlogTotalCount { get; set; }
    public int DocumentTotalCount { get; set; }

    public IList<ProductOverviewModel> ProductResults { get; set; }
    public IList<BlogPostSearchResultModel> BlogResults { get; set; }
    public IList<DocumentSearchResultModel> DocumentResults { get; set; }
    public IList<UnitSearchResultModel> UnitResults { get; set; }

    public IList<string> TopBlogTags { get; set; }
    public IList<string> TopDocumentKeywords { get; set; }

    public bool HasQuery => !string.IsNullOrWhiteSpace(q);

    public bool ShowAllSections => string.IsNullOrWhiteSpace(Type);
    public bool ShowProductsSection => ShowAllSections || string.Equals(Type, "products", StringComparison.OrdinalIgnoreCase);
    public bool ShowUnitsSection => ShowAllSections || string.Equals(Type, "units", StringComparison.OrdinalIgnoreCase);
    public bool ShowBlogsSection => ShowAllSections || string.Equals(Type, "news", StringComparison.OrdinalIgnoreCase);
    public bool ShowDocumentsSection => ShowAllSections || string.Equals(Type, "documents", StringComparison.OrdinalIgnoreCase);

    public int ProductTotalPages => ProductPageSize > 0 ? (int)Math.Ceiling(ProductTotalCount / (double)ProductPageSize) : 0;
    public int UnitTotalPages => UnitPageSize > 0 ? (int)Math.Ceiling(UnitTotalCount / (double)UnitPageSize) : 0;
    public int BlogTotalPages => BlogPageSize > 0 ? (int)Math.Ceiling(BlogTotalCount / (double)BlogPageSize) : 0;
    public int DocumentTotalPages => DocumentPageSize > 0 ? (int)Math.Ceiling(DocumentTotalCount / (double)DocumentPageSize) : 0;

    #region Nested classes

    public partial record CategoryModel : BaseNopEntityModel
    {
        public string Breadcrumb { get; set; }
    }

    public partial record BlogPostSearchResultModel : BaseNopEntityModel
    {
        public string Title { get; set; }
        public string SeName { get; set; }
        public string Url { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int PostTypeId { get; set; }
    }

    public partial record DocumentSearchResultModel : BaseNopEntityModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Code { get; set; }
        public string Summary { get; set; }
        public DateTime? IssuedDate { get; set; }
    }

    public partial record UnitSearchResultModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public string SeName { get; set; }
        public string Url { get; set; }
    }

    #endregion
}
