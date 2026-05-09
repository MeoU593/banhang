using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record WishlistModel : BaseNopModel
{
    public IList<ProductOverviewModel> Products { get; set; } = new List<ProductOverviewModel>();
}
