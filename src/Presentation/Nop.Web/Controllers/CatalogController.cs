using Microsoft.AspNetCore.Mvc;
using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Rss;
using Nop.Data;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CatalogController : BasePublicController
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly IBlogService _blogService;
    protected readonly ICatalogModelFactory _catalogModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductFavoriteService _productFavoriteService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly MediaSettings _mediaSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public CatalogController(CatalogSettings catalogSettings,
        IAclService aclService,
        IBlogService blogService,
        ICatalogModelFactory catalogModelFactory,
        ICategoryService categoryService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        INopDataProvider dataProvider,
        IFilterLevelValueModelFactory filterLevelValueModelFactory,
        IFilterLevelValueService filterLevelValueService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        IProductFavoriteService productFavoriteService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IProductTagService productTagService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWebHelper webHelper,
        IWorkContext workContext,
        FilterLevelSettings filterLevelSettings,
        MediaSettings mediaSettings,
        VendorSettings vendorSettings)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _blogService = blogService;
        _catalogModelFactory = catalogModelFactory;
        _categoryService = categoryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dataProvider = dataProvider;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
        _filterLevelValueService = filterLevelValueService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _manufacturerService = manufacturerService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _productFavoriteService = productFavoriteService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _productTagService = productTagService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _workContext = workContext;
        _filterLevelSettings = filterLevelSettings;
        _mediaSettings = mediaSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Categories

    public virtual async Task<IActionResult> Category(int categoryId, CatalogProductsCommand command)
    {
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

        if (!await CheckCategoryAvailabilityAsync(category))
            return InvokeHttp404();

        var store = await _storeContext.GetCurrentStoreAsync();

        //'Continue shopping' URL
        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
            NopCustomerDefaults.LastContinueShoppingPageAttribute,
            _webHelper.GetThisPageUrl(false),
            store.Id);

        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.CATEGORIES_VIEW))
            DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.ADMIN }));

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.ViewCategory",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

        //model
        var model = await _catalogModelFactory.PrepareCategoryModelAsync(category, command);

        //template
        var templateViewPath = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(category.CategoryTemplateId);
        return View(templateViewPath, model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
    {
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

        if (!await CheckCategoryAvailabilityAsync(category))
            return NotFound();

        var model = await _catalogModelFactory.PrepareCategoryProductsModelAsync(category, command);

        return PartialView("_ProductsInGridOrLines", model);
    }

    #endregion

    #region Manufacturers

    public virtual async Task<IActionResult> Manufacturer(int manufacturerId, CatalogProductsCommand command)
    {
        var vendor = await ResolveVendorFromManufacturerAsync(manufacturerId);
        if (!await CheckVendorAvailabilityAsync(vendor))
            return InvokeHttp404();

        var vendorUrl = await _nopUrlHelper.RouteGenericUrlAsync(vendor);
        return RedirectPermanent(vendorUrl);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetManufacturerProducts(int manufacturerId, CatalogProductsCommand command)
    {
        var vendor = await ResolveVendorFromManufacturerAsync(manufacturerId);
        if (!await CheckVendorAvailabilityAsync(vendor))
            return NotFound();

        var model = await _catalogModelFactory.PrepareVendorProductsModelAsync(vendor, command);
        return PartialView("_ProductsInGridOrLines", model);
    }

    public virtual IActionResult ManufacturerAll()
        => RedirectToAction(nameof(VendorAll));

    public virtual async Task<IActionResult> CategoryAll(CatalogProductsCommand command)
    {
        command ??= new CatalogProductsCommand();

        var pageNumber = command.PageNumber > 0 ? command.PageNumber : 1;
        var pageSize = command.PageSize > 0
            ? command.PageSize
            : (_catalogSettings.DefaultCategoryPageSize > 0 ? _catalogSettings.DefaultCategoryPageSize : 16);

        var orderBy = command.OrderBy.HasValue && Enum.IsDefined(typeof(ProductSortingEnum), command.OrderBy.Value)
            ? (ProductSortingEnum)command.OrderBy.Value
            : ProductSortingEnum.Position;

        var customer = await _workContext.GetCurrentCustomerAsync();
        var canFilterByFavorites = customer != null && !await _customerService.IsGuestAsync(customer);
        var favoritesOnly = canFilterByFavorites && command.FavoritesOnly;
        var favoriteProductIds = favoritesOnly
            ? await _productFavoriteService.GetFavoriteProductIdsAsync(customer.Id)
            : null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var products = await _productService.SearchProductsAsync(
            pageNumber - 1,
            pageSize,
            storeId: store.Id,
            visibleIndividuallyOnly: true,
            productIds: favoriteProductIds,
            manufacturerIds: command.Ms,
            orderBy: orderBy);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = false,
            OrderBy = (int)orderBy,
            ViewMode = string.Equals(command.ViewMode, "list", StringComparison.OrdinalIgnoreCase) ? "list" : "grid",
            NoResultMessage = favoritesOnly
                ? "Tai khoan nay chua co san pham yeu thich."
                : await _localizationService.GetResourceAsync("Categories.NoProducts"),
            CanFilterByFavorites = canFilterByFavorites,
            FavoritesOnly = favoritesOnly
        };

        model.LoadPagedList(products);
        model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

        return View(model);
    }

    #endregion

    #region Vendors

    public virtual async Task<IActionResult> Vendor(int vendorId, CatalogProductsCommand command)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

        if (!await CheckVendorAvailabilityAsync(vendor))
            return InvokeHttp404();

        var store = await _storeContext.GetCurrentStoreAsync();

        //'Continue shopping' URL
        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
            NopCustomerDefaults.LastContinueShoppingPageAttribute,
            _webHelper.GetThisPageUrl(false),
            store.Id);

        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_VIEW))
            DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.ADMIN }));

        //model
        var model = await _catalogModelFactory.PrepareVendorModelAsync(vendor, command);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> UpdateUnitContactCustomer(int vendorId, int contactCustomerId)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
        if (!await CheckVendorAvailabilityAsync(vendor))
            return Json(new { success = false, message = "Không tìm thấy đơn vị." });

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        if (!vendor.PmCustomerId.HasValue || vendor.PmCustomerId.Value != currentCustomer.Id)
            return Json(new { success = false, message = "Bạn không có quyền cập nhật phụ trách liên hệ của đơn vị này." });

        if (contactCustomerId <= 0)
        {
            vendor.ContactCustomerId = null;
            await _vendorService.UpdateVendorAsync(vendor);
            return Json(new { success = true, message = "Đã bỏ phụ trách liên hệ." });
        }

        var contactCustomer = await _customerService.GetCustomerByIdAsync(contactCustomerId);
        if (contactCustomer == null || !contactCustomer.Active || contactCustomer.Deleted || await _customerService.IsGuestAsync(contactCustomer))
            return Json(new { success = false, message = "Tài khoản phụ trách không hợp lệ." });

        vendor.ContactCustomerId = contactCustomer.Id;
        await _vendorService.UpdateVendorAsync(vendor);

        return Json(new
        {
            success = true,
            message = "Đã cập nhật phụ trách liên hệ.",
            contactCustomerName = await _customerService.FormatUsernameAsync(contactCustomer)
        });
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetVendorProducts(int vendorId, CatalogProductsCommand command)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

        if (!await CheckVendorAvailabilityAsync(vendor))
            return NotFound();

        var model = await _catalogModelFactory.PrepareVendorProductsModelAsync(vendor, command);

        return PartialView("_ProductsInGridOrLines", model);
    }

    public virtual async Task<IActionResult> VendorReviews(int vendorId, VendorReviewsPagingFilteringModel pagingModel)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

        if (!await CheckVendorAvailabilityAsync(vendor))
            return NotFound();

        var model = await _catalogModelFactory.PrepareVendorProductReviewsModelAsync(vendor, pagingModel);

        return View(model);
    }

    public virtual async Task<IActionResult> VendorAll()
    {
        //we don't allow viewing of vendors if "vendors" block is hidden
        if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();
        return View(model);
    }

    #endregion

    #region Product tags

    public virtual async Task<IActionResult> ProductsByTag(int productTagId, CatalogProductsCommand command)
    {
        var productTag = await _productTagService.GetProductTagByIdAsync(productTagId);
        if (productTag == null)
            return InvokeHttp404();

        var model = await _catalogModelFactory.PrepareProductsByTagModelAsync(productTag, command);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetTagProducts(int tagId, CatalogProductsCommand command)
    {
        var productTag = await _productTagService.GetProductTagByIdAsync(tagId);
        if (productTag == null)
            return NotFound();

        var model = await _catalogModelFactory.PrepareTagProductsModelAsync(productTag, command);

        return PartialView("_ProductsInGridOrLines", model);
    }

    public virtual async Task<IActionResult> ProductTagsAll()
    {
        var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();

        return View(model);
    }

    #endregion

    #region New (recently added) products page

    public virtual async Task<IActionResult> NewProducts(CatalogProductsCommand command)
    {
        if (!_catalogSettings.NewProductsEnabled)
            return InvokeHttp404();

        var model = new NewProductsModel
        {
            CatalogProductsModel = await _catalogModelFactory.PrepareNewProductsModelAsync(command)
        };

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetNewProducts(CatalogProductsCommand command)
    {
        if (!_catalogSettings.NewProductsEnabled)
            return NotFound();

        var model = await _catalogModelFactory.PrepareNewProductsModelAsync(command);

        return PartialView("_ProductsInGridOrLines", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> NewProductsRss()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var feed = new RssFeed(
            $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: New products",
            "Information about products",
            new Uri(_webHelper.GetStoreLocation()),
            DateTime.UtcNow);

        if (!_catalogSettings.NewProductsEnabled)
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

        var items = new List<RssItem>();

        var storeId = store.Id;
        var products = await _productService.GetProductsMarkedAsNewAsync(storeId: storeId);

        foreach (var product in products)
        {
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol());
            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
            var productDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription);
            var item = new RssItem(productName, productDescription, new Uri(productUrl), $"urn:store:{store.Id}:newProducts:product:{product.Id}", product.CreatedOnUtc);
            items.Add(item);
            //uncomment below if you want to add RSS enclosure for pictures
            //var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
            //if (picture != null)
            //{
            //    var imageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductDetailsPictureSize);
            //    item.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", imageUrl), new XAttribute("length", picture.PictureBinary.Length)));
            //}

        }
        feed.Items = items;
        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    #endregion

    #region Searching

    public virtual async Task<IActionResult> Search(SearchModel model, CatalogProductsCommand command)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var isGuest = await _customerService.IsGuestAsync(currentCustomer);

        //'Continue shopping' URL
        await _genericAttributeService.SaveAttributeAsync(currentCustomer,
            NopCustomerDefaults.LastContinueShoppingPageAttribute,
            _webHelper.GetThisPageUrl(true),
            store.Id);

        model ??= new SearchModel();

        model.q = model.q?.Trim();
        model.Type = model.Type?.Trim().ToLowerInvariant();

        if (model.Type is not (null or "products" or "units" or "news" or "documents"))
            model.Type = null;

        model.ProductPage = Math.Max(1, model.ProductPage);
        model.UnitPage = Math.Max(1, model.UnitPage);
        model.BlogPage = Math.Max(1, model.BlogPage);
        model.DocumentPage = Math.Max(1, model.DocumentPage);

        model.ProductPageSize = 4;
        model.UnitPageSize = 4;
        model.BlogPageSize = 5;
        model.DocumentPageSize = 5;

        if (model.HasQuery)
        {
            if (model.ShowProductsSection)
            {
                var products = await _productService.SearchProductsAsync(
                    pageIndex: model.ProductPage - 1,
                    pageSize: model.ProductPageSize,
                    storeId: store.Id,
                    keywords: model.q,
                    languageId: language.Id,
                    visibleIndividuallyOnly: true);

                model.ProductTotalCount = products.TotalCount;
                model.ProductResults = (await _productModelFactory.PrepareProductOverviewModelsAsync(
                    products,
                    preparePriceModel: true,
                    preparePictureModel: true,
                    productThumbPictureSize: _mediaSettings.ProductThumbPictureSize)).ToList();
            }

            if (model.ShowUnitsSection)
            {
                var units = await _vendorService.GetAllVendorsAsync(
                    name: model.q,
                    pageIndex: model.UnitPage - 1,
                    pageSize: model.UnitPageSize,
                    showHidden: false);

                model.UnitTotalCount = units.TotalCount;
                model.UnitResults = new List<SearchModel.UnitSearchResultModel>();

                foreach (var unit in units)
                {
                    var unitSeName = await _urlRecordService.GetSeNameAsync(unit);
                    model.UnitResults.Add(new SearchModel.UnitSearchResultModel
                    {
                        Id = unit.Id,
                        Name = unit.Name,
                        SeName = unitSeName,
                        Url = await _nopUrlHelper.RouteGenericUrlAsync(unit)
                    });
                }
            }

            if (model.ShowBlogsSection)
            {
                var blogPosts = await _blogService.GetAllBlogPostsAsync(
                    storeId: store.Id,
                    languageId: language.Id,
                    pageIndex: model.BlogPage - 1,
                    pageSize: model.BlogPageSize,
                    keywords: model.q);

                model.BlogTotalCount = blogPosts.TotalCount;
                model.BlogResults = new List<SearchModel.BlogPostSearchResultModel>();

                foreach (var blogPost in blogPosts)
                {
                    var seName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
                    var routeName = blogPost.PostTypeId == 1 ? NopRouteNames.Standard.BLOG_DOCUMENT_POST : NopRouteNames.Standard.BLOG_NEWS_POST;
                    var blogUrl = Url.RouteUrl(routeName, new { SeName = seName })
                                  ?? await _nopUrlHelper.RouteGenericUrlAsync(blogPost, languageId: blogPost.LanguageId, ensureTwoPublishedLanguages: false);

                    model.BlogResults.Add(new SearchModel.BlogPostSearchResultModel
                    {
                        Id = blogPost.Id,
                        Title = blogPost.Title,
                        SeName = seName,
                        Url = blogUrl,
                        CreatedOnUtc = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc,
                        PostTypeId = blogPost.PostTypeId
                    });
                }
            }

            if (model.ShowDocumentsSection)
            {
                var (items, totalCount) = await SearchDocumentsAsync(model.q, model.DocumentPage - 1, model.DocumentPageSize, isGuest);
                model.DocumentResults = items;
                model.DocumentTotalCount = totalCount;
            }
        }

        var blogTags = await _blogService.GetAllBlogPostTagsAsync(store.Id, language.Id);
        model.TopBlogTags = blogTags
            .OrderByDescending(tag => tag.BlogPostCount)
            .ThenBy(tag => tag.Name)
            .Take(10)
            .Select(tag => tag.Name)
            .ToList();

        model.TopDocumentKeywords = await GetTopDocumentKeywordsAsync(10, isGuest);

        return View(model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> SearchTermAutoComplete(string term, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Content("");

        term = term.Trim();

        if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
            return Content("");

        //products
        var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
            _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;
        var store = await _storeContext.GetCurrentStoreAsync();

        var categoryIds = new List<int>();
        if (categoryId > 0)
            categoryIds.AddRange([categoryId, .. await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id)]);

        var products = await _productService.SearchProductsAsync(0,
            categoryIds: categoryIds,
            storeId: store.Id,
            keywords: term,
            languageId: (await _workContext.GetWorkingLanguageAsync()).Id,
            visibleIndividuallyOnly: true,
            pageSize: productNumber);

        var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (products.TotalCount > productNumber);

        var models = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize)).ToList();
        var result = (from p in models
                select new
                {
                    label = p.Name,
                    producturl = _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = p.SeName }).Result,
                    productpictureurl = p.PictureModels.FirstOrDefault()?.ImageUrl,
                    showlinktoresultsearch = showLinkToResultSearch
                })
            .ToList();
        return Json(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SearchProducts(SearchModel searchModel, CatalogProductsCommand command)
    {
        if (searchModel == null)
            searchModel = new SearchModel();

        var model = await _catalogModelFactory.PrepareSearchProductsModelAsync(searchModel, command);

        return PartialView("_ProductsInGridOrLines", model);
    }

    #endregion

    #region Filter level values

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetFilterLevelValues(string filterLevel1Value = "", string filterLevel2Value = "", string filterLevel3Value = "")
    {
        var values = await _filterLevelValueService.GetAllFilterLevelValuesAsync(
            filterLevel1Value, filterLevel2Value, filterLevel3Value);

        var defaultItemText = await _localizationService.GetResourceAsync("Admin.Common.Select");

        if (string.IsNullOrEmpty(filterLevel1Value))
        {
            var result = values
                .Select(f => new 
                { 
                    filterLevel1Value = f.FilterLevel1Value, 
                    defaultItemText = defaultItemText 
                })
                .Distinct();
            return Json(result);
        }

        if (string.IsNullOrEmpty(filterLevel2Value))
        {
            var result = values
                .Where(f => f.FilterLevel1Value == filterLevel1Value)
                .Select(f => new
                {
                    filterLevel1Value = f.FilterLevel1Value,
                    filterLevel2Value = f.FilterLevel2Value,
                    defaultItemText = defaultItemText
                })
                .Distinct();
            return Json(result);
        }

        if (string.IsNullOrEmpty(filterLevel3Value))
        {
            var result = values
                .Where(f => f.FilterLevel1Value == filterLevel1Value &&
                            f.FilterLevel2Value == filterLevel2Value)
                .Select(f => new
                {
                    filterLevel1Value = f.FilterLevel1Value,
                    filterLevel2Value = f.FilterLevel2Value,
                    filterLevel3Value = f.FilterLevel3Value,
                    defaultItemText = defaultItemText
                })
                .Distinct();
            return Json(result);
        }

        var finalResult = values
            .Where(f => f.FilterLevel1Value == filterLevel1Value &&
                        f.FilterLevel2Value == filterLevel2Value &&
                        f.FilterLevel3Value == filterLevel3Value)
            .Select(f => new
            {
                filterLevel1Value = f.FilterLevel1Value,
                filterLevel2Value = f.FilterLevel2Value,
                filterLevel3Value = f.FilterLevel3Value,
                defaultItemText = defaultItemText
            })
            .Distinct();

        return Json(finalResult);
    }

    public virtual async Task<IActionResult> SearchByFilterLevelValues(SearchFilterLevelValueModel model, CatalogProductsCommand command)
    {
        if (!_filterLevelSettings.FilterLevelEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var store = await _storeContext.GetCurrentStoreAsync();

        //'Continue shopping' URL
        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
            NopCustomerDefaults.LastContinueShoppingPageAttribute,
            _webHelper.GetThisPageUrl(true),
            store.Id);

        if (model == null)
            model = new SearchFilterLevelValueModel();

        model = await _filterLevelValueModelFactory.PrepareSearchFilterLevelValueModelAsync(model, command);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SearchProductsByFilterLevelValues(SearchFilterLevelValueModel searchModel, CatalogProductsCommand command)
    {
        if (searchModel == null)
            searchModel = new SearchFilterLevelValueModel();

        var model = await _catalogModelFactory.PrepareSearchProductsByFilterLevelValuesModelAsync(searchModel, command);

        return PartialView("_ProductsInGridOrLines", model);
    }

    #endregion

    #region Utilities

    protected virtual async Task<(IList<SearchModel.DocumentSearchResultModel> items, int totalCount)> SearchDocumentsAsync(string keywords, int pageIndex, int pageSize, bool isGuest)
    {
        try
        {
            var normalizedKeywords = keywords?.Trim() ?? string.Empty;
            var likeKeywords = $"%{normalizedKeywords}%";
            var offset = Math.Max(0, pageIndex) * Math.Max(1, pageSize);
            const int publicAccessScopeId = 5;

            var totalCountResult = await _dataProvider.QueryAsync<DocumentCountResult>(
                @"SELECT COUNT(1) AS TotalCount
                  FROM [Document]
                  WHERE [Published] = 1
                    AND [Deleted] = 0
                    AND (@IsGuest = 0 OR [AccessScopeId] = @PublicAccessScopeId)
                    AND (@Keywords = ''
                         OR ([Code] IS NOT NULL AND [Code] LIKE @LikeKeywords)
                         OR [Title] LIKE @LikeKeywords
                         OR ([Keywords] IS NOT NULL AND [Keywords] LIKE @LikeKeywords)
                         OR ([Summary] IS NOT NULL AND [Summary] LIKE @LikeKeywords))",
                new DataParameter("IsGuest", isGuest ? 1 : 0),
                new DataParameter("PublicAccessScopeId", publicAccessScopeId),
                new DataParameter("Keywords", normalizedKeywords),
                new DataParameter("LikeKeywords", likeKeywords));

            var totalCount = totalCountResult.FirstOrDefault()?.TotalCount ?? 0;

            var rows = await _dataProvider.QueryAsync<DocumentSearchRow>(
                @"SELECT [Id], [Title], [Slug], [Code], [Summary], [IssuedDate]
                  FROM [Document]
                  WHERE [Published] = 1
                    AND [Deleted] = 0
                    AND (@IsGuest = 0 OR [AccessScopeId] = @PublicAccessScopeId)
                    AND (@Keywords = ''
                         OR ([Code] IS NOT NULL AND [Code] LIKE @LikeKeywords)
                         OR [Title] LIKE @LikeKeywords
                         OR ([Keywords] IS NOT NULL AND [Keywords] LIKE @LikeKeywords)
                         OR ([Summary] IS NOT NULL AND [Summary] LIKE @LikeKeywords))
                  ORDER BY
                    CASE WHEN @Keywords <> '' AND [Code] = @Keywords THEN 0 ELSE 1 END,
                    CASE WHEN @Keywords <> '' AND [Title] LIKE @Keywords + '%' THEN 0 ELSE 1 END,
                    [IssuedDate] DESC,
                    [DisplayOrder] ASC,
                    [Id] DESC
                  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
                new DataParameter("IsGuest", isGuest ? 1 : 0),
                new DataParameter("PublicAccessScopeId", publicAccessScopeId),
                new DataParameter("Keywords", normalizedKeywords),
                new DataParameter("LikeKeywords", likeKeywords),
                new DataParameter("Offset", offset),
                new DataParameter("PageSize", Math.Max(1, pageSize)));

            var items = rows.Select(row => new SearchModel.DocumentSearchResultModel
            {
                Id = row.Id,
                Title = row.Title,
                Slug = row.Slug,
                Code = row.Code,
                Summary = row.Summary,
                IssuedDate = row.IssuedDate
            }).ToList();

            return (items, totalCount);
        }
        catch
        {
            return (new List<SearchModel.DocumentSearchResultModel>(), 0);
        }
    }

    protected virtual async Task<IList<string>> GetTopDocumentKeywordsAsync(int limit, bool isGuest)
    {
        try
        {
            const int publicAccessScopeId = 5;
            var rows = await _dataProvider.QueryAsync<DocumentKeywordRow>(
                @"SELECT [Keywords]
                  FROM [Document]
                  WHERE [Published] = 1
                    AND [Deleted] = 0
                    AND (@IsGuest = 0 OR [AccessScopeId] = @PublicAccessScopeId)
                    AND [Keywords] IS NOT NULL
                    AND LTRIM(RTRIM([Keywords])) <> ''",
                new DataParameter("IsGuest", isGuest ? 1 : 0),
                new DataParameter("PublicAccessScopeId", publicAccessScopeId));

            var frequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var separators = new[] { ',', ';', '|' };

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.Keywords))
                    continue;

                var tokens = row.Keywords.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var token in tokens)
                {
                    if (string.IsNullOrWhiteSpace(token))
                        continue;

                    frequency[token] = frequency.GetValueOrDefault(token) + 1;
                }
            }

            return frequency
                .OrderByDescending(entry => entry.Value)
                .ThenBy(entry => entry.Key)
                .Take(limit)
                .Select(entry => entry.Key)
                .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    protected sealed class DocumentSearchRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Code { get; set; }
        public string Summary { get; set; }
        public DateTime? IssuedDate { get; set; }
    }

    protected sealed class DocumentCountResult
    {
        public int TotalCount { get; set; }
    }

    protected sealed class DocumentKeywordRow
    {
        public string Keywords { get; set; }
    }

    protected virtual async Task<Vendor> ResolveVendorFromManufacturerAsync(int manufacturerId)
    {
        if (manufacturerId <= 0)
            return null;

        var vendor = await _vendorService.GetVendorByIdAsync(manufacturerId);
        if (vendor != null && !vendor.Deleted && vendor.Active)
            return vendor;

        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);
        if (manufacturer == null || manufacturer.Deleted)
            return null;

        var candidates = await _vendorService.GetAllVendorsAsync(name: manufacturer.Name, showHidden: true, pageSize: 1);
        return candidates.FirstOrDefault();
    }

    protected virtual async Task<bool> CheckCategoryAvailabilityAsync(Category category)
    {
        if (category is null)
            return false;

        var isAvailable = true;

        if (category.Deleted)
            isAvailable = false;

        var notAvailable =
            //published?
            !category.Published ||
            //ACL (access control list) 
            !await _aclService.AuthorizeAsync(category) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(category);
        //Check whether the current user has a "Manage categories" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.CATEGORIES_VIEW);
        if (notAvailable && !hasAdminAccess)
            isAvailable = false;

        return isAvailable;
    }

    protected virtual async Task<bool> CheckManufacturerAvailabilityAsync(Manufacturer manufacturer)
    {
        if (manufacturer == null)
            return false;

        var isAvailable = true;

        if (manufacturer.Deleted)
            isAvailable = false;

        var notAvailable =
            //published?
            !manufacturer.Published ||
            //ACL (access control list) 
            !await _aclService.AuthorizeAsync(manufacturer) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(manufacturer);
        //Check whether the current user has a "Manage categories" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Catalog.MANUFACTURER_VIEW);
        if (notAvailable && !hasAdminAccess)
            isAvailable = false;

        return isAvailable;
    }

    protected virtual Task<bool> CheckVendorAvailabilityAsync(Vendor vendor)
    {
        var isAvailable = true;

        if (vendor == null || vendor.Deleted || !vendor.Active)
            isAvailable = false;

        return Task.FromResult(isAvailable);
    }

    #endregion
}
