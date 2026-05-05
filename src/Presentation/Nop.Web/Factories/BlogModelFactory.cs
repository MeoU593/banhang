using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Services.Blogs;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the blog model factory
/// </summary>
public partial class BlogModelFactory : IBlogModelFactory
{
    private static readonly Regex ReplyToTokenRegex = new(@"^\[replyto:(\d+)\]\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    #region Fields

    protected readonly BlogSettings _blogSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly IBlogService _blogService;
    protected readonly ICustomerMilitaryProfileService _customerMilitaryProfileService;
    protected readonly ICustomerService _customerService;
    protected readonly IDownloadService _downloadService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IPictureService _pictureService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public BlogModelFactory(BlogSettings blogSettings,
        CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        IBlogService blogService,
        ICustomerMilitaryProfileService customerMilitaryProfileService,
        ICustomerService customerService,
        IDownloadService downloadService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWorkContext workContext,
        MediaSettings mediaSettings)
    {
        _blogSettings = blogSettings;
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _blogService = blogService;
        _customerMilitaryProfileService = customerMilitaryProfileService;
        _customerService = customerService;
        _downloadService = downloadService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
    }

    #endregion

    #region Utilities

    protected virtual async Task<BlogPostModel> PrepareBlogPostPreviewModelAsync(BlogPost blogPost)
    {
        var model = new BlogPostModel
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            BodyOverview = blogPost.BodyOverview,
            Body = blogPost.Body,
            PostTypeId = blogPost.PostTypeId,
            ThumbnailPictureId = blogPost.ThumbnailPictureId,
            AuthorName = blogPost.AuthorName,
            ViewCount = blogPost.ViewCount,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.StartDateUtc ?? blogPost.CreatedOnUtc, DateTimeKind.Utc),
            SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false)
        };

        if (blogPost.ThumbnailPictureId > 0)
            model.ThumbnailImageUrl = await _pictureService.GetPictureUrlAsync(blogPost.ThumbnailPictureId, showDefaultPicture: false);

        return model;
    }

    private static (int ReplyToCommentId, string DisplayText) ParseReplyData(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return (0, string.Empty);

        var match = ReplyToTokenRegex.Match(text);
        if (!match.Success)
            return (0, text);

        _ = int.TryParse(match.Groups[1].Value, out var replyToCommentId);
        return (replyToCommentId, text[match.Length..].TrimStart());
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare blog post model
    /// </summary>
    /// <param name="model">Blog post model</param>
    /// <param name="blogPost">Blog post entity</param>
    /// <param name="prepareComments">Whether to prepare blog comments</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool prepareComments)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(blogPost);

        model.Id = blogPost.Id;
        model.MetaTitle = blogPost.MetaTitle;
        model.MetaDescription = blogPost.MetaDescription;
        model.MetaKeywords = blogPost.MetaKeywords;
        model.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
        model.Title = blogPost.Title;
        model.Body = blogPost.Body;
        model.BodyOverview = blogPost.BodyOverview;
        model.PostTypeId = blogPost.PostTypeId;
        model.ThumbnailPictureId = blogPost.ThumbnailPictureId;
        model.AuthorName = blogPost.AuthorName;
        model.PdfDownloadId = blogPost.PdfDownloadId;
        model.DocDownloadId = blogPost.DocDownloadId;
        model.AllowComments = blogPost.AllowComments;
        model.ViewCount = blogPost.ViewCount;

        if (blogPost.ThumbnailPictureId > 0)
            model.ThumbnailImageUrl = await _pictureService.GetPictureUrlAsync(blogPost.ThumbnailPictureId, showDefaultPicture: false);

        if (blogPost.PdfDownloadId > 0)
        {
            var pdfDownload = await _downloadService.GetDownloadByIdAsync(blogPost.PdfDownloadId);
            if (pdfDownload != null)
                model.PdfDownloadGuid = pdfDownload.DownloadGuid;
        }

        if (blogPost.DocDownloadId > 0)
        {
            var docDownload = await _downloadService.GetDownloadByIdAsync(blogPost.DocDownloadId);
            if (docDownload != null)
                model.DocDownloadGuid = docDownload.DownloadGuid;
        }

        model.PreventNotRegisteredUsersToLeaveComments =
            await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) &&
            !_blogSettings.AllowNotRegisteredUsersToLeaveComments;

        model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogPost.StartDateUtc ?? blogPost.CreatedOnUtc, DateTimeKind.Utc);
        model.Tags = await _blogService.ParseTagsAsync(blogPost);
        model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage;

        //number of blog comments
        var store = await _storeContext.GetCurrentStoreAsync();
        var storeId = _blogSettings.ShowBlogCommentsPerStore ? store.Id : 0;

        model.NumberOfComments = await _blogService.GetBlogCommentsCountAsync(blogPost, storeId, true);

        if (prepareComments)
        {
            var blogComments = await _blogService.GetAllCommentsAsync(
                blogPostId: blogPost.Id,
                approved: true,
                storeId: storeId);

            foreach (var bc in blogComments)
            {
                var commentModel = await PrepareBlogPostCommentModelAsync(bc);
                model.Comments.Add(commentModel);
            }
        }

        var latestNewsPosts = await _blogService.GetAllBlogPostsAsync(
            store.Id,
            blogPost.LanguageId,
            pageIndex: 0,
            pageSize: 6,
            postTypeId: 0);

        model.LatestNewsPosts = await latestNewsPosts
            .Where(post => post.Id != blogPost.Id)
            .Take(5)
            .SelectAwait(async post => await PrepareBlogPostPreviewModelAsync(post))
            .ToListAsync();

        var popularNewsPosts = await _blogService.GetPopularBlogPostsAsync(
            store.Id,
            blogPost.LanguageId,
            pageSize: 5,
            postTypeId: 0,
            excludeBlogPostId: blogPost.Id);

        model.PopularNewsPosts = await popularNewsPosts
            .SelectAwait(async post => await PrepareBlogPostPreviewModelAsync(post))
            .ToListAsync();
    }

    /// <summary>
    /// Prepare blog post list model
    /// </summary>
    /// <param name="command">Blog paging filtering model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post list model
    /// </returns>
    public virtual async Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPagingFilteringModel command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.PageSize <= 0)
            command.PageSize = _blogSettings.PostsPageSize;
        if (command.PageNumber <= 0)
            command.PageNumber = 1;

        var dateFrom = command.GetFromDate();
        var dateTo = command.GetToDate();
        if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
            (dateFrom, dateTo) = (dateTo, dateFrom);

        var language = await _workContext.GetWorkingLanguageAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var vendorIds = command.VendorId > 0 ? new List<int> { command.VendorId } : null;
        var blogPosts = string.IsNullOrEmpty(command.Tag)
            ? await _blogService.GetAllBlogPostsAsync(store.Id, language.Id, dateFrom, dateTo, command.PageNumber - 1, command.PageSize, postTypeId: command.PostTypeId, vendorIds: vendorIds, keywords: command.Q)
            : await _blogService.GetAllBlogPostsByTagAsync(store.Id, language.Id, command.Tag, command.PageNumber - 1, command.PageSize, vendorIds: vendorIds);

        var availableVendors = (await _vendorService.GetAllVendorsAsync(showHidden: false))
            .OrderBy(vendor => vendor.DisplayOrder)
            .ThenBy(vendor => vendor.Name)
            .Select(vendor => new SelectListItem
            {
                Text = vendor.Name,
                Value = vendor.Id.ToString(),
                Selected = vendor.Id == command.VendorId
            })
            .ToList();
        availableVendors.Insert(0, new SelectListItem { Text = "Tất cả đơn vị", Value = "0", Selected = command.VendorId <= 0 });

        var model = new BlogPostListModel
        {
            PagingFilteringContext =
            {
                Q = command.Q,
                Tag = command.Tag,
                Month = command.Month,
                PostTypeId = command.PostTypeId,
                VendorId = command.VendorId,
                DateFrom = command.DateFrom,
                DateTo = command.DateTo
            },
            WorkingLanguageId = language.Id,
            AvailableVendors = availableVendors,
            BlogPosts = await blogPosts.SelectAwait(async blogPost =>
            {
                var blogPostModel = new BlogPostModel();
                await PrepareBlogPostModelAsync(blogPostModel, blogPost, false);
                return blogPostModel;
            }).ToListAsync()
        };
        model.PagingFilteringContext.LoadPagedList(blogPosts);

        if (!command.PostTypeId.HasValue && string.IsNullOrEmpty(command.Tag))
        {
            var latestDocumentPosts = await _blogService.GetAllBlogPostsAsync(
                store.Id,
                language.Id,
                dateFrom,
                dateTo,
                pageIndex: 0,
                pageSize: 8,
                postTypeId: 1,
                vendorIds: vendorIds,
                keywords: command.Q);

            model.LatestDocumentPosts = await latestDocumentPosts.SelectAwait(async blogPost =>
            {
                var blogPostModel = new BlogPostModel();
                await PrepareBlogPostModelAsync(blogPostModel, blogPost, false);
                return blogPostModel;
            }).ToListAsync();
        }

        return model;
    }

    /// <summary>
    /// Prepare blog post tag list model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog post tag list model
    /// </returns>
    public virtual async Task<BlogPostTagListModel> PrepareBlogPostTagListModelAsync()
    {
        var model = new BlogPostTagListModel();
        var store = await _storeContext.GetCurrentStoreAsync();

        //get tags
        var tags = (await _blogService
                .GetAllBlogPostTagsAsync(store.Id, (await _workContext.GetWorkingLanguageAsync()).Id))
            .OrderByDescending(x => x.BlogPostCount)
            .Take(_blogSettings.NumberOfTags);

        //sorting and setting into the model
        model.Tags.AddRange(tags.OrderBy(x => x.Name).Select(tag => new BlogPostTagModel
        {
            Name = tag.Name,
            BlogPostCount = tag.BlogPostCount
        }));

        return model;
    }

    /// <summary>
    /// Prepare blog post year models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of blog post year model
    /// </returns>
    public virtual async Task<List<BlogPostYearModel>> PrepareBlogPostYearModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.BlogMonthsModelKey, currentLanguage, store);
        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var model = new List<BlogPostYearModel>();

            var blogPosts = await _blogService.GetAllBlogPostsAsync(store.Id,
                currentLanguage.Id);
            if (blogPosts.Any())
            {
                var months = new SortedDictionary<DateTime, int>();

                var blogPost = blogPosts[blogPosts.Count - 1];
                var first = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc;
                while (DateTime.SpecifyKind(first, DateTimeKind.Utc) <= DateTime.UtcNow.AddMonths(1))
                {
                    var list = await _blogService.GetPostsByDateAsync(blogPosts, new DateTime(first.Year, first.Month, 1),
                        new DateTime(first.Year, first.Month, 1).AddMonths(1).AddSeconds(-1));
                    if (list.Any())
                    {
                        var date = new DateTime(first.Year, first.Month, 1);
                        months.Add(date, list.Count);
                    }

                    first = first.AddMonths(1);
                }

                var current = 0;
                foreach (var kvp in months)
                {
                    var date = kvp.Key;
                    var blogPostCount = kvp.Value;
                    if (current == 0)
                        current = date.Year;

                    if (date.Year > current || !model.Any())
                    {
                        var yearModel = new BlogPostYearModel
                        {
                            Year = date.Year
                        };
                        model.Insert(0, yearModel);
                    }

                    model.First().Months.Insert(0, new BlogPostMonthModel
                    {
                        Month = date.Month,
                        BlogPostCount = blogPostCount
                    });

                    current = date.Year;
                }
            }

            return model;
        });

        return cachedModel;
    }

    /// <summary>
    /// Prepare blog comment model
    /// </summary>
    /// <param name="blogComment">Blog comment entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog comment model
    /// </returns>
    public virtual async Task<BlogCommentModel> PrepareBlogPostCommentModelAsync(BlogComment blogComment)
    {
        ArgumentNullException.ThrowIfNull(blogComment);

        var customer = await _customerService.GetCustomerByIdAsync(blogComment.CustomerId);
        var customerIsGuest = customer == null || await _customerService.IsGuestAsync(customer);
        var militaryProfile = customerIsGuest ? null : await _customerMilitaryProfileService.GetByCustomerIdAsync(blogComment.CustomerId);
        var (replyToCommentId, displayCommentText) = ParseReplyData(blogComment.CommentText);

        var model = new BlogCommentModel
        {
            Id = blogComment.Id,
            ReplyToCommentId = replyToCommentId,
            CustomerId = blogComment.CustomerId,
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            CommentText = displayCommentText,
            Rank = militaryProfile?.Rank,
            UnitName = militaryProfile?.UnitName,
            PositionTitle = militaryProfile?.PositionTitle,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc),
            AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !customerIsGuest
        };

        if (_customerSettings.AllowCustomersToUploadAvatars && customer != null)
        {
            model.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
        }

        return model;
    }

    #endregion
}
