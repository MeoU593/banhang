using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a stock quantity history search model
/// </summary>
public partial record StockQuantityHistorySearchModel : BaseSearchModel
{
    public int ProductId { get; set; }
}
