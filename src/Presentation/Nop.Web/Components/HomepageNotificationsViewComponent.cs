using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Data;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Home;

namespace Nop.Web.Components;

public partial class HomepageNotificationsViewComponent : NopViewComponent
{
    private const int PublicDocumentAccessScopeId = 5;

    protected readonly IAclService _aclService;
    protected readonly IBlogService _blogService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IForumModelFactory _forumModelFactory;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IProductService _productService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

    protected const int HomepageNotificationTake = 3;

    public HomepageNotificationsViewComponent(
        IAclService aclService,
        IBlogService blogService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        INopDataProvider dataProvider,
        IForumModelFactory forumModelFactory,
        INopUrlHelper nopUrlHelper,
        IProductService productService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _aclService = aclService;
        _blogService = blogService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _dataProvider = dataProvider;
        _forumModelFactory = forumModelFactory;
        _nopUrlHelper = nopUrlHelper;
        _productService = productService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = new List<HomepageNotificationItemModel>
        {
            await PrepareProductNotificationAsync(),
            await PrepareNewsNotificationAsync(),
            await PrepareBlogDocumentNotificationAsync(),
            await PrepareForumNotificationAsync()
        };

        return View(items);
    }

    protected virtual async Task<HomepageNotificationItemModel> PrepareProductNotificationAsync()
    {
        var allProductsUrl = Url.RouteUrl(NopRouteNames.General.CATEGORIES) ?? "#";
        var products = await (await _productService.GetAllProductsDisplayedOnHomepageAsync())
            .WhereAwait(async product => await _aclService.AuthorizeAsync(product) && await _storeMappingService.AuthorizeAsync(product))
            .Where(product => _productService.ProductIsAvailable(product) && product.VisibleIndividually)
            .OrderByDescending(product => product.CreatedOnUtc)
            .ToListAsync();

        if (!products.Any())
            return CreateEmptyNotification("Sản phẩm", "restaurant", "text-amber-500", "Chưa có sản phẩm mới", allProductsUrl);

        var entries = new List<HomepageNotificationEntryModel>();
        foreach (var product in products.Take(HomepageNotificationTake))
        {
            entries.Add(new HomepageNotificationEntryModel
            {
                Badge = "Sản phẩm",
                DateText = await FormatDateAsync(product.CreatedOnUtc),
                Subtitle = await GetCustomerDisplayNameAsync(product.CreatedByCustomerId, "Hệ thống"),
                Title = product.Name,
                Url = await _nopUrlHelper.RouteGenericUrlAsync(product) ?? allProductsUrl
            });
        }

        return new HomepageNotificationItemModel
        {
            AreaTitle = "Sản phẩm mới",
            Badge = "Sản phẩm",
            EmptyText = "Chưa có sản phẩm mới",
            Icon = "restaurant",
            IconColorClass = "text-amber-500",
            Url = allProductsUrl,
            Entries = entries
        };
    }

    protected virtual Task<HomepageNotificationItemModel> PrepareNewsNotificationAsync()
    {
        return PrepareBlogTypeNotificationAsync(
            postTypeId: 0,
            areaTitle: "Tin tức mới",
            icon: "newsmode",
            iconColorClass: "text-blue-500",
            emptyText: "Chưa có tin tức mới",
            listRouteName: NopRouteNames.Standard.BLOG_NEWS,
            postRouteName: NopRouteNames.Standard.BLOG_NEWS_POST,
            badge: "Tin tức");
    }

    protected virtual Task<HomepageNotificationItemModel> PrepareBlogDocumentNotificationAsync()
    {
        return PrepareBlogTypeNotificationAsync(
            postTypeId: 1,
            areaTitle: "Văn bản mới",
            icon: "description",
            iconColorClass: "text-red-500",
            emptyText: "Chưa có văn bản mới",
            listRouteName: NopRouteNames.Standard.BLOG_DOCUMENTS,
            postRouteName: NopRouteNames.Standard.BLOG_DOCUMENT_POST,
            badge: "Văn bản");
    }

    protected virtual async Task<HomepageNotificationItemModel> PrepareBlogTypeNotificationAsync(
        int postTypeId,
        string areaTitle,
        string icon,
        string iconColorClass,
        string emptyText,
        string listRouteName,
        string postRouteName,
        string badge)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var listUrl = Url.RouteUrl(listRouteName) ?? Url.RouteUrl(NopRouteNames.General.BLOG) ?? "#";
        var posts = await _blogService.GetAllBlogPostsAsync(store.Id, language.Id, pageIndex: 0, pageSize: HomepageNotificationTake, postTypeId: postTypeId);

        if (!posts.Any())
            return CreateEmptyNotification(areaTitle, icon, iconColorClass, emptyText, listUrl);

        var entries = new List<HomepageNotificationEntryModel>();
        foreach (var post in posts.Take(HomepageNotificationTake))
        {
            var seName = await _urlRecordService.GetSeNameAsync(post, post.LanguageId, ensureTwoPublishedLanguages: false);
            var url = Url.RouteUrl(postRouteName, new { SeName = seName })
                ?? await _nopUrlHelper.RouteGenericUrlAsync(post, languageId: post.LanguageId, ensureTwoPublishedLanguages: false)
                ?? listUrl;

            entries.Add(new HomepageNotificationEntryModel
            {
                Badge = badge,
                DateText = await FormatDateAsync(post.StartDateUtc ?? post.CreatedOnUtc),
                Subtitle = string.IsNullOrWhiteSpace(post.AuthorName) ? "Ban biên tập" : post.AuthorName,
                Title = post.Title,
                Url = url
            });
        }

        return new HomepageNotificationItemModel
        {
            AreaTitle = areaTitle,
            Badge = badge,
            EmptyText = emptyText,
            Icon = icon,
            IconColorClass = iconColorClass,
            Url = listUrl,
            Entries = entries
        };
    }

    protected virtual async Task<HomepageNotificationItemModel> PrepareForumNotificationAsync()
    {
        var boardsUrl = Url.RouteUrl(NopRouteNames.General.BOARDS) ?? "#";
        var activeDiscussions = await _forumModelFactory.PrepareActiveDiscussionsModelAsync();

        if (!activeDiscussions.ForumTopics.Any())
            return CreateEmptyNotification("Diễn đàn", "forum", "text-purple-500", "Chưa có thảo luận mới", boardsUrl);

        var entries = activeDiscussions.ForumTopics
            .Take(HomepageNotificationTake)
            .Select(topic => new HomepageNotificationEntryModel
            {
                Badge = "Diễn đàn",
                DateText = $"{topic.NumReplies} phản hồi",
                Subtitle = string.IsNullOrWhiteSpace(topic.CustomerName) ? "Hoạt động diễn đàn" : topic.CustomerName,
                Title = topic.Subject,
                Url = Url.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG, new { id = topic.Id, slug = topic.SeName }) ?? boardsUrl
            })
            .ToList();

        return new HomepageNotificationItemModel
        {
            AreaTitle = "Hoạt động diễn đàn",
            Badge = "Diễn đàn",
            EmptyText = "Chưa có thảo luận mới",
            Icon = "forum",
            IconColorClass = "text-purple-500",
            Url = boardsUrl,
            Entries = entries
        };
    }

    protected virtual async Task<HomepageNotificationItemModel> PrepareDocumentNotificationAsync()
    {
        var documentPortalUrl = "/tai-lieu";

        try
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var isGuest = currentCustomer == null || await _customerService.IsGuestAsync(currentCustomer);
            var rows = await _dataProvider.QueryAsync<DocumentNotificationRow>(
                @"SELECT TOP 3 [Title], [Slug], [Code], [IssuedDate], [UpdatedOnUtc], [UploadedByCustomerId]
                  FROM [Document]
                  WHERE [Published] = 1
                    AND [Deleted] = 0
                    AND (@IsGuest = 0 OR [AccessScopeId] = @PublicAccessScopeId)
                  ORDER BY [IssuedDate] DESC, [UpdatedOnUtc] DESC, [Id] DESC",
                new DataParameter("IsGuest", isGuest ? 1 : 0),
                new DataParameter("PublicAccessScopeId", PublicDocumentAccessScopeId));

            var documents = rows.ToList();
            if (!documents.Any())
                return CreateEmptyNotification("Kho tài liệu", "folder_open", "text-teal-500", "Chưa có tài liệu mới", documentPortalUrl);

            var entries = new List<HomepageNotificationEntryModel>();
            foreach (var document in documents.Take(HomepageNotificationTake))
            {
                entries.Add(new HomepageNotificationEntryModel
                {
                    Badge = "Tài liệu",
                    DateText = await FormatDateAsync(document.IssuedDate ?? document.UpdatedOnUtc),
                    Subtitle = await GetCustomerDisplayNameAsync(document.UploadedByCustomerId ?? 0, "Kho tài liệu"),
                    Title = document.Title,
                    Url = string.IsNullOrWhiteSpace(document.Slug) ? documentPortalUrl : $"/tai-lieu/{document.Slug}"
                });
            }

            return new HomepageNotificationItemModel
            {
                AreaTitle = "Tài liệu mới",
                Badge = "Kho tài liệu",
                EmptyText = "Chưa có tài liệu mới",
                Icon = "folder_open",
                IconColorClass = "text-teal-500",
                Url = documentPortalUrl,
                Entries = entries
            };
        }
        catch
        {
            return CreateEmptyNotification("Kho tài liệu", "folder_open", "text-teal-500", "Chưa có tài liệu mới", documentPortalUrl);
        }
    }

    protected virtual HomepageNotificationItemModel CreateEmptyNotification(string areaTitle, string icon, string iconColorClass, string emptyText, string url)
    {
        return new HomepageNotificationItemModel
        {
            AreaTitle = areaTitle,
            Badge = "Mới nhất",
            EmptyText = emptyText,
            Icon = icon,
            IconColorClass = iconColorClass,
            Url = url
        };
    }

    protected virtual async Task<string> FormatDateAsync(DateTime dateTimeUtc)
    {
        var userTime = await _dateTimeHelper.ConvertToUserTimeAsync(dateTimeUtc, DateTimeKind.Utc);
        return userTime.ToString("dd/MM/yyyy HH:mm");
    }

    protected virtual async Task<string> GetCustomerDisplayNameAsync(int customerId, string fallback)
    {
        if (customerId <= 0)
            return fallback;

        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null || await _customerService.IsGuestAsync(customer))
            return fallback;

        var fullName = await _customerService.GetCustomerFullNameAsync(customer);
        return string.IsNullOrWhiteSpace(fullName) ? await _customerService.FormatUsernameAsync(customer) : fullName;
    }

    private sealed class DocumentNotificationRow
    {
        public string Code { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public int? UploadedByCustomerId { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
