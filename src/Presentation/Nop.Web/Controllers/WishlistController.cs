using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers;

public partial class WishlistController : BasePublicController
{
    protected readonly ICustomerService _customerService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWishlistService _wishlistService;
    protected readonly IWorkContext _workContext;

    public WishlistController(ICustomerService customerService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreContext storeContext,
        IWishlistService wishlistService,
        IWorkContext workContext)
    {
        _customerService = customerService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _storeContext = storeContext;
        _wishlistService = wishlistService;
        _workContext = workContext;
    }

    [HttpGet]
    [Route("wishlist")]
    public virtual async Task<IActionResult> Index()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var store = await _storeContext.GetCurrentStoreAsync();
        var wishlistItems = await _wishlistService.GetWishlistAsync(customer.Id, store.Id);
        var products = await _productService.GetProductsByIdsAsync(wishlistItems.Select(item => item.ProductId).Distinct().ToArray());

        var model = new WishlistModel
        {
            Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [Route("wishlist/delete/{productId:int}")]
    public virtual async Task<IActionResult> Delete(int productId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsGuestAsync(customer))
            await _wishlistService.DeleteWishlistProductAsync(customer.Id, productId);

        return RedirectToAction(nameof(Index));
    }
}
