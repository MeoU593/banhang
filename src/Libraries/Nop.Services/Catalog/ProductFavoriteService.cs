using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Product favorite service
/// </summary>
public partial class ProductFavoriteService : IProductFavoriteService
{
    protected readonly IRepository<ProductFavorite> _productFavoriteRepository;

    public ProductFavoriteService(IRepository<ProductFavorite> productFavoriteRepository)
    {
        _productFavoriteRepository = productFavoriteRepository;
    }

    /// <summary>
    /// Gets the product identifiers marked as favorite by customer
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product identifiers
    /// </returns>
    public virtual async Task<IList<int>> GetFavoriteProductIdsAsync(int customerId)
    {
        if (customerId <= 0)
            return new List<int>();

        return await _productFavoriteRepository.Table
            .Where(pf => pf.CustomerId == customerId)
            .Select(pf => pf.ProductId)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>
    /// Gets the favorite state map for selected products
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the favorite state map by product id
    /// </returns>
    public virtual async Task<IDictionary<int, bool>> GetFavoriteStatesAsync(int customerId, IList<int> productIds)
    {
        var result = new Dictionary<int, bool>();

        if (customerId <= 0 || productIds is null || !productIds.Any())
            return result;

        var normalizedIds = productIds.Where(id => id > 0).Distinct().ToList();
        if (!normalizedIds.Any())
            return result;

        var favoriteProductIds = await _productFavoriteRepository.Table
            .Where(pf => pf.CustomerId == customerId && normalizedIds.Contains(pf.ProductId))
            .Select(pf => pf.ProductId)
            .Distinct()
            .ToListAsync();

        foreach (var productId in normalizedIds)
            result[productId] = favoriteProductIds.Contains(productId);

        return result;
    }

    /// <summary>
    /// Toggles favorite state for product and customer
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the current favorite state after toggle
    /// </returns>
    public virtual async Task<bool> ToggleFavoriteAsync(int customerId, int productId)
    {
        if (customerId <= 0 || productId <= 0)
            return false;

        var record = await _productFavoriteRepository.Table
            .Where(pf => pf.CustomerId == customerId && pf.ProductId == productId)
            .OrderByDescending(pf => pf.Id)
            .FirstOrDefaultAsync();

        if (record != null)
        {
            await _productFavoriteRepository.DeleteAsync(record);
            return false;
        }

        await _productFavoriteRepository.InsertAsync(new ProductFavorite
        {
            CustomerId = customerId,
            ProductId = productId,
            CreatedOnUtc = DateTime.UtcNow
        });

        return true;
    }
}
