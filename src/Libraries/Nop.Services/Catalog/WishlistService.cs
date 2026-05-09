using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Wishlist service
/// </summary>
public partial class WishlistService : IWishlistService
{
    protected readonly IRepository<WishlistItem> _wishlistItemRepository;

    public WishlistService(IRepository<WishlistItem> wishlistItemRepository)
    {
        _wishlistItemRepository = wishlistItemRepository;
    }

    /// <summary>
    /// Gets wishlist items by customer
    /// </summary>
    public virtual async Task<IList<WishlistItem>> GetWishlistAsync(int customerId, int storeId = 0)
    {
        if (customerId <= 0)
            return new List<WishlistItem>();

        var query = _wishlistItemRepository.Table.Where(item => item.CustomerId == customerId);

        if (storeId > 0)
            query = query.Where(item => item.StoreId == storeId);

        return await query
            .OrderByDescending(item => item.UpdatedOnUtc)
            .ThenByDescending(item => item.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Gets wishlist product identifiers by customer
    /// </summary>
    public virtual async Task<IList<int>> GetWishlistProductIdsAsync(int customerId)
    {
        if (customerId <= 0)
            return new List<int>();

        return await _wishlistItemRepository.Table
            .Where(item => item.CustomerId == customerId)
            .Select(item => item.ProductId)
            .Distinct()
            .ToListAsync();
    }

    /// <summary>
    /// Gets wishlist state map for selected products
    /// </summary>
    public virtual async Task<IDictionary<int, bool>> GetWishlistStatesAsync(int customerId, IList<int> productIds)
    {
        var result = new Dictionary<int, bool>();

        if (customerId <= 0 || productIds is null || !productIds.Any())
            return result;

        var normalizedIds = productIds.Where(id => id > 0).Distinct().ToList();
        if (!normalizedIds.Any())
            return result;

        var wishlistProductIds = await _wishlistItemRepository.Table
            .Where(item => item.CustomerId == customerId && normalizedIds.Contains(item.ProductId))
            .Select(item => item.ProductId)
            .Distinct()
            .ToListAsync();

        foreach (var productId in normalizedIds)
            result[productId] = wishlistProductIds.Contains(productId);

        return result;
    }

    /// <summary>
    /// Adds a product to wishlist
    /// </summary>
    public virtual async Task<WishlistItem> AddToWishlistAsync(int customerId, int productId, int storeId, int quantity = 1, string attributesXml = null)
    {
        if (customerId <= 0 || productId <= 0)
            return null;

        quantity = Math.Max(quantity, 1);
        attributesXml ??= string.Empty;

        var item = await _wishlistItemRepository.Table
            .Where(wishlistItem => wishlistItem.CustomerId == customerId &&
                wishlistItem.ProductId == productId &&
                wishlistItem.AttributesXml == attributesXml)
            .OrderByDescending(wishlistItem => wishlistItem.Id)
            .FirstOrDefaultAsync();

        if (item != null)
        {
            item.Quantity += quantity;
            item.StoreId = storeId;
            item.UpdatedOnUtc = DateTime.UtcNow;
            await _wishlistItemRepository.UpdateAsync(item);
            return item;
        }

        item = new WishlistItem
        {
            CustomerId = customerId,
            ProductId = productId,
            StoreId = storeId,
            Quantity = quantity,
            AttributesXml = attributesXml,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        await _wishlistItemRepository.InsertAsync(item);
        return item;
    }

    /// <summary>
    /// Deletes a wishlist item
    /// </summary>
    public virtual async Task DeleteWishlistItemAsync(int id, int customerId)
    {
        if (id <= 0 || customerId <= 0)
            return;

        var item = await _wishlistItemRepository.Table
            .FirstOrDefaultAsync(wishlistItem => wishlistItem.Id == id && wishlistItem.CustomerId == customerId);

        if (item != null)
            await _wishlistItemRepository.DeleteAsync(item);
    }

    /// <summary>
    /// Deletes a product from wishlist
    /// </summary>
    public virtual async Task DeleteWishlistProductAsync(int customerId, int productId)
    {
        if (customerId <= 0 || productId <= 0)
            return;

        var items = await _wishlistItemRepository.Table
            .Where(item => item.CustomerId == customerId && item.ProductId == productId)
            .ToListAsync();

        if (items.Any())
            await _wishlistItemRepository.DeleteAsync(items);
    }

    /// <summary>
    /// Clears customer wishlist
    /// </summary>
    public virtual async Task ClearWishlistAsync(int customerId)
    {
        if (customerId <= 0)
            return;

        var items = await _wishlistItemRepository.Table
            .Where(item => item.CustomerId == customerId)
            .ToListAsync();

        if (items.Any())
            await _wishlistItemRepository.DeleteAsync(items);
    }

    /// <summary>
    /// Toggles product wishlist state
    /// </summary>
    public virtual async Task<bool> ToggleWishlistAsync(int customerId, int productId, int storeId = 0)
    {
        if (customerId <= 0 || productId <= 0)
            return false;

        var item = await _wishlistItemRepository.Table
            .Where(wishlistItem => wishlistItem.CustomerId == customerId && wishlistItem.ProductId == productId)
            .OrderByDescending(wishlistItem => wishlistItem.Id)
            .FirstOrDefaultAsync();

        if (item != null)
        {
            await _wishlistItemRepository.DeleteAsync(item);
            return false;
        }

        await AddToWishlistAsync(customerId, productId, storeId);
        return true;
    }
}
