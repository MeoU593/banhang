using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Translation;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the setting model factory implementation
/// </summary>
public partial class SettingModelFactory : ISettingModelFactory
{
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerAttributeModelFactory _customerAttributeModelFactory;
    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGdprService _gdprService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IReviewTypeModelFactory _reviewTypeModelFactory;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IThemeProvider _themeProvider;
    protected readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SettingModelFactory(AppSettings appSettings,
        CurrencySettings currencySettings,
        FilterLevelSettings filterLevelSettings,
        IAddressModelFactory addressModelFactory,
        IAddressAttributeModelFactory addressAttributeModelFactory,
        IAddressService addressService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICurrencyService currencyService,
        ICustomerAttributeModelFactory customerAttributeModelFactory,
        INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        IDateTimeHelper dateTimeHelper,
        IGdprService gdprService,
        ILocalizedModelFactory localizedModelFactory,
        IGenericAttributeService genericAttributeService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IThemeProvider themeProvider,
        IVendorAttributeModelFactory vendorAttributeModelFactory,
        IReviewTypeModelFactory reviewTypeModelFactory,
        IWorkContext workContext)
    {
        _appSettings = appSettings;
        _currencySettings = currencySettings;
        _filterLevelSettings = filterLevelSettings;
        _addressModelFactory = addressModelFactory;
        _addressAttributeModelFactory = addressAttributeModelFactory;
        _addressService = addressService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _currencyService = currencyService;
        _customerAttributeModelFactory = customerAttributeModelFactory;
        _dataProvider = dataProvider;
        _fileProvider = fileProvider;
        _dateTimeHelper = dateTimeHelper;
        _gdprService = gdprService;
        _localizedModelFactory = localizedModelFactory;
        _genericAttributeService = genericAttributeService;
        _languageService = languageService;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeService = storeService;
        _themeProvider = themeProvider;
        _vendorAttributeModelFactory = vendorAttributeModelFactory;
        _reviewTypeModelFactory = reviewTypeModelFactory;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare store theme models
    /// </summary>
    /// <param name="models">List of store theme models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareStoreThemeModelsAsync(IList<StoreInformationSettingsModel.ThemeModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);

        //get available themes
        var availableThemes = await _themeProvider.GetThemesAsync();
        foreach (var theme in availableThemes)
        {
            models.Add(new StoreInformationSettingsModel.ThemeModel
            {
                FriendlyName = theme.FriendlyName,
                SystemName = theme.SystemName,
                PreviewImageUrl = theme.PreviewImageUrl,
                PreviewText = theme.PreviewText,
                SupportRtl = theme.SupportRtl,
                Selected = theme.SystemName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
            });
        }
    }

    /// <summary>
    /// Prepare sort option search model
    /// </summary>
    /// <param name="searchModel">Sort option search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sort option search model
    /// </returns>
    protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModelAsync(SortOptionSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare filter level search model
    /// </summary>
    /// <param name="searchModel">Filter level search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level search model
    /// </returns>
    protected virtual Task<FilterLevelSearchModel> PrepareFilterLevelSearchModelAsync(FilterLevelSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare GPSR settings model
    /// </summary>
    /// <param name="model">GPSR search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the GPSR settings model
    /// </returns>
    protected virtual async Task<GpsrSettingsModel> PrepareGpsrSettingsModelAsync(GpsrSettingsModel model)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var gpsrSettings = await _settingService.LoadSettingAsync<GpsrSettings>(storeId);

        //fill in model values from the entity
        model ??= new GpsrSettingsModel();
        model.Enabled = gpsrSettings.Enabled;

        //fill in overridden values
        if (storeId > 0)
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(gpsrSettings, x => x.Enabled, storeId);

        return model;
    }

    /// <summary>
    /// Prepare artificial intelligence settings model
    /// </summary>
    /// <param name="model">Artificial intelligence search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the artificial intelligence settings model
    /// </returns>
    protected virtual async Task<ArtificialIntelligenceSettingsModel> PrepareArtificialIntelligenceSettingsModelAsync(ArtificialIntelligenceSettingsModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var artificialIntelligenceSettings = await _settingService.LoadSettingAsync<ArtificialIntelligenceSettings>();

        model.Enabled = artificialIntelligenceSettings.Enabled;
        model.ChatGptApiKey = artificialIntelligenceSettings.ChatGptApiKey;
        model.DeepSeekApiKey = artificialIntelligenceSettings.DeepSeekApiKey;
        model.GeminiApiKey = artificialIntelligenceSettings.GeminiApiKey;
        model.ProviderTypeId = (int)artificialIntelligenceSettings.ProviderType;
        model.AllowProductDescriptionGeneration = artificialIntelligenceSettings.AllowProductDescriptionGeneration;
        model.ProductDescriptionQuery = artificialIntelligenceSettings.ProductDescriptionQuery;
        model.AllowMetaTitleGeneration = artificialIntelligenceSettings.AllowMetaTitleGeneration;
        model.MetaTitleQuery = artificialIntelligenceSettings.MetaTitleQuery;
        model.AllowMetaKeywordsGeneration = artificialIntelligenceSettings.AllowMetaKeywordsGeneration;
        model.MetaKeywordsQuery = artificialIntelligenceSettings.MetaKeywordsQuery;
        model.AllowMetaDescriptionGeneration = artificialIntelligenceSettings.AllowMetaDescriptionGeneration;
        model.MetaDescriptionQuery = artificialIntelligenceSettings.MetaDescriptionQuery;
        model.LogRequests = artificialIntelligenceSettings.LogRequests;

        //prepare available translation services
        var availableProviderType = await ArtificialIntelligenceProviderType.Gemini.ToSelectListAsync(false);
        model.AvailableProviderType = availableProviderType.ToList();

        return model;
    }

    /// <summary>
    /// Prepare GDPR consent search model
    /// </summary>
    /// <param name="searchModel">GDPR consent search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent search model
    /// </returns>
    protected virtual Task<GdprConsentSearchModel> PrepareGdprConsentSearchModelAsync(GdprConsentSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare address settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address settings model
    /// </returns>
    protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>(storeId);

        //fill in model values from the entity
        var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

        return model;
    }

    /// <summary>
    /// Prepare customer settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer settings model
    /// </returns>
    protected virtual async Task<CustomerSettingsModel> PrepareCustomerSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var customerSettings = await _settingService.LoadSettingAsync<CustomerSettings>(storeId);

        //fill in model values from the entity
        var model = customerSettings.ToSettingsModel<CustomerSettingsModel>();

        return model;
    }

    /// <summary>
    /// Prepare multi-factor authentication settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multiFactorAuthenticationSettingsModel
    /// </returns>
    protected virtual async Task<MultiFactorAuthenticationSettingsModel> PrepareMultiFactorAuthenticationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeId);

        //fill in model values from the entity
        var model = multiFactorAuthenticationSettings.ToSettingsModel<MultiFactorAuthenticationSettingsModel>();

        return model;

    }

    /// <summary>
    /// Prepare date time settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the date time settings model
    /// </returns>
    protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>(storeId);

        //fill in model values from the entity
        var model = new DateTimeSettingsModel
        {
            AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone
        };

        //fill in additional values (not existing in the entity)
        model.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;

        //prepare available time zones
        await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

        return model;
    }

    /// <summary>
    /// Prepare external authentication settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the external authentication settings model
    /// </returns>
    protected virtual async Task<ExternalAuthenticationSettingsModel> PrepareExternalAuthenticationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>(storeId);

        //fill in model values from the entity
        var model = new ExternalAuthenticationSettingsModel
        {
            AllowCustomersToRemoveAssociations = externalAuthenticationSettings.AllowCustomersToRemoveAssociations
        };

        return model;
    }

    /// <summary>
    /// Prepare store information settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store information settings model
    /// </returns>
    protected virtual async Task<StoreInformationSettingsModel> PrepareStoreInformationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);
        var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

        //fill in model values from the entity
        var model = new StoreInformationSettingsModel
        {
            StoreClosed = storeInformationSettings.StoreClosed,
            DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme,
            AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme,
            LogoPictureId = storeInformationSettings.LogoPictureId,
            DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning,
            FacebookLink = storeInformationSettings.FacebookLink,
            TwitterLink = storeInformationSettings.TwitterLink,
            YoutubeLink = storeInformationSettings.YoutubeLink,
            InstagramLink = storeInformationSettings.InstagramLink,
            SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm,
            UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm,
            PopupForTermsOfServiceLinks = commonSettings.PopupForTermsOfServiceLinks
        };

        //prepare available themes
        await PrepareStoreThemeModelsAsync(model.AvailableStoreThemes);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.StoreClosed_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.StoreClosed, storeId);
        model.DefaultStoreTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DefaultStoreTheme, storeId);
        model.AllowCustomerToSelectTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeId);
        model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.LogoPictureId, storeId);
        model.DisplayEuCookieLawWarning_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeId);
        model.FacebookLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.FacebookLink, storeId);
        model.TwitterLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.TwitterLink, storeId);
        model.YoutubeLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.YoutubeLink, storeId);
        model.InstagramLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.InstagramLink, storeId);
        model.SubjectFieldOnContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, storeId);
        model.UseSystemEmailForContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, storeId);
        model.PopupForTermsOfServiceLinks_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, storeId);

        return model;
    }

    /// <summary>
    /// Prepare Sitemap settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap settings model
    /// </returns>
    protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>(storeId);

        //fill in model values from the entity
        var model = new SitemapSettingsModel
        {
            SitemapEnabled = sitemapSettings.SitemapEnabled,
            SitemapPageSize = sitemapSettings.SitemapPageSize,
            SitemapIncludeCategories = sitemapSettings.SitemapIncludeCategories,
            SitemapIncludeManufacturers = sitemapSettings.SitemapIncludeManufacturers,
            SitemapIncludeProducts = sitemapSettings.SitemapIncludeProducts,
            SitemapIncludeProductTags = sitemapSettings.SitemapIncludeProductTags,
            SitemapIncludeBlogPosts = sitemapSettings.SitemapIncludeBlogPosts,
            SitemapIncludeTopics = sitemapSettings.SitemapIncludeTopics
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.SitemapEnabled_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapEnabled, storeId);
        model.SitemapPageSize_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapPageSize, storeId);
        model.SitemapIncludeCategories_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeCategories, storeId);
        model.SitemapIncludeManufacturers_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, storeId);
        model.SitemapIncludeProducts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProducts, storeId);
        model.SitemapIncludeProductTags_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProductTags, storeId);
        model.SitemapIncludeBlogPosts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, storeId);
        model.SitemapIncludeTopics_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTopics, storeId);

        return model;
    }

    /// <summary>
    /// Prepare minification settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the minification settings model
    /// </returns>
    protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var minificationSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

        //fill in model values from the entity
        var model = new MinificationSettingsModel
        {
            EnableHtmlMinification = minificationSettings.EnableHtmlMinification,
            UseResponseCompression = minificationSettings.UseResponseCompression
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.EnableHtmlMinification_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.EnableHtmlMinification, storeId);
        model.UseResponseCompression_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.UseResponseCompression, storeId);

        return model;
    }

    /// <summary>
    /// Prepare SEO settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sEO settings model
    /// </returns>
    protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeId);

        //fill in model values from the entity
        var model = new SeoSettingsModel
        {
            PageTitleSeparator = seoSettings.PageTitleSeparator,
            PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
            PageTitleSeoAdjustmentValues = await seoSettings.PageTitleSeoAdjustment.ToSelectListAsync(),
            GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription,
            ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
            CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
            WwwRequirement = (int)seoSettings.WwwRequirement,
            WwwRequirementValues = await seoSettings.WwwRequirement.ToSelectListAsync(),

            TwitterMetaTags = seoSettings.TwitterMetaTags,
            OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
            CustomHeadTags = seoSettings.CustomHeadTags,
            MicrodataEnabled = seoSettings.MicrodataEnabled
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.PageTitleSeparator_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeparator, storeId);
        model.PageTitleSeoAdjustment_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeoAdjustment, storeId);
        model.GenerateProductMetaDescription_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.GenerateProductMetaDescription, storeId);
        model.ConvertNonWesternChars_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.ConvertNonWesternChars, storeId);
        model.CanonicalUrlsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CanonicalUrlsEnabled, storeId);
        model.WwwRequirement_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.WwwRequirement, storeId);
        model.TwitterMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.TwitterMetaTags, storeId);
        model.OpenGraphMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.OpenGraphMetaTags, storeId);
        model.CustomHeadTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CustomHeadTags, storeId);
        model.MicrodataEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.MicrodataEnabled, storeId);

        return model;
    }

    /// <summary>
    /// Prepare security settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the security settings model
    /// </returns>
    protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeId);

        //fill in model values from the entity
        var model = new SecuritySettingsModel
        {
            EncryptionKey = securitySettings.EncryptionKey,
            HoneypotEnabled = securitySettings.HoneypotEnabled
        };

        //fill in additional values (not existing in the entity)
        if (securitySettings.AdminAreaAllowedIpAddresses != null)
            model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

        return model;
    }

    /// <summary>
    /// Prepare captcha settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the captcha settings model
    /// </returns>
    protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>(storeId);

        //fill in model values from the entity
        var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

        model.CaptchaTypeValues = await captchaSettings.CaptchaType.ToSelectListAsync();

        if (storeId <= 0)
            return model;

        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.Enabled, storeId);
        model.ShowOnLoginPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnLoginPage, storeId);
        model.ShowOnRegistrationPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnRegistrationPage, storeId);
        model.ShowOnContactUsPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnContactUsPage, storeId);
        model.ShowOnEmailWishlistToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, storeId);
        model.ShowOnEmailProductToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailProductToFriendPage, storeId);
        model.ShowOnBlogCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnBlogCommentPage, storeId);
        model.ShowOnNewsLetterPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsletterPage, storeId);
        model.ShowOnProductReviewPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnProductReviewPage, storeId);
        model.ShowOnApplyVendorPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnApplyVendorPage, storeId);
        model.ShowOnForgotPasswordPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, storeId);
        model.ShowOnForum_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForum, storeId);
        model.ShowOnCheckoutPageForGuests_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnCheckoutPageForGuests, storeId);
        model.ReCaptchaPublicKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPublicKey, storeId);
        model.ReCaptchaPrivateKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPrivateKey, storeId);
        model.CaptchaType_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.CaptchaType, storeId);
        model.ReCaptchaV3ScoreThreshold_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, storeId);

        return model;
    }

    /// <summary>
    /// Prepare PDF settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pDF settings model
    /// </returns>
    protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>(storeId);

        //fill in model values from the entity
        var model = new PdfSettingsModel
        {
            LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
            LogoPictureId = pdfSettings.LogoPictureId,
            DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders,
            InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1,
            InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.LetterPageSizeEnabled_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LetterPageSizeEnabled, storeId);
        model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LogoPictureId, storeId);
        model.DisablePdfInvoicesForPendingOrders_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeId);
        model.InvoiceFooterTextColumn1_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, storeId);
        model.InvoiceFooterTextColumn2_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, storeId);

        return model;
    }

    /// <summary>
    /// Prepare localization settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localization settings model
    /// </returns>
    protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeId);

        //fill in model values from the entity
        var model = new LocalizationSettingsModel
        {
            UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
            SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
            AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
            LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
            LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
            LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
        };

        return model;
    }

    /// <summary>
    /// Prepare translation settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation settings model
    /// </returns>
    protected virtual async Task<TranslationSettingsModel> PrepareTranslationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var translationSettings = await _settingService.LoadSettingAsync<TranslationSettings>(storeId);

        //fill in model values from the entity
        var model = new TranslationSettingsModel
        {
            AllowPreTranslate = translationSettings.AllowPreTranslate,
            TranslateFromLanguageId = translationSettings.TranslateFromLanguageId,
            NotTranslateLanguages = translationSettings.NotTranslateLanguages ?? new List<int>(),
            GoogleApiKey = translationSettings.GoogleApiKey,
            DeepLAuthKey = translationSettings.DeepLAuthKey,
            TranslationServiceId = translationSettings.TranslationServiceId
        };

        //prepare available translation services
        var availableTranslationServices = await TranslationServiceType.GoogleTranslate.ToSelectListAsync(false);
        model.AvailableTranslationService = availableTranslationServices.ToList();

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        return model;
    }

    /// <summary>
    /// Prepare admin area settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the admin area settings model
    /// </returns>
    protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>(storeId);

        //fill in model values from the entity
        var model = new AdminAreaSettingsModel
        {
            UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates,
            UseStickyHeaderLayout = adminAreaSettings.UseStickyHeaderLayout
        };

        //fill in overridden values
        if (storeId > 0)
            model.UseRichEditorInMessageTemplates_OverrideForStore = await _settingService.SettingExistsAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, storeId);

        return model;
    }

    /// <summary>
    /// Prepare setting model to add
    /// </summary>
    /// <param name="model">Setting model to add</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAddSettingModelAsync(SettingModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
    }

    /// <summary>
    /// Prepare custom HTML settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the custom HTML settings model
    /// </returns>
    protected virtual async Task<CustomHtmlSettingsModel> PrepareCustomHtmlSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

        //fill in model values from the entity
        var model = new CustomHtmlSettingsModel
        {
            HeaderCustomHtml = commonSettings.HeaderCustomHtml,
            FooterCustomHtml = commonSettings.FooterCustomHtml
        };

        //fill in overridden values
        if (storeId > 0)
        {
            model.HeaderCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.HeaderCustomHtml, storeId);
            model.FooterCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.FooterCustomHtml, storeId);
        }

        return model;
    }

    /// <summary>
    /// Prepare robots.txt settings model
    /// </summary>
    /// <param name="model">robots.txt model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the robots.txt settings model
    /// </returns>
    protected virtual async Task<RobotsTxtSettingsModel> PrepareRobotsTxtSettingsModelAsync(RobotsTxtSettingsModel model = null)
    {
        var additionsInstruction =
            string.Format(
                await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsInstruction"),
                RobotsTxtDefaults.RobotsAdditionsFileName);

        if (_fileProvider.FileExists(_fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsCustomFileName)))
            return new RobotsTxtSettingsModel { CustomFileExists = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsCustomFileExists"), RobotsTxtDefaults.RobotsCustomFileName), AdditionsInstruction = additionsInstruction };

        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var robotsTxtSettings = await _settingService.LoadSettingAsync<RobotsTxtSettings>(storeId);

        model ??= new RobotsTxtSettingsModel
        {
            AllowSitemapXml = robotsTxtSettings.AllowSitemapXml,
            DisallowPaths = string.Join(Environment.NewLine, robotsTxtSettings.DisallowPaths),
            LocalizableDisallowPaths =
                string.Join(Environment.NewLine, robotsTxtSettings.LocalizableDisallowPaths),
            DisallowLanguages = robotsTxtSettings.DisallowLanguages.ToList(),
            AdditionsRules = string.Join(Environment.NewLine, robotsTxtSettings.AdditionsRules),
            AvailableLanguages = new List<SelectListItem>()
        };

        if (!model.AvailableLanguages.Any())
        {
            (model.AvailableLanguages as List<SelectListItem>)?.AddRange((await _languageService.GetAllLanguagesAsync(storeId: storeId)).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }));
        }

        model.AdditionsInstruction = additionsInstruction;

        if (storeId <= 0)
            return model;

        model.AdditionsRules_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AdditionsRules, storeId);
        model.AllowSitemapXml_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AllowSitemapXml, storeId);
        model.DisallowLanguages_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowLanguages, storeId);
        model.DisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowPaths, storeId);
        model.LocalizableDisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.LocalizableDisallowPaths, storeId);

        return model;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare app settings model
    /// </summary>
    /// <param name="model">AppSettings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the app settings model
    /// </returns>
    public virtual async Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null)
    {
        model ??= new AppSettingsModel
        {
            CacheConfigModel = _appSettings.Get<CacheConfig>().ToConfigModel<CacheConfigModel>(),
            HostingConfigModel = _appSettings.Get<HostingConfig>().ToConfigModel<HostingConfigModel>(),
            DistributedCacheConfigModel = _appSettings.Get<DistributedCacheConfig>().ToConfigModel<DistributedCacheConfigModel>(),
            InstallationConfigModel = _appSettings.Get<InstallationConfig>().ToConfigModel<InstallationConfigModel>(),
            PluginConfigModel = _appSettings.Get<PluginConfig>().ToConfigModel<PluginConfigModel>(),
            CommonConfigModel = _appSettings.Get<CommonConfig>().ToConfigModel<CommonConfigModel>(),
            DataConfigModel = _appSettings.Get<DataConfig>().ToConfigModel<DataConfigModel>(),
            WebOptimizerConfigModel = _appSettings.Get<WebOptimizerConfig>().ToConfigModel<WebOptimizerConfigModel>(),
        };

        model.DistributedCacheConfigModel.DistributedCacheTypeValues = await _appSettings.Get<DistributedCacheConfig>().DistributedCacheType.ToSelectListAsync();

        model.DataConfigModel.DataProviderTypeValues = await _appSettings.Get<DataConfig>().DataProvider.ToSelectListAsync();

        //Since we decided to use the naming of the DB connections section as in the .net core - "ConnectionStrings",
        //we are forced to adjust our internal model naming to this convention in this check.
        model.EnvironmentVariables.AddRange(from property in model.GetType().GetProperties()
            where property.Name != nameof(AppSettingsModel.EnvironmentVariables)
            from pp in property.PropertyType.GetProperties()
            where Environment.GetEnvironmentVariables().Contains($"{property.Name.Replace("Model", "").Replace("DataConfig", "ConnectionStrings")}__{pp.Name}")
            select $"{property.Name}_{pp.Name}");
        return model;
    }

    /// <summary>
    /// Prepare blog settings model
    /// </summary>
    /// <param name="model">Blog settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog settings model
    /// </returns>
    public virtual async Task<BlogSettingsModel> PrepareBlogSettingsModelAsync(BlogSettingsModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var blogSettings = await _settingService.LoadSettingAsync<BlogSettings>(storeId);

        //fill in model values from the entity
        model ??= blogSettings.ToSettingsModel<BlogSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.Enabled, storeId);
        model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.PostsPageSize, storeId);
        model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
        model.NotifyAboutNewBlogComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NotifyAboutNewBlogComments, storeId);
        model.NumberOfTags_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NumberOfTags, storeId);
        model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.ShowHeaderRssUrl, storeId);
        model.BlogCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.BlogCommentsMustBeApproved, storeId);

        return model;
    }

    /// <summary>
    /// Prepare vendor settings model
    /// </summary>
    /// <param name="model">Vendor settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor settings model
    /// </returns>
    public virtual async Task<VendorSettingsModel> PrepareVendorSettingsModelAsync(VendorSettingsModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var vendorSettings = await _settingService.LoadSettingAsync<VendorSettings>(storeId);

        //fill in model values from the entity
        model ??= vendorSettings.ToSettingsModel<VendorSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;

        //fill in overridden values
        if (storeId > 0)
        {
            model.VendorsBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeId);
            model.ShowVendorOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeId);
            model.ShowVendorOnOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnOrderDetailsPage, storeId);
            model.AllowCustomersToContactVendors_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowCustomersToContactVendors, storeId);
            model.AllowCustomersToApplyForVendorAccount_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, storeId);
            model.TermsOfServiceEnabled_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.TermsOfServiceEnabled, storeId);
            model.AllowSearchByVendor_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowSearchByVendor, storeId);
            model.AllowVendorsToEditInfo_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToEditInfo, storeId);
            model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, storeId);
            model.MaximumProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.MaximumProductNumber, storeId);
            model.AllowVendorsToImportProducts_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToImportProducts, storeId);
        }

        //prepare nested search model
        await _vendorAttributeModelFactory.PrepareVendorAttributeSearchModelAsync(model.VendorAttributeSearchModel);

        return model;
    }

    /// <summary>
    /// Prepare forum settings model
    /// </summary>
    /// <param name="model">Forum settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum settings model
    /// </returns>
    public virtual async Task<ForumSettingsModel> PrepareForumSettingsModelAsync(ForumSettingsModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>(storeId);

        //fill in model values from the entity
        model ??= forumSettings.ToSettingsModel<ForumSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
        model.ForumEditorValues = await forumSettings.ForumEditor.ToSelectListAsync();

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.ForumsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumsEnabled, storeId);
        model.RelativeDateTimeFormattingEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeId);
        model.ShowCustomersPostCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowCustomersPostCount, storeId);
        model.AllowGuestsToCreatePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreatePosts, storeId);
        model.AllowGuestsToCreateTopics_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreateTopics, storeId);
        model.AllowCustomersToEditPosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToEditPosts, storeId);
        model.AllowCustomersToDeletePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToDeletePosts, storeId);
        model.AllowPostVoting_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPostVoting, storeId);
        model.MaxVotesPerDay_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.MaxVotesPerDay, storeId);
        model.AllowCustomersToManageSubscriptions_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeId);
        model.TopicsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.TopicsPageSize, storeId);
        model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.PostsPageSize, storeId);
        model.ForumEditor_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumEditor, storeId);
        model.SignaturesEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SignaturesEnabled, storeId);
        model.AllowPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPrivateMessages, storeId);
        model.ShowAlertForPM_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowAlertForPM, storeId);
        model.NotifyAboutPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.NotifyAboutPrivateMessages, storeId);
        model.ActiveDiscussionsFeedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeId);
        model.ActiveDiscussionsFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, storeId);
        model.ForumFeedsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedsEnabled, storeId);
        model.ForumFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedCount, storeId);
        model.SearchResultsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SearchResultsPageSize, storeId);
        model.ActiveDiscussionsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsPageSize, storeId);

        return model;
    }

    /// <summary>
    /// Prepare catalog settings model
    /// </summary>
    /// <param name="model">Catalog settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the catalog settings model
    /// </returns>
    public virtual async Task<CatalogSettingsModel> PrepareCatalogSettingsModelAsync(CatalogSettingsModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);

        //fill in model values from the entity
        model ??= catalogSettings.ToSettingsModel<CatalogSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
        model.AttributeValueOutOfStockDisplayTypes = await catalogSettings.AttributeValueOutOfStockDisplayType.ToSelectListAsync();
        model.ProductUrlStructureTypes = await ((ProductUrlStructureType)catalogSettings.ProductUrlStructureTypeId).ToSelectListAsync();
        model.AvailableViewModes.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.Grid"),
            Value = "grid"
        });
        model.AvailableViewModes.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.List"),
            Value = "list"
        });

        //fill in overridden values
        if (storeId > 0)
        {
            model.AllowViewUnpublishedProductPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeId);
            model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, storeId);
            model.ShowSkuOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnProductDetailsPage, storeId);
            model.ShowSkuOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnCatalogPages, storeId);
            model.ShowManufacturerPartNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowManufacturerPartNumber, storeId);
            model.ShowGtin_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowGtin, storeId);
            model.ShowFreeShippingNotification_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowFreeShippingNotification, storeId);
            model.ShowShortDescriptionOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowShortDescriptionOnCatalogPages, storeId);
            model.AllowProductSorting_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowProductSorting, storeId);
            model.AllowProductViewModeChanging_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowProductViewModeChanging, storeId);
            model.DefaultViewMode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DefaultViewMode, storeId);
            model.ShowProductsFromSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductsFromSubcategories, storeId);
            model.ShowCategoryProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumber, storeId);
            model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeId);
            model.CategoryBreadcrumbEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeId);
            model.ShowShareButton_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowShareButton, storeId);
            model.PageShareCode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.PageShareCode, storeId);
            model.ProductReviewsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsMustBeApproved, storeId);
            model.OneReviewPerProductFromCustomer_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.OneReviewPerProductFromCustomer, storeId);
            model.AllowAnonymousUsersToReviewProduct_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeId);
            model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewPossibleOnlyAfterPurchasing, storeId);
            model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeId);
            model.NotifyCustomerAboutProductReviewReply_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyCustomerAboutProductReviewReply, storeId);
            model.EmailAFriendEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EmailAFriendEnabled, storeId);
            model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeId);
            model.RecentlyViewedProductsNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsNumber, storeId);
            model.RecentlyViewedProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeId);
            model.NewProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsEnabled, storeId);
            model.NewProductsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsPageSize, storeId);
            model.NewProductsAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsAllowCustomersToSelectPageSize, storeId);
            model.NewProductsPageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsPageSizeOptions, storeId);
            model.CompareProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CompareProductsEnabled, storeId);
            model.ShowBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowBestsellersOnHomepage, storeId);
            model.NumberOfBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeId);
            model.SearchPageProductsPerPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageProductsPerPage, storeId);
            model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeId);
            model.ShowSearchBoxCategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSearchBoxCategories, storeId);
            model.SearchPagePriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceRangeFiltering, storeId);
            model.SearchPagePriceFrom_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceFrom, storeId);
            model.SearchPagePriceTo_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceTo, storeId);
            model.SearchPageManuallyPriceRange_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageManuallyPriceRange, storeId);
            model.SearchPagePageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePageSizeOptions, storeId);
            model.ProductSearchAutoCompleteEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeId);
            model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeId);
            model.ShowProductImagesInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeId);
            model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowLinkToAllResultInSearchAutoComplete, storeId);
            model.ProductSearchTermMinimumLength_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchTermMinimumLength, storeId);
            model.ProductsAlsoPurchasedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeId);
            model.ProductsAlsoPurchasedNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeId);
            model.NumberOfProductTags_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfProductTags, storeId);
            model.ProductsByTagPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSize, storeId);
            model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeId);
            model.ProductsByTagPageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeId);
            model.ProductsByTagPriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceRangeFiltering, storeId);
            model.ProductsByTagPriceFrom_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceFrom, storeId);
            model.ProductsByTagPriceTo_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceTo, storeId);
            model.ProductsByTagManuallyPriceRange_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagManuallyPriceRange, storeId);
            model.IncludeShortDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeId);
            model.IncludeFullDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeId);
            model.ManufacturersBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeId);
            model.DisplayTaxShippingInfoFooter_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeId);
            model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeId);
            model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeId);
            model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeId);
            model.DisplayTaxShippingInfoWishlist_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeId);
            model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeId);
            model.ShowProductReviewsPerStore_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsPerStore, storeId);
            model.ShowProductReviewsOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, storeId);
            model.ProductReviewsPageSizeOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsPageSizeOnAccountPage, storeId);
            model.ProductReviewsSortByCreatedDateAscending_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsSortByCreatedDateAscending, storeId);
            model.ExportImportProductAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductAttributes, storeId);
            model.ExportImportProductSpecificationAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductSpecificationAttributes, storeId);
            model.ExportImportProductCategoryBreadcrumb_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductCategoryBreadcrumb, storeId);
            model.ExportImportCategoriesUsingCategoryName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportCategoriesUsingCategoryName, storeId);
            model.ExportImportAllowDownloadImages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportAllowDownloadImages, storeId);
            model.ExportImportSplitProductsFile_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportSplitProductsFile, storeId);
            model.RemoveRequiredProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RemoveRequiredProducts, storeId);
            model.ExportImportRelatedEntitiesByName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportRelatedEntitiesByName, storeId);
            model.ExportImportProductUseLimitedToStores_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductUseLimitedToStores, storeId);
            model.ExportImportCategoryUseLimitedToStores_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportCategoryUseLimitedToStores, storeId);
            model.DisplayDatePreOrderAvailability_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDatePreOrderAvailability, storeId);
            model.UseAjaxCatalogProductsLoading_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.UseAjaxCatalogProductsLoading, storeId);
            model.EnableManufacturerFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnableManufacturerFiltering, storeId);
            model.EnablePriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnablePriceRangeFiltering, storeId);
            model.EnableSpecificationAttributeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnableSpecificationAttributeFiltering, storeId);
            model.DisplayFromPrices_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayFromPrices, storeId);
            model.AttributeValueOutOfStockDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AttributeValueOutOfStockDisplayType, storeId);
            model.AllowCustomersToSearchWithManufacturerName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowCustomersToSearchWithManufacturerName, storeId);
            model.AllowCustomersToSearchWithCategoryName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowCustomersToSearchWithCategoryName, storeId);
            model.DisplayAllPicturesOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayAllPicturesOnCatalogPages, storeId);
            model.ProductUrlStructureTypeId_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductUrlStructureTypeId, storeId);
        }

        //prepare nested search model
        await PrepareSortOptionSearchModelAsync(model.SortOptionSearchModel);
        await _reviewTypeModelFactory.PrepareReviewTypeSearchModelAsync(model.ReviewTypeSearchModel);

        await PrepareArtificialIntelligenceSettingsModelAsync(model.ArtificialIntelligenceSettingsModel);
        await PrepareGpsrSettingsModelAsync(model.GpsrSettingsModel);

        return model;
    }

    /// <summary>
    /// Prepare filter level settings model
    /// </summary>
    /// <param name="model">Filter level settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level settings model
    /// </returns>
    public virtual async Task<FilterLevelSettingsModel> PrepareFilterLevelSettingsModelAsync(FilterLevelSettingsModel model = null)
    {
        //load settings

        //fill in model values from the entity
        model ??= _filterLevelSettings.ToSettingsModel<FilterLevelSettingsModel>();

        //prepare nested search model
        await PrepareFilterLevelSearchModelAsync(model.FilterLevelSearchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged filter level list model
    /// </summary>
    /// <param name="searchModel">Filter level search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level list model
    /// </returns>
    public virtual async Task<FilterLevelListModel> PrepareFilterLevelListModelAsync(FilterLevelSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get filter levels
        var filterLevels = Enum.GetValues(typeof(FilterLevelEnum)).OfType<FilterLevelEnum>().ToList().ToPagedList(searchModel);

        //prepare list model
        var model = await new FilterLevelListModel().PrepareToGridAsync(searchModel, filterLevels, () =>
        {
            return filterLevels.SelectAwait(async filterLevel =>
            {
                //fill in model values from the entity
                var filterLevelModel = new FilterLevelModel { Id = (int)filterLevel };

                //fill in additional values (not existing in the entity)
                filterLevelModel.Name = await _localizationService.GetLocalizedEnumAsync(filterLevel);
                filterLevelModel.Enabled = !_filterLevelSettings.FilterLevelEnumDisabled.Contains((int)filterLevel);

                return filterLevelModel;
            }).OrderBy(filterLevel => filterLevel.Id);
        });

        return model;
    }

    /// <summary>
    /// Prepare filter level model
    /// </summary>
    /// <param name="model">Filter level model</param>
    /// <param name="filterLevel">Filter level</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level model
    /// </returns>
    public virtual async Task<FilterLevelModel> PrepareFilterLevelModelAsync(FilterLevelModel model, FilterLevelEnum filterLevel, bool excludeProperties = false)
    {
        Func<FilterLevelLocalizedModel, int, Task> localizedModelConfiguration = null;

        //fill in model values from settings
        model ??= new FilterLevelModel { Id = (int)filterLevel };
        model.Name = await _localizationService.GetLocalizedEnumAsync(filterLevel);
        model.Enabled = !_filterLevelSettings.FilterLevelEnumDisabled.Contains((int)filterLevel);

        //define localized model configuration action
        localizedModelConfiguration = async (locale, languageId) =>
        {
            var resourceName = $"Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.{filterLevel}";
            var resource = await _localizationService.GetLocaleStringResourceByNameAsync(resourceName, languageId, false);
            locale.Name = resource?.ResourceValue ?? string.Empty;
        };

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    /// <summary>
    /// Prepare paged sort option list model
    /// </summary>
    /// <param name="searchModel">Sort option search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sort option list model
    /// </returns>
    public virtual async Task<SortOptionListModel> PrepareSortOptionListModelAsync(SortOptionSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);

        //get sort options
        var sortOptions = Enum.GetValues(typeof(ProductSortingEnum)).OfType<ProductSortingEnum>().ToList().ToPagedList(searchModel);

        //prepare list model
        var model = await new SortOptionListModel().PrepareToGridAsync(searchModel, sortOptions, () =>
        {
            return sortOptions.SelectAwait(async option =>
            {
                //fill in model values from the entity
                var sortOptionModel = new SortOptionModel { Id = (int)option };

                //fill in additional values (not existing in the entity)
                sortOptionModel.Name = await _localizationService.GetLocalizedEnumAsync(option);
                sortOptionModel.IsActive = !catalogSettings.ProductSortingEnumDisabled.Contains((int)option);
                sortOptionModel.DisplayOrder = catalogSettings
                    .ProductSortingEnumDisplayOrder.TryGetValue((int)option, out var value) ? value : (int)option;

                return sortOptionModel;
            }).OrderBy(option => option.DisplayOrder);
        });

        return model;
    }

    /// <summary>
    /// Prepare media settings model
    /// </summary>
    /// <param name="model">Media settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the media settings model
    /// </returns>
    public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeId);

        //fill in model values from the entity
        model ??= mediaSettings.ToSettingsModel<MediaSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
        model.PicturesStoredIntoDatabase = await _pictureService.IsStoreInDbAsync();

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.AvatarPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AvatarPictureSize, storeId);
        model.ProductThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSize, storeId);
        model.ProductDetailsPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductDetailsPictureSize, storeId);
        model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeId);
        model.AssociatedProductPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AssociatedProductPictureSize, storeId);
        model.CategoryThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CategoryThumbPictureSize, storeId);
        model.ManufacturerThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, storeId);
        model.VendorThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.VendorThumbPictureSize, storeId);
        model.CartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CartThumbPictureSize, storeId);
        model.OrderThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.OrderThumbPictureSize, storeId);
        model.MiniCartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MiniCartThumbPictureSize, storeId);
        model.MaximumImageSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MaximumImageSize, storeId);
        model.MultipleThumbDirectories_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MultipleThumbDirectories, storeId);
        model.DefaultImageQuality_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultImageQuality, storeId);
        model.ImportProductImagesUsingHash_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ImportProductImagesUsingHash, storeId);
        model.DefaultPictureZoomEnabled_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, storeId);
        model.AllowSvgUploads_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AllowSvgUploads, storeId);
        model.ProductDefaultImageId_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductDefaultImageId, storeId);

        return model;
    }

    /// <summary>
    /// Prepare customer user settings model
    /// </summary>
    /// <param name="model">Customer user settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer user settings model
    /// </returns>
    public virtual async Task<CustomerUserSettingsModel> PrepareCustomerUserSettingsModelAsync(CustomerUserSettingsModel model = null)
    {
        model ??= new CustomerUserSettingsModel
        {
            ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        //prepare customer settings model
        model.CustomerSettings = await PrepareCustomerSettingsModelAsync();

        //prepare CustomerSettings list availableCountries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.CustomerSettings.AvailableCountries);

        //prepare multi-factor authentication settings model
        model.MultiFactorAuthenticationSettings = await PrepareMultiFactorAuthenticationSettingsModelAsync();

        //prepare address settings model
        model.AddressSettings = await PrepareAddressSettingsModelAsync();

        //prepare AddressSettings list availableCountries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.AddressSettings.AvailableCountries);

        //prepare date time settings model
        model.DateTimeSettings = await PrepareDateTimeSettingsModelAsync();

        //prepare external authentication settings model
        model.ExternalAuthenticationSettings = await PrepareExternalAuthenticationSettingsModelAsync();

        //prepare nested search models
        await _customerAttributeModelFactory.PrepareCustomerAttributeSearchModelAsync(model.CustomerAttributeSearchModel);
        await _addressAttributeModelFactory.PrepareAddressAttributeSearchModelAsync(model.AddressAttributeSearchModel);

        return model;
    }

    /// <summary>
    /// Prepare GDPR settings model
    /// </summary>
    /// <param name="model">Gdpr settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR settings model
    /// </returns>
    public virtual async Task<GdprSettingsModel> PrepareGdprSettingsModelAsync(GdprSettingsModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var gdprSettings = await _settingService.LoadSettingAsync<GdprSettings>(storeId);

        //fill in model values from the entity
        model ??= gdprSettings.ToSettingsModel<GdprSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;

        //prepare nested search model
        await PrepareGdprConsentSearchModelAsync(model.GdprConsentSearchModel);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.GdprEnabled_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.GdprEnabled, storeId);
        model.LogPrivacyPolicyConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogPrivacyPolicyConsent, storeId);
        model.LogNewsLetterConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogNewsletterConsent, storeId);
        model.LogUserProfileChanges_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogUserProfileChanges, storeId);
        model.DeleteInactiveCustomersAfterMonths_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.DeleteInactiveCustomersAfterMonths, storeId);

        return model;
    }

    /// <summary>
    /// Prepare paged GDPR consent list model
    /// </summary>
    /// <param name="searchModel">GDPR search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent list model
    /// </returns>
    public virtual async Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get sort options
        var consentList = (await _gdprService.GetAllConsentsAsync()).ToPagedList(searchModel);

        //prepare list model
        var model = await new GdprConsentListModel().PrepareToGridAsync(searchModel, consentList, () =>
        {
            return consentList.SelectAwait(async consent =>
            {
                var gdprConsentModel = consent.ToModel<GdprConsentModel>();

                var gdprConsent = await _gdprService.GetConsentByIdAsync(gdprConsentModel.Id);
                gdprConsentModel.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message);
                gdprConsentModel.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage);

                return gdprConsentModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare GDPR consent model
    /// </summary>
    /// <param name="model">GDPR consent model</param>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent model
    /// </returns>
    public virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false)
    {
        Func<GdprConsentLocalizedModel, int, Task> localizedModelConfiguration = null;

        //fill in model values from the entity
        if (gdprConsent != null)
        {
            model ??= gdprConsent.ToModel<GdprConsentModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message, languageId, false, false);
                locale.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage, languageId, false, false);
            };
        }

        //set default values for the new model
        if (gdprConsent == null)
            model.DisplayOrder = 1;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    /// <summary>
    /// Prepare general and common settings model
    /// </summary>
    /// <param name="model">General common settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the general and common settings model
    /// </returns>
    public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null)
    {
        model ??= new GeneralCommonSettingsModel
        {
            ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        //prepare store information settings model
        model.StoreInformationSettings = await PrepareStoreInformationSettingsModelAsync();

        //prepare Sitemap settings model
        model.SitemapSettings = await PrepareSitemapSettingsModelAsync();

        //prepare Minification settings model
        model.MinificationSettings = await PrepareMinificationSettingsModelAsync();

        //prepare SEO settings model
        model.SeoSettings = await PrepareSeoSettingsModelAsync();

        //prepare security settings model
        model.SecuritySettings = await PrepareSecuritySettingsModelAsync();

        //prepare robots.txt settings model
        model.RobotsTxtSettings = await PrepareRobotsTxtSettingsModelAsync();

        //prepare captcha settings model
        model.CaptchaSettings = await PrepareCaptchaSettingsModelAsync();

        //prepare PDF settings model
        model.PdfSettings = await PreparePdfSettingsModelAsync();

        //prepare localization settings model
        model.LocalizationSettings = await PrepareLocalizationSettingsModelAsync();

        //prepare translation settings model
        model.TranslationSettings = await PrepareTranslationSettingsModelAsync();

        //prepare admin area settings model
        model.AdminAreaSettings = await PrepareAdminAreaSettingsModelAsync();

        //prepare custom HTML settings model
        model.CustomHtmlSettings = await PrepareCustomHtmlSettingsModelAsync();

        return model;
    }

    /// <summary>
    /// Prepare product editor settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product editor settings model
    /// </returns>
    public virtual async Task<ProductEditorSettingsModel> PrepareProductEditorSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var productEditorSettings = await _settingService.LoadSettingAsync<ProductEditorSettings>(storeId);

        //fill in model values from the entity
        var model = productEditorSettings.ToSettingsModel<ProductEditorSettingsModel>();

        return model;
    }

    /// <summary>
    /// Prepare setting search model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting search model
    /// </returns>
    public virtual async Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare model to add
        await PrepareAddSettingModelAsync(searchModel.AddSetting);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged setting list model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting list model
    /// </returns>
    public virtual async Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get settings
        var settings = (await _settingService.GetAllSettingsAsync()).AsQueryable();

        //filter settings
        if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
            settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
        if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
            settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

        var pagedSettings = settings.ToList().ToPagedList(searchModel);

        //prepare list model
        var model = await new SettingListModel().PrepareToGridAsync(searchModel, pagedSettings, () =>
        {
            return pagedSettings.SelectAwait(async setting =>
            {
                //fill in model values from the entity
                var settingModel = setting.ToModel<SettingModel>();

                //fill in additional values (not existing in the entity)
                settingModel.Store = setting.StoreId > 0
                    ? (await _storeService.GetStoreByIdAsync(setting.StoreId))?.Name ?? "Deleted"
                    : await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");

                return settingModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare setting mode model
    /// </summary>
    /// <param name="modeName">Mode name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting mode model
    /// </returns>
    public virtual async Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName)
    {
        var model = new SettingModeModel
        {
            ModeName = modeName,
            Enabled = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), modeName)
        };

        return model;
    }

    /// <summary>
    /// Prepare store scope configuration model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store scope configuration model
    /// </returns>
    public virtual async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync()
    {
        var model = new StoreScopeConfigurationModel
        {
            Stores = (await _storeService.GetAllStoresAsync()).Select(store => store.ToModel<StoreModel>()).ToList(),
            StoreId = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        return model;
    }

    #endregion
}
