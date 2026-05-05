using Nop.Core.Http;
using Nop.Services.Installation;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure;

/// <summary>
/// Represents provider that provided basic routes
/// </summary>
public partial class RouteProvider : BaseRouteProvider, IRouteProvider
{
    #region Methods

    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public virtual void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        //get language pattern
        //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
        //use it only for URLs of pages that the user can go to
        var lang = GetLanguageRoutePattern();

        //areas
        endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
            pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

        //home page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.HOMEPAGE,
            pattern: $"{lang}",
            defaults: new { controller = "Home", action = "Index" });

        //login
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.LOGIN,
            pattern: $"{lang}/login/",
            defaults: new { controller = "Customer", action = "Login" });

        // multi-factor verification digit code page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.MULTIFACTOR_VERIFICATION,
            pattern: $"{lang}/multi-factor-verification/",
            defaults: new { controller = "Customer", action = "MultiFactorVerification" });

        //register
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.REGISTER,
            pattern: $"{lang}/register/",
            defaults: new { controller = "Customer", action = "Register" });

        //logout
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.LOGOUT,
            pattern: $"{lang}/logout/",
            defaults: new { controller = "Customer", action = "Logout" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT,
            pattern: $"{lang}/customer/profile",
            defaults: new { controller = "Customer", action = "Profile" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CUSTOMER_INFO,
            pattern: $"{lang}/customer/info",
            defaults: new { controller = "Customer", action = "Info" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerEditAccount",
            pattern: $"{lang}/customer/edit",
            defaults: new { controller = "Customer", action = "Info" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CUSTOMER_ADDRESSES,
            pattern: $"{lang}/customer/addresses",
            defaults: new { controller = "Customer", action = "Addresses" });

        //customer address delete (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.CUSTOMER_ADDRESS_DELETE,
            pattern: $"customer/addressdelete",
            defaults: new { controller = "Customer", action = "AddressDelete" });

        //remove external association (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.CUSTOMER_REMOVE_EXTERNAL_ASSOCIATION,
            pattern: $"customer/removeexternalassociation",
            defaults: new { controller = "Customer", action = "RemoveExternalAssociation" });


        //product search
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.SEARCH,
            pattern: $"{lang}/search/",
            defaults: new { controller = "Catalog", action = "Search" });

        //product search by filter level values
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRODUCT_SEARCH_BY_FILTER_LEVEL_VALUES,
            pattern: $"{lang}/search-ymm/",
            defaults: new { controller = "Catalog", action = "SearchByFilterLevelValues" });

        //autocomplete search term (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.PRODUCT_SEARCH_AUTOCOMPLETE,
            pattern: $"catalog/searchtermautocomplete",
            defaults: new { controller = "Catalog", action = "SearchTermAutoComplete" });

        //change currency
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHANGE_CURRENCY,
            pattern: $"{lang}/changecurrency/{{customercurrency:min(0)}}",
            defaults: new { controller = "Common", action = "SetCurrency" });

        //change language
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHANGE_LANGUAGE,
            pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
            defaults: new { controller = "Common", action = "SetLanguage" });

        //set store theme
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SET_STORE_THEME,
            pattern: $"{lang}/setstoretheme/{{themeName}}/{{returnUrl}}",
            defaults: new { controller = "Common", action = "SetStoreTheme" });

        //recently viewed products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.RECENTLY_VIEWED_PRODUCTS,
            pattern: $"{lang}/recentlyviewedproducts/",
            defaults: new { controller = "Product", action = "RecentlyViewedProducts" });

        //new products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.NEW_PRODUCTS,
            pattern: $"{lang}/newproducts/",
            defaults: new { controller = "Catalog", action = "NewProducts" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.BLOG,
            pattern: $"{lang}/blog",
            defaults: new { controller = "Blog", action = "List" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_NEWS,
            pattern: $"{lang}/blog/news",
            defaults: new { controller = "Blog", action = "ListByType", postTypeId = 0 });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_DOCUMENTS,
            pattern: $"{lang}/blog/documents",
            defaults: new { controller = "Blog", action = "ListByType", postTypeId = 1 });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_NEWS_POST,
            pattern: $"{lang}/blog/news/{{{NopRoutingDefaults.RouteValue.SeName}}}",
            defaults: new { controller = "Blog", action = "PostByType", postTypeId = 0 });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_DOCUMENT_POST,
            pattern: $"{lang}/blog/documents/{{{NopRoutingDefaults.RouteValue.SeName}}}",
            defaults: new { controller = "Blog", action = "PostByType", postTypeId = 1 });

        //forum
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.BOARDS,
            pattern: $"{lang}/boards",
            defaults: new { controller = "Boards", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BOARDS_PAGED,
            pattern: $"{lang}/boards/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Index" });

        //compare products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.COMPARE_PRODUCTS,
            pattern: $"{lang}/compareproducts/",
            defaults: new { controller = "Product", action = "CompareProducts" });

        //product tags
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.PRODUCT_TAGS,
            pattern: $"{lang}/producttag/all/",
            defaults: new { controller = "Catalog", action = "ProductTagsAll" });

        //manufacturers
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.MANUFACTURERS,
            pattern: $"{lang}/manufacturer/all/",
            defaults: new { controller = "Catalog", action = "VendorAll" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CATEGORIES,
            pattern: $"{lang}/catalog/",
            defaults: new { controller = "Catalog", action = "CategoryAll" });

        //vendors
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.VENDORS,
            pattern: $"{lang}/vendor/all/",
            defaults: new { controller = "Catalog", action = "VendorAll" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.UNITS,
            pattern: $"{lang}/don-vi",
            defaults: new { controller = "Catalog", action = "VendorAll" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.UNIT_DETAILS,
            pattern: $"{lang}/don-vi/{{vendorId:min(0)}}",
            defaults: new { controller = "Catalog", action = "Vendor" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.VENDOR_REVIEWS,
            pattern: $"{lang}/vendor/{{vendorId:min(0)}}/reviews",
            defaults: new { controller = "Catalog", action = "VendorReviews" });

        //comparing products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.ADD_PRODUCT_TO_COMPARE,
            pattern: $"compareproducts/add/{{productId:min(0)}}",
            defaults: new { controller = "Product", action = "AddProductToCompareList" });

        //reviews
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PRODUCT_REVIEWS,
            pattern: $"{lang}/customer/productreviews",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PRODUCT_REVIEWS_PAGED,
            pattern: $"{lang}/customer/productreviews/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        //back in stock notifications (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.BACK_IN_STOCK_SUBSCRIBE_POPUP,
            pattern: $"backinstocksubscribe/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopup" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.BACK_IN_STOCK_SUBSCRIBE_SEND,
            pattern: $"backinstocksubscribesend/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

        //downloads (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GET_SAMPLE_DOWNLOAD,
            pattern: $"download/sample/{{productid:min(0)}}",
            defaults: new { controller = "Download", action = "Sample" });

        //downloads
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.DOWNLOAD_GET_FILE_UPLOAD,
            pattern: $"download/getfileupload/{{downloadId}}",
            defaults: new { controller = "Download", action = "GetFileUpload" });

        //register result page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.REGISTER_RESULT,
            pattern: $"{lang}/registerresult/{{resultId:min(0)}}",
            defaults: new { controller = "Customer", action = "RegisterResult" });

        //check username availability (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.CHECK_USERNAME_AVAILABILITY,
            pattern: $"customer/checkusernameavailability",
            defaults: new { controller = "Customer", action = "CheckUsernameAvailability" });

        //passwordrecovery
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PASSWORD_RECOVERY,
            pattern: $"{lang}/passwordrecovery",
            defaults: new { controller = "Customer", action = "PasswordRecovery" });

        //password recovery confirmation
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PASSWORD_RECOVERY_CONFIRM,
            pattern: $"{lang}/passwordrecovery/confirm",
            defaults: new { controller = "Customer", action = "PasswordRecoveryConfirm" });

        //topics (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.TOPIC_POPUP,
            pattern: $"t-popup/{{SystemName}}",
            defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_BY_TAG,
            pattern: $"{lang}/blog/tag/{{tag}}",
            defaults: new { controller = "Blog", action = "BlogByTag" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_BY_MONTH,
            pattern: $"{lang}/blog/month/{{month}}",
            defaults: new { controller = "Blog", action = "BlogByMonth" });

        //blog RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_RSS,
            pattern: $"blog/rss/{{languageId:min(0)}}",
            defaults: new { controller = "Blog", action = "ListRss" });

        //set review helpfulness (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.SET_PRODUCT_REVIEW_HELPFULNESS,
            pattern: $"setproductreviewhelpfulness",
            defaults: new { controller = "Product", action = "SetProductReviewHelpfulness" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_DOWNLOADABLE_PRODUCTS,
            pattern: $"{lang}/customer/downloadableproducts",
            defaults: new { controller = "Customer", action = "DownloadableProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_BACK_IN_STOCK_SUBSCRIPTIONS,
            pattern: $"{lang}/backinstocksubscriptions/manage/{{pageNumber:int?}}",
            defaults: new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_CHANGE_PASSWORD,
            pattern: $"{lang}/customer/changepassword",
            defaults: new { controller = "Customer", action = "ChangePassword" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_AVATAR,
            pattern: $"{lang}/customer/avatar",
            defaults: new { controller = "Customer", action = "Avatar" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACCOUNT_ACTIVATION,
            pattern: $"{lang}/customer/activation",
            defaults: new { controller = "Customer", action = "AccountActivation" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.EMAIL_REVALIDATION,
            pattern: $"{lang}/customer/revalidateemail",
            defaults: new { controller = "Customer", action = "EmailRevalidation" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_FORUM_SUBSCRIPTIONS,
            pattern: $"{lang}/boards/forumsubscriptions/{{pageNumber:int?}}",
            defaults: new { controller = "Boards", action = "CustomerForumSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_ADDRESS_EDIT,
            pattern: $"{lang}/customer/addressedit/{{addressId:min(0)}}",
            defaults: new { controller = "Customer", action = "AddressEdit" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_ADDRESS_ADD,
            pattern: $"{lang}/customer/addressadd",
            defaults: new { controller = "Customer", action = "AddressAdd" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_MULTI_FACTOR_AUTHENTICATION_PROVIDER_CONFIG,
            pattern: $"{lang}/customer/providerconfig",
            defaults: new { controller = "Customer", action = "ConfigureMultiFactorAuthenticationProvider" });

        //customer profile page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PROFILE,
            pattern: $"{lang}/profile/{{id:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PROFILE_PAGED,
            pattern: $"{lang}/profile/{{id:min(0)}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });


        //apply for vendor account
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.APPLY_VENDOR_ACCOUNT,
            pattern: $"{lang}/vendor/apply",
            defaults: new { controller = "Vendor", action = "ApplyVendor" });

        //vendor info
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_VENDOR_INFO,
            pattern: $"{lang}/customer/vendorinfo",
            defaults: new { controller = "Vendor", action = "Info" });

        //customer GDPR
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GDPR_TOOLS,
            pattern: $"{lang}/customer/gdpr",
            defaults: new { controller = "Customer", action = "GdprTools" });

        //customer check gift card balance 
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CHECK_GIFT_CARD_BALANCE,
            pattern: $"{lang}/customer/checkgiftcardbalance",
            defaults: new { controller = "Customer", action = "CheckGiftCardBalance" });

        //customer multi-factor authentication settings 
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.MULTI_FACTOR_AUTHENTICATION_SETTINGS,
            pattern: $"{lang}/customer/multifactorauthentication",
            defaults: new { controller = "Customer", action = "MultiFactorAuthentication" });

        //comparing products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.REMOVE_PRODUCT_FROM_COMPARE_LIST,
            pattern: $"{lang}/compareproducts/remove/{{productId}}",
            defaults: new { controller = "Product", action = "RemoveProductFromCompareList" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CLEAR_COMPARE_LIST,
            pattern: $"{lang}/clearcomparelist/",
            defaults: new { controller = "Product", action = "ClearCompareList" });

        //new RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.NEW_PRODUCTS_RSS,
            pattern: $"newproducts/rss",
            defaults: new { controller = "Catalog", action = "NewProductsRss" });

        //get state list by country ID (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_STATES_BY_COUNTRY_ID,
            pattern: $"country/getstatesbycountryid/",
            defaults: new { controller = "Country", action = "GetStatesByCountryId" });

        //get filter level value list by parent ID (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_FILTER_LEVEL_VALUES,
            pattern: $"catalog/getfilterlevelvalues/",
            defaults: new { controller = "Catalog", action = "GetFilterLevelValues" });

        //EU Cookie law accept button handler (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.EU_COOKIE_LAW_ACCEPT,
            pattern: $"eucookielawaccept",
            defaults: new { controller = "Common", action = "EuCookieLawAccept" });

        //authenticate topic (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.TOPIC_AUTHENTICATE,
            pattern: $"topic/authenticate",
            defaults: new { controller = "Topic", action = "Authenticate" });

        //Catalog products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_CATEGORY_PRODUCTS,
            pattern: $"category/products/",
            defaults: new { controller = "Catalog", action = "GetCategoryProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_MANUFACTURER_PRODUCTS,
            pattern: $"manufacturer/products/",
            defaults: new { controller = "Catalog", action = "GetManufacturerProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_TAG_PRODUCTS,
            pattern: $"tag/products",
            defaults: new { controller = "Catalog", action = "GetTagProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.SEARCH_PRODUCTS,
            pattern: "product/search",
            defaults: new { controller = "Catalog", action = "SearchProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.SEARCH_PRODUCTS_BY_FILTER_LEVEL_VALUES,
            pattern: "product/searchbyflv",
            defaults: new { controller = "Catalog", action = "SearchProductsByFilterLevelValues" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_VENDOR_PRODUCTS,
            pattern: $"vendor/products",
            defaults: new { controller = "Catalog", action = "GetVendorProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_NEW_PRODUCTS,
            pattern: $"newproducts/products/",
            defaults: new { controller = "Catalog", action = "GetNewProducts" });

        //product combinations (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_PRODUCT_COMBINATIONS,
            pattern: $"product/combinations",
            defaults: new { controller = "Product", action = "GetProductCombinations" });

        //forums
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACTIVE_DISCUSSIONS,
            pattern: $"{lang}/boards/activediscussions",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACTIVE_DISCUSSIONS_PAGED,
            pattern: $"{lang}/boards/activediscussions/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACTIVE_DISCUSSIONS_RSS,
            pattern: $"boards/activediscussionsrss",
            defaults: new { controller = "Boards", action = "ActiveDiscussionsRSS" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_EDIT,
            pattern: $"{lang}/boards/postedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostEdit" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_DELETE,
            pattern: $"{lang}/boards/postdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "PostDelete" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_CREATE,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_CREATE_QUOTE,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}/{{quote:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_EDIT,
            pattern: $"{lang}/boards/topicedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicEdit" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_DELETE,
            pattern: $"{lang}/boards/topicdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "TopicDelete" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_CREATE,
            pattern: $"{lang}/boards/topiccreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicCreate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_MOVE,
            pattern: $"{lang}/boards/topicmove/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicMove" });

        //topic watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.TOPIC_WATCH,
            pattern: $"boards/topicwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicWatch" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_SLUG,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Topic" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_SLUG_PAGED,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Topic" });

        //forum watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.FORUM_WATCH,
            pattern: $"boards/forumwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumWatch" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_RSS,
            pattern: $"boards/forumrss/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumRSS" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_SLUG,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_SLUG_PAGED,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_GROUP_SLUG,
            pattern: $"{lang}/boards/forumgroup/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "ForumGroup" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BOARDS_SEARCH,
            pattern: $"{lang}/boards/search",
            defaults: new { controller = "Boards", action = "Search" });

        //post vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.POST_VOTE,
            pattern: "boards/postvote",
            defaults: new { controller = "Boards", action = "PostVote" });

        //private messages
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES,
            pattern: $"{lang}/privatemessages/{{tab?}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES_PAGED,
            pattern: $"{lang}/privatemessages/{{tab?}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES_INBOX,
            pattern: $"{lang}/inboxupdate",
            defaults: new { controller = "PrivateMessages", action = "InboxUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES_SENT,
            pattern: $"{lang}/sentupdate",
            defaults: new { controller = "PrivateMessages", action = "SentUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SEND_PM,
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SEND_PM_REPLY,
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}/{{replyToMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.VIEW_PM,
            pattern: $"{lang}/viewpm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "ViewPM" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.DELETE_PM,
            pattern: $"{lang}/deletepm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "DeletePM" });

        //robots.txt (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ROBOTS_TXT,
            pattern: $"robots.txt",
            defaults: new { controller = "Common", action = "RobotsTextFile" });

        //sitemap
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.SITEMAP,
            pattern: $"{lang}/sitemap",
            defaults: new { controller = "Common", action = "Sitemap" });

        //sitemap.xml (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SITEMAP_XML,
            pattern: $"sitemap.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SITEMAP_INDEXED_XML,
            pattern: $"sitemap-{{Id:min(0)}}.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        //store closed
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.STORE_CLOSED,
            pattern: $"{lang}/storeclosed",
            defaults: new { controller = "Common", action = "StoreClosed" });

        //install
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.INSTALLATION,
            pattern: $"{NopInstallationDefaults.InstallPath}",
            defaults: new { controller = "Install", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.INSTALLATION_CHANGE_LANGUAGE,
            pattern: $"{NopInstallationDefaults.InstallPath}/ChangeLanguage/{{language}}",
            defaults: new { controller = "Install", action = "ChangeLanguage" });

        //restart application (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.INSTALLATION_RESTART_APPLICATION,
            pattern: $"{NopInstallationDefaults.InstallPath}/restartapplication",
            defaults: new { controller = "Install", action = "RestartApplication" });

        //page not found
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PAGE_NOT_FOUND,
            pattern: $"{lang}/page-not-found",
            defaults: new { controller = "Common", action = "PageNotFound" });

        //fallback is intended to handle cases when no other endpoint has matched
        //we use it to invoke [CheckLanguageSeoCode] and give a chance to find a localized route
        endpointRouteBuilder.MapFallbackToController("FallbackRedirect", "Common");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;

    #endregion
}
