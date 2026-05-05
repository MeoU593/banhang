using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Models.Blogs;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog;

public partial record VendorModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
    public VendorModel()
    {
        PictureModel = new PictureModel();
        CatalogProductsModel = new CatalogProductsModel();
        ProductReviews = new VendorProductReviewsListModel();
        ChildUnits = new List<UnitSummaryModel>();
        ContactCandidates = new List<UnitContactCandidateModel>();
        ActivityPosts = new List<BlogPostModel>();
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }
    public bool AllowCustomersToContactVendors { get; set; }
    public int? PmCustomerId { get; set; }
    public int? ContactCustomerId { get; set; }
    public string ContactCustomerName { get; set; }
    public bool CanManageContactCustomer { get; set; }
    public int NumberOfProducts { get; set; }
    public string Code { get; set; }
    public int? ParentId { get; set; }
    public string ParentName { get; set; }
    public int Level { get; set; }
    public string Path { get; set; }
    public int AddressId { get; set; }
    public string Email { get; set; }
    public bool Active { get; set; }
    public int DisplayOrder { get; set; }

    public PictureModel PictureModel { get; set; }

    public CatalogProductsModel CatalogProductsModel { get; set; }

    public VendorProductReviewsListModel ProductReviews { get; set; }

    public IList<UnitSummaryModel> ChildUnits { get; set; }

    public IList<UnitContactCandidateModel> ContactCandidates { get; set; }

    public IList<BlogPostModel> ActivityPosts { get; set; }

    public partial record UnitSummaryModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string SeName { get; set; }
        public int NumberOfProducts { get; set; }
        public PictureModel PictureModel { get; set; } = new();
    }

    public partial record UnitContactCandidateModel : BaseNopEntityModel
    {
        public string DisplayName { get; set; }
    }
}
