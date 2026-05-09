using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;

/// <summary>
/// Wishlist service interface
/// </summary>
public partial interface IWishlistService
{
    /// <summary>
    /// Gets wishlist items by customer
    /// </summary>
    Task<IList<WishlistItem>> GetWishlistAsync(int customerId, int storeId = 0);

    /// <summary>
    /// Gets wishlist product identifiers by customer
    /// </summary>
    Task<IList<int>> GetWishlistProductIdsAsync(int customerId);

    /// <summary>
    /// Gets wishlist state map for selected products
    /// </summary>
    Task<IDictionary<int, bool>> GetWishlistStatesAsync(int customerId, IList<int> productIds);

    /// <summary>
    /// Adds a product to wishlist
    /// </summary>
    Task<WishlistItem> AddToWishlistAsync(int customerId, int productId, int storeId, int quantity = 1, string attributesXml = null);

    /// <summary>
    /// Deletes a wishlist item
    /// </summary>
    Task DeleteWishlistItemAsync(int id, int customerId);

    /// <summary>
    /// Deletes a product from wishlist
    /// </summary>
    Task DeleteWishlistProductAsync(int customerId, int productId);

    /// <summary>
    /// Clears customer wishlist
    /// </summary>
    Task ClearWishlistAsync(int customerId);

    /// <summary>
    /// Toggles product wishlist state
    /// </summary>
    Task<bool> ToggleWishlistAsync(int customerId, int productId, int storeId = 0);
}
