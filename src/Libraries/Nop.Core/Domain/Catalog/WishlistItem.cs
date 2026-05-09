namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a customer's wishlist item
/// </summary>
public partial class WishlistItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets selected product attributes in XML format
    /// </summary>
    public string AttributesXml { get; set; }

    /// <summary>
    /// Gets or sets the created date and time in UTC
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the updated date and time in UTC
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}
