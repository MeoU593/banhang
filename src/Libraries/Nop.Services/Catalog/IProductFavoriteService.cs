namespace Nop.Services.Catalog;

/// <summary>
/// Product favorite service interface
/// </summary>
public partial interface IProductFavoriteService
{
    /// <summary>
    /// Gets the product identifiers marked as favorite by customer
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product identifiers
    /// </returns>
    Task<IList<int>> GetFavoriteProductIdsAsync(int customerId);

    /// <summary>
    /// Gets the favorite state map for selected products
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the favorite state map by product id
    /// </returns>
    Task<IDictionary<int, bool>> GetFavoriteStatesAsync(int customerId, IList<int> productIds);

    /// <summary>
    /// Toggles favorite state for product and customer
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the current favorite state after toggle
    /// </returns>
    Task<bool> ToggleFavoriteAsync(int customerId, int productId);
}
