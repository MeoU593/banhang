using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Attributes;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CustomerController : BasePublicController
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly GdprSettings _gdprSettings;
    protected readonly HtmlEncoder _htmlEncoder;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerMilitaryProfileService _customerMilitaryProfileService;
    protected readonly ICustomerModelFactory _customerModelFactory;
    protected readonly ICustomerRegistrationService _customerRegistrationService;
    protected readonly ICustomerService _customerService;
    protected readonly IDownloadService _downloadService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IExportManager _exportManager;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxService _taxService;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly MediaSettings _mediaSettings;
    protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;
    protected readonly StoreInformationSettings _storeInformationSettings;
    protected readonly TaxSettings _taxSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public CustomerController(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        ForumSettings forumSettings,
        GdprSettings gdprSettings,
        HtmlEncoder htmlEncoder,
        INopDataProvider dataProvider,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        IAuthenticationService authenticationService,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ICustomerMilitaryProfileService customerMilitaryProfileService,
        ICustomerModelFactory customerModelFactory,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IDownloadService downloadService,
        IEventPublisher eventPublisher,
        IExportManager exportManager,
        IExternalAuthenticationService externalAuthenticationService,
        IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        IGiftCardService giftCardService,
        ILocalizationService localizationService,
        ILogger logger,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        INotificationService notificationService,
        IOrderService orderService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        ITaxService taxService,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        MediaSettings mediaSettings,
        MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
        StoreInformationSettings storeInformationSettings,
        TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _forumSettings = forumSettings;
        _gdprSettings = gdprSettings;
        _htmlEncoder = htmlEncoder;
        _dataProvider = dataProvider;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _addressAttributeParser = addressAttributeParser;
        _customerAttributeParser = customerAttributeParser;
        _customerAttributeService = customerAttributeService;
        _authenticationService = authenticationService;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _customerMilitaryProfileService = customerMilitaryProfileService;
        _customerModelFactory = customerModelFactory;
        _customerRegistrationService = customerRegistrationService;
        _customerService = customerService;
        _downloadService = downloadService;
        _eventPublisher = eventPublisher;
        _exportManager = exportManager;
        _externalAuthenticationService = externalAuthenticationService;
        _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _giftCardService = giftCardService;
        _localizationService = localizationService;
        _logger = logger;
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
        _notificationService = notificationService;
        _orderService = orderService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _taxService = taxService;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
        _mediaSettings = mediaSettings;
        _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
        _storeInformationSettings = storeInformationSettings;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    protected virtual void ValidateRequiredConsents(List<GdprConsent> consents, IFormCollection form)
    {
        foreach (var consent in consents)
        {
            var controlId = $"consent{consent.Id}";
            var cbConsent = form[controlId];
            if (StringValues.IsNullOrEmpty(cbConsent) || !cbConsent.ToString().Equals("on"))
                ModelState.AddModelError("", consent.RequiredMessage);
        }
    }

    protected virtual async Task<string> ParseSelectedProviderAsync(IFormCollection form)
    {
        ArgumentNullException.ThrowIfNull(form);

        if (!form.TryGetValue("mfa_provider", out var curProvider) || StringValues.IsNullOrEmpty(curProvider))
            return string.Empty;

        var selectedProvider = curProvider.ToString();
        if (string.IsNullOrEmpty(selectedProvider))
            return string.Empty;

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var multiFactorAuthenticationProviders = await _multiFactorAuthenticationPluginManager
            .LoadActivePluginsAsync(customer, store.Id);
        
        var isValidProvider = multiFactorAuthenticationProviders
            .Any(p => p.PluginDescriptor.SystemName.Equals(selectedProvider, StringComparison.InvariantCultureIgnoreCase));

        return isValidProvider ? selectedProvider : string.Empty;
    }

    protected virtual async Task<string> ParseCustomCustomerAttributesAsync(IFormCollection form)
    {
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = "";
        var attributes = await _customerAttributeService.GetAllAttributesAsync();
        foreach (var attribute in attributes)
        {
            var controlId = $"{NopCustomerServicesDefaults.CustomerAttributePrefix}{attribute.Id}";
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var selectedAttributeId = int.Parse(ctrlAttributes);
                        if (selectedAttributeId > 0)
                        {
                            attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }
                    }
                }
                    break;
                case AttributeControlType.Checkboxes:
                {
                    var cblAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cblAttributes))
                    {
                        foreach (var item in cblAttributes.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var selectedAttributeId = int.Parse(item);
                            if (selectedAttributeId > 0)
                            {
                                attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //load read-only (already server-side selected) values
                    var attributeValues = await _customerAttributeService.GetAttributeValuesAsync(attribute.Id);
                    foreach (var selectedAttributeId in attributeValues
                                 .Where(v => v.IsPreSelected)
                                 .Select(v => v.Id)
                                 .ToList())
                    {
                        attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                    }
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var enteredText = ctrlAttributes.ToString().Trim();
                        attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                            attribute, enteredText);
                    }
                }
                    break;
                case AttributeControlType.Datepicker:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.FileUpload:
                //not supported customer attributes
                default:
                    break;
            }
        }

        return attributesXml;
    }

    protected virtual async Task LogGdprAsync(Customer customer, CustomerInfoModel oldCustomerInfoModel,
        CustomerInfoModel newCustomerInfoModel, IFormCollection form)
    {
        try
        {
            //consents
            var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
            foreach (var consent in consents)
            {
                var previousConsentValue = await _gdprService.IsConsentAcceptedAsync(consent.Id, customer.Id);
                var controlId = $"consent{consent.Id}";
                var cbConsent = form[controlId];
                if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                {
                    //agree
                    if (!previousConsentValue.HasValue || !previousConsentValue.Value)
                        await _gdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                }
                else
                {
                    //disagree
                    if (!previousConsentValue.HasValue || previousConsentValue.Value)
                        await _gdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                }
            }

            //user profile changes
            if (!_gdprSettings.LogUserProfileChanges)
                return;

            if (oldCustomerInfoModel.Gender != newCustomerInfoModel.Gender)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Gender")} = {newCustomerInfoModel.Gender}");

            if (oldCustomerInfoModel.FirstName != newCustomerInfoModel.FirstName)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.FirstName")} = {newCustomerInfoModel.FirstName}");

            if (oldCustomerInfoModel.LastName != newCustomerInfoModel.LastName)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.LastName")} = {newCustomerInfoModel.LastName}");

            if (oldCustomerInfoModel.ParseDateOfBirth() != newCustomerInfoModel.ParseDateOfBirth())
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.DateOfBirth")} = {newCustomerInfoModel.ParseDateOfBirth()}");

            if (oldCustomerInfoModel.Email != newCustomerInfoModel.Email)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Email")} = {newCustomerInfoModel.Email}");

            if (oldCustomerInfoModel.Company != newCustomerInfoModel.Company)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Company")} = {newCustomerInfoModel.Company}");

            if (oldCustomerInfoModel.StreetAddress != newCustomerInfoModel.StreetAddress)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.StreetAddress")} = {newCustomerInfoModel.StreetAddress}");

            if (oldCustomerInfoModel.StreetAddress2 != newCustomerInfoModel.StreetAddress2)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.StreetAddress2")} = {newCustomerInfoModel.StreetAddress2}");

            if (oldCustomerInfoModel.ZipPostalCode != newCustomerInfoModel.ZipPostalCode)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.ZipPostalCode")} = {newCustomerInfoModel.ZipPostalCode}");

            if (oldCustomerInfoModel.City != newCustomerInfoModel.City)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.City")} = {newCustomerInfoModel.City}");

            if (oldCustomerInfoModel.County != newCustomerInfoModel.County)
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.County")} = {newCustomerInfoModel.County}");

            if (oldCustomerInfoModel.CountryId != newCustomerInfoModel.CountryId)
            {
                var countryName = (await _countryService.GetCountryByIdAsync(newCustomerInfoModel.CountryId))?.Name;
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.Country")} = {countryName}");
            }

            if (oldCustomerInfoModel.StateProvinceId != newCustomerInfoModel.StateProvinceId)
            {
                var stateProvinceName = (await _stateProvinceService.GetStateProvinceByIdAsync(newCustomerInfoModel.StateProvinceId))?.Name;
                await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await _localizationService.GetResourceAsync("Account.Fields.StateProvince")} = {stateProvinceName}");
            }
        }
        catch (Exception exception)
        {
            await _logger.ErrorAsync(exception.Message, exception, customer);
        }
    }

    protected virtual async Task SaveCustomerMilitaryProfileAsync(Customer customer, CustomerInfoModel model)
    {
        var now = DateTime.UtcNow;
        var militaryProfile = await _customerMilitaryProfileService.GetByCustomerIdAsync(customer.Id);
        if (militaryProfile == null)
        {
            militaryProfile = new CustomerMilitaryProfile
            {
                CustomerId = customer.Id,
                CreatedOnUtc = now
            };

            PopulateMilitaryProfile(militaryProfile, model, now);
            await _customerMilitaryProfileService.InsertAsync(militaryProfile);
            return;
        }

        PopulateMilitaryProfile(militaryProfile, model, now);
        await _customerMilitaryProfileService.UpdateAsync(militaryProfile);
    }

    protected virtual void PopulateMilitaryProfile(CustomerMilitaryProfile militaryProfile, CustomerInfoModel model, DateTime updatedOnUtc)
    {
        militaryProfile.MilitaryCode = model.MilitaryCode?.Trim();
        militaryProfile.Rank = model.Rank?.Trim();
        militaryProfile.UnitName = model.UnitName?.Trim();
        militaryProfile.PositionTitle = model.PositionTitle?.Trim();
        militaryProfile.EnlistmentDate = model.EnlistmentDate;
        militaryProfile.UpdatedOnUtc = updatedOnUtc;
    }

    protected virtual async Task PrepareRecentActivitiesAsync(CustomerInfoModel model, int customerId)
    {
        var activityTypes = await _customerActivityService.GetAllActivityTypesAsync();
        var activityTypeMap = activityTypes.ToDictionary(activityType => activityType.Id, activityType => activityType.Name);

        var activities = await _customerActivityService.GetAllActivitiesAsync(customerId: customerId, pageIndex: 0, pageSize: 10);
        foreach (var activity in activities)
        {
            model.RecentActivities.Add(new CustomerInfoModel.RecentActivityModel
            {
                ActivityTypeName = activityTypeMap.TryGetValue(activity.ActivityLogTypeId, out var activityTypeName)
                    ? activityTypeName
                    : string.IsNullOrWhiteSpace(activity.EntityName) ? "Hoạt động hệ thống" : activity.EntityName,
                Comment = activity.Comment,
                EntityName = activity.EntityName,
                IpAddress = activity.IpAddress,
                CreatedOnUtc = activity.CreatedOnUtc
            });
        }
    }

    protected virtual async Task PreparePostedDocumentsAsync(CustomerInfoModel model, int customerId, int documentPage = 1)
    {
        const int pageSize = 10;
        var page = Math.Max(1, documentPage);

        model.PostedDocumentsPage = page;
        model.PostedDocumentsPageSize = pageSize;

        try
        {
            const string countSql = "SELECT COUNT(1) AS TotalCount FROM Document WHERE UploadedByCustomerId = @customerId AND Deleted = 0";
            var countResult = await _dataProvider.QueryAsync<DocumentCountRow>(countSql, new DataParameter("customerId", customerId));
            model.PostedDocumentsTotalCount = countResult.FirstOrDefault()?.TotalCount ?? 0;
            model.PostedDocumentsTotalPages = model.PostedDocumentsTotalCount == 0
                ? 0
                : (int)Math.Ceiling(model.PostedDocumentsTotalCount / (double)pageSize);

            if (model.PostedDocumentsTotalPages > 0 && page > model.PostedDocumentsTotalPages)
            {
                page = model.PostedDocumentsTotalPages;
                model.PostedDocumentsPage = page;
            }

            const string sql = @"SELECT Id, Title, Code, Slug, Summary, AccessScopeId, DownloadId, Published, CreatedOnUtc, UpdatedOnUtc
FROM Document
WHERE UploadedByCustomerId = @customerId AND Deleted = 0
ORDER BY UpdatedOnUtc DESC, CreatedOnUtc DESC
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            var documents = await _dataProvider.QueryAsync<DocumentRow>(sql,
                new DataParameter("customerId", customerId),
                new DataParameter("offset", (page - 1) * pageSize),
                new DataParameter("pageSize", pageSize));

            foreach (var document in documents)
            {
                model.PostedDocuments.Add(new CustomerInfoModel.PostedDocumentModel
                {
                    Id = document.Id,
                    Title = document.Title,
                    Code = document.Code,
                    Slug = document.Slug,
                    Summary = document.Summary,
                    AccessScopeId = document.AccessScopeId,
                    AccessScopeName = document.AccessScopeId == 5 ? "Công khai" : "Nội bộ",
                    HasDownload = document.DownloadId.HasValue,
                    Published = document.Published,
                    CreatedOnUtc = document.CreatedOnUtc,
                    UpdatedOnUtc = document.UpdatedOnUtc,
                    CanView = document.Published && !string.IsNullOrWhiteSpace(document.Slug),
                    CanEdit = true,
                    CanDelete = true
                });
            }
        }
        catch
        {
            //document portal plugin may be disabled or not installed
        }
    }

    protected partial record DocumentCountRow
    {
        public int TotalCount { get; set; }
    }

    protected partial record DocumentRow
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Slug { get; set; }

        public string Summary { get; set; }

        public int AccessScopeId { get; set; }

        public int? DownloadId { get; set; }

        public bool Published { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }
    }

    #endregion

    #region Methods

    #region Login / logout

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Login(bool? checkoutAsGuest)
    {
        var model = await _customerModelFactory.PrepareLoginModelAsync(checkoutAsGuest);
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsRegisteredAsync(customer))
        {
            var fullName = await _customerService.GetCustomerFullNameAsync(customer);
            var message = await _localizationService.GetResourceAsync("Account.Login.AlreadyLogin");
            _notificationService.SuccessNotification(string.Format(message, _htmlEncoder.Encode(fullName)));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Login(LoginModel model, string returnUrl, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            var customerUserName = model.Username;
            var customerEmail = model.Email;
            var userNameOrEmail = _customerSettings.UsernamesEnabled ? customerUserName : customerEmail;

            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(userNameOrEmail, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                {
                    var customer = _customerSettings.UsernamesEnabled
                        ? await _customerService.GetCustomerByUsernameAsync(customerUserName)
                        : await _customerService.GetCustomerByEmailAsync(customerEmail);

                    return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, model.RememberMe);
                }
                case CustomerLoginResults.MultiFactorAuthenticationRequired:
                {
                    var customerMultiFactorAuthenticationInfo = new CustomerMultiFactorAuthenticationInfo
                    {
                        UserName = userNameOrEmail,
                        RememberMe = model.RememberMe,
                        ReturnUrl = returnUrl
                    };
                    await HttpContext.Session.SetAsync(
                        NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo,
                        customerMultiFactorAuthenticationInfo);
                    return RedirectToRoute(NopRouteNames.Standard.MULTIFACTOR_VERIFICATION);
                }
                case CustomerLoginResults.CustomerNotExist:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                    break;
                case CustomerLoginResults.Deleted:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                    break;
                case CustomerLoginResults.NotActive:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                    break;
                case CustomerLoginResults.NotRegistered:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                    break;
                case CustomerLoginResults.LockedOut:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                    break;
                case CustomerLoginResults.WrongPassword:
                default:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
                    break;
            }

            //if (loginResult == CustomerLoginResults.WrongPassword && _customerSettings.NotifyFailedLoginAttempt)
            //{
            //    var customer = _customerSettings.UsernamesEnabled
            //            ? await _customerService.GetCustomerByUsernameAsync(customerUserName)
            //            : await _customerService.GetCustomerByEmailAsync(customerEmail);
            //
            //    await _workflowMessageService.SendCustomerFailedLoginAttemptNotificationAsync(customer, customer.LanguageId ?? 0);
            //}

            await _customerActivityService.InsertActivityAsync("PublicStore.FailedLogin",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login.Fail"), _customerSettings.UsernamesEnabled ? customerUserName : customerEmail));
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareLoginModelAsync(model.CheckoutAsGuest);
        return View(model);
    }

    /// <summary>
    /// The entry point for injecting a plugin component of type "MultiFactorAuth"
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the user verification page for Multi-factor authentication. Served by an authentication provider.
    /// </returns>
    [CheckAccessClosedStore(ignore: true)]
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> MultiFactorVerification()
    {
        if (!await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            return RedirectToRoute(NopRouteNames.General.LOGIN);

        var customerMultiFactorAuthenticationInfo = await HttpContext.Session.GetAsync<CustomerMultiFactorAuthenticationInfo>(
            NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo);
        var userName = customerMultiFactorAuthenticationInfo?.UserName;
        if (string.IsNullOrEmpty(userName))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = _customerSettings.UsernamesEnabled ? await _customerService.GetCustomerByUsernameAsync(userName) : await _customerService.GetCustomerByEmailAsync(userName);
        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION, customer))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
        if (string.IsNullOrEmpty(selectedProvider))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new MultiFactorAuthenticationProviderModel();
        model = await _customerModelFactory.PrepareMultiFactorAuthenticationProviderModelAsync(model, selectedProvider, true);

        return View(model);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Logout()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (_workContext.OriginalCustomerIfImpersonated != null)
        {
            //activity log
            await _customerActivityService.InsertActivityAsync(_workContext.OriginalCustomerIfImpersonated, "Impersonation.Finished",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.StoreOwner"),
                    customer.Email, customer.Id),
                customer);

            await _customerActivityService.InsertActivityAsync("Impersonation.Finished",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.Customer"),
                    _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                _workContext.OriginalCustomerIfImpersonated);

            //logout impersonated customer
            await _genericAttributeService
                .SaveAttributeAsync<int?>(_workContext.OriginalCustomerIfImpersonated, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);

            //redirect back to customer details page (admin area)
            return RedirectToAction("Edit", "Customer", new { id = customer.Id, area = AreaNames.ADMIN });
        }

        //activity log
        await _customerActivityService.InsertActivityAsync(customer, "PublicStore.Logout",
            await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Logout"), customer);

        //standard logout 
        await _authenticationService.SignOutAsync();

        //raise logged out event       
        await _eventPublisher.PublishAsync(new CustomerLoggedOutEvent(customer));

        //EU Cookie
        if (_storeInformationSettings.DisplayEuCookieLawWarning)
        {
            //the cookie law message should not pop up immediately after logout.
            //otherwise, the user will have to click it again...
            //and thus next visitor will not click it... so violation for that cookie law..
            //the only good solution in this case is to store a temporary variable
            //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
            //but it'll be displayed for further page loads
            TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"] = true;
        }

        return RedirectToRoute(NopRouteNames.General.LOGIN);
    }

    #endregion

    #region Password recovery

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecovery()
    {
        var model = new PasswordRecoveryModel();
        model = await _customerModelFactory.PreparePasswordRecoveryModelAsync(model);

        return View(model);
    }

    [ValidateCaptcha]
    [HttpPost, ActionName("PasswordRecovery")]
    [FormValueRequired("send-email")]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecoverySend(PasswordRecoveryModel model, bool captchaValid)
    {
        // validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            var customer = await _customerService.GetCustomerByEmailAsync(model.Email);
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                var passwordRecoveryToken = Guid.NewGuid();
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                    passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                await _genericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                ////send email
                //await _workflowMessageService.SendCustomerPasswordRecoveryMessageAsync(customer,
                //    (await _workContext.GetWorkingLanguageAsync()).Id);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailHasBeenSent"));
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailNotFound"));
            }
        }

        model = await _customerModelFactory.PreparePasswordRecoveryModelAsync(model);

        return View(model);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecoveryConfirm(string token, string email, Guid guid)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new PasswordRecoveryConfirmModel { ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE) };
        if (string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute)))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged");
            return View(model);
        }

        //validate token
        if (!await _customerService.IsPasswordRecoveryTokenValidAsync(customer, token))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
            return View(model);
        }

        //validate token expiration date
        if (await _customerService.IsPasswordRecoveryLinkExpiredAsync(customer))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
            return View(model);
        }

        return View(model);
    }

    [HttpPost, ActionName("PasswordRecoveryConfirm")]
    [FormValueRequired("set-password")]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecoveryConfirmPOST(string token, string email, Guid guid, PasswordRecoveryConfirmModel model)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        model.ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        //validate token
        if (!await _customerService.IsPasswordRecoveryTokenValidAsync(customer, token))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
            return View(model);
        }

        //validate token expiration date
        if (await _customerService.IsPasswordRecoveryLinkExpiredAsync(customer))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
            return View(model);
        }

        if (!ModelState.IsValid)
            return View(model);

        var response = await _customerRegistrationService
            .ChangePasswordAsync(new ChangePasswordRequest(customer.Email, false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
        if (!response.Success)
        {
            model.Result = string.Join(';', response.Errors);
            return View(model);
        }

        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, "");

        await _customerActivityService.InsertActivityAsync(customer, "PublicStore.PasswordChanged", await
            _localizationService.GetResourceAsync("ActivityLog.PublicStore.PasswordChanged"));

        //authenticate customer after changing password
        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

        model.DisablePasswordChanging = true;
        model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.PasswordHasBeenChanged");
        return View(model);
    }

    #endregion     

    #region Register

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Register(string returnUrl)
    {
        var model = new RegisterModel();
        model = await _customerModelFactory.PrepareRegisterModelAsync(model, false, setDefaultValues: true);

        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    [ValidateHoneypot]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Register(RegisterModel model, string returnUrl, bool captchaValid, IFormCollection form)
    {
        const UserRegistrationType registrationType = UserRegistrationType.AdminApproval;

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var language = await _workContext.GetWorkingLanguageAsync();

        if (await _customerService.IsRegisteredAsync(customer))
        {
            //Already registered customer. 
            await _authenticationService.SignOutAsync();

            //raise logged out event       
            await _eventPublisher.PublishAsync(new CustomerLoggedOutEvent(customer));

            customer = await _customerService.InsertGuestCustomerAsync();

            //Save a new record
            await _workContext.SetCurrentCustomerAsync(customer);
        }

        customer.RegisteredInStoreId = store.Id;

        //custom customer attributes
        var customerAttributesXml = await ParseCustomCustomerAttributesAsync(form);
        var customerAttributeWarnings = await _customerAttributeParser.GetAttributeWarningsAsync(customerAttributesXml);
        foreach (var error in customerAttributeWarnings)
            ModelState.AddModelError("", error);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        //GDPR
        if (_gdprSettings.GdprEnabled)
        {
            var consents = (await _gdprService
                .GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration && consent.IsRequired).ToList();

            ValidateRequiredConsents(consents, form);
        }

        if (ModelState.IsValid)
        {
            var customerUserName = model.Username;
            var customerEmail = model.Email;

            var isApproved = registrationType == UserRegistrationType.Standard;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                _customerSettings.UsernamesEnabled ? customerUserName : customerEmail,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                store.Id,
                isApproved);
            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //properties
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    customer.TimeZoneId = model.TimeZoneId;

                //VAT number
                if (_taxSettings.EuVatEnabled)
                {
                    customer.VatNumber = model.VatNumber;

                    var (vatNumberStatus, _, vatAddress) = await _taxService.GetVatNumberStatusAsync(model.VatNumber);
                    customer.VatNumberStatusId = (int)vatNumberStatus;
                    ////send VAT number admin notification
                    //if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                    //    await _workflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                }

                //form fields
                if (_customerSettings.GenderEnabled)
                    customer.Gender = model.Gender;
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.DateOfBirthEnabled)
                    customer.DateOfBirth = model.ParseDateOfBirth();
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.StreetAddress;
                if (_customerSettings.StreetAddress2Enabled)
                    customer.StreetAddress2 = model.StreetAddress2;
                if (_customerSettings.ZipPostalCodeEnabled)
                    customer.ZipPostalCode = model.ZipPostalCode;
                if (_customerSettings.CityEnabled)
                    customer.City = model.City;
                if (_customerSettings.CountyEnabled)
                    customer.County = model.County;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                if (_customerSettings.FaxEnabled)
                    customer.Fax = model.Fax;

                //save customer attributes
                customer.CustomCustomerAttributesXML = customerAttributesXml;
                await _customerService.UpdateCustomerAsync(customer);

                if (_customerSettings.AcceptPrivacyPolicyEnabled)
                {
                    //privacy policy is required
                    //GDPR
                    if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                        await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ConsentAgree, await _localizationService.GetResourceAsync("Gdpr.Consent.PrivacyPolicy"));
                }

                //GDPR
                if (_gdprSettings.GdprEnabled)
                {
                    var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
                    foreach (var consent in consents)
                    {
                        var controlId = $"consent{consent.Id}";
                        var cbConsent = form[controlId];
                        if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                        {
                            //agree
                            await _gdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                        }
                        else
                        {
                            //disagree
                            await _gdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                        }
                    }
                }

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                        ? (int?)customer.CountryId
                        : null,
                    StateProvinceId = customer.StateProvinceId > 0
                        ? (int?)customer.StateProvinceId
                        : null,
                    County = customer.County,
                    City = customer.City,
                    Address1 = customer.StreetAddress,
                    Address2 = customer.StreetAddress2,
                    ZipPostalCode = customer.ZipPostalCode,
                    PhoneNumber = customer.Phone,
                    FaxNumber = customer.Fax,
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (await _addressService.IsAddressValidAsync(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    //customer.Addresses.Add(defaultAddress);

                    await _addressService.InsertAddressAsync(defaultAddress);

                    await _customerService.InsertCustomerAddressAsync(customer, defaultAddress);

                    customer.BillingAddressId = defaultAddress.Id;
                    customer.ShippingAddressId = defaultAddress.Id;

                    await _customerService.UpdateCustomerAsync(customer);
                }

                ////notifications
                //if (_customerSettings.NotifyNewCustomerRegistration)
                //{
                //    await _workflowMessageService.SendCustomerRegisteredStoreOwnerNotificationMessageAsync(customer,
                //        _localizationSettings.DefaultAdminLanguageId);
                //}

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                return RedirectToRoute(NopRouteNames.Standard.REGISTER_RESULT,
                    new { resultId = (int)registrationType, returnUrl });
            }

            //errors
            foreach (var error in registrationResult.Errors)
                ModelState.AddModelError("", error);
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareRegisterModelAsync(model, true, customerAttributesXml);

        return View(model);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> RegisterResult(int resultId, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        var model = await _customerModelFactory.PrepareRegisterResultModelAsync(resultId, returnUrl);
        return View(model);
    }

    [HttpPost]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> CheckUsernameAvailability(string username)
    {
        var usernameAvailable = false;
        var statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.NotAvailable");

        if (!UsernamePropertyValidator<string, string>.IsValid(username, _customerSettings))
        {
            statusText = await _localizationService.GetResourceAsync("Account.Fields.Username.NotValid");
        }
        else if (_customerSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            if (currentCustomer != null &&
                currentCustomer.Username != null &&
                currentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
            {
                statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.CurrentUsername");
            }
            else
            {
                var customer = await _customerService.GetCustomerByUsernameAsync(username);
                if (customer == null)
                {
                    statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.Available");
                    usernameAvailable = true;
                }
            }
        }

        return Json(new { Available = usernameAvailable, Text = statusText });
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> AccountActivation(string token, string email, Guid guid)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new AccountActivationModel { ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE) };
        var cToken = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute);
        if (string.IsNullOrEmpty(cToken))
        {
            model.Result = await _localizationService.GetResourceAsync("Account.AccountActivation.AlreadyActivated");
            return View(model);
        }

        if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //activate user account
        customer.Active = true;
        await _customerService.UpdateCustomerAsync(customer);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, "");

        ////send welcome message
        //await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

        //raise event       
        await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

        //authenticate customer after activation
        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

        model.Result = await _localizationService.GetResourceAsync("Account.AccountActivation.Activated");
        return View(model);
    }

    #endregion

    #region My account / Info

    public virtual async Task<IActionResult> Profile(int documentPage = 1)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var model = new CustomerInfoModel();
        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, customer, false);
        await PrepareRecentActivitiesAsync(model, customer.Id);
        await PreparePostedDocumentsAsync(model, customer.Id, documentPage);

        return View(model);
    }

    public virtual async Task<IActionResult> Info()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var requestPath = HttpContext?.Request?.Path.Value ?? string.Empty;
        if (requestPath.EndsWith("/customer/info", StringComparison.OrdinalIgnoreCase))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT);

        var model = new CustomerInfoModel();
        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, customer, false);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Info(CustomerInfoModel model, IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();

        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var oldCustomerModel = new CustomerInfoModel();

        //get customer info model before changes for gdpr log
        if (_gdprSettings.GdprEnabled & _gdprSettings.LogUserProfileChanges)
            oldCustomerModel = await _customerModelFactory.PrepareCustomerInfoModelAsync(oldCustomerModel, customer, false);

        //custom customer attributes
        var customerAttributesXml = await ParseCustomCustomerAttributesAsync(form);
        var customerAttributeWarnings = await _customerAttributeParser.GetAttributeWarningsAsync(customerAttributesXml);
        foreach (var error in customerAttributeWarnings)
            ModelState.AddModelError("", error);

        //GDPR
        if (_gdprSettings.GdprEnabled)
        {
            var consents = (await _gdprService
                .GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage && consent.IsRequired).ToList();

            ValidateRequiredConsents(consents, form);
        }

        try
        {
            if (ModelState.IsValid)
            {
                //username 
                if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                {
                    var userName = model.Username;
                    if (!customer.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change username
                        await _customerRegistrationService.SetUsernameAsync(customer, userName);

                        //re-authenticate
                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalCustomerIfImpersonated == null)
                            await _authenticationService.SignInAsync(customer, true);
                    }
                }
                //email
                var email = model.Email;
                if (!customer.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                {
                    //change email
                    var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                    await _customerRegistrationService.SetEmailAsync(customer, email, requireValidation);

                    //do not authenticate users in impersonation mode
                    if (_workContext.OriginalCustomerIfImpersonated == null)
                    {
                        //re-authenticate (if usernames are disabled)
                        if (!_customerSettings.UsernamesEnabled && !requireValidation)
                            await _authenticationService.SignInAsync(customer, true);
                    }
                }

                //properties
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    customer.TimeZoneId = model.TimeZoneId;
                //VAT number
                if (_taxSettings.EuVatEnabled)
                {
                    var prevVatNumber = customer.VatNumber;
                    customer.VatNumber = model.VatNumber;

                    if (prevVatNumber != model.VatNumber)
                    {
                        var (vatNumberStatus, _, vatAddress) = await _taxService.GetVatNumberStatusAsync(model.VatNumber);
                        customer.VatNumberStatusId = (int)vatNumberStatus;

                        ////send VAT number admin notification
                        //if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                        //{
                        //    await _workflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(customer,
                        //        model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        //}
                    }
                }

                //form fields
                if (_customerSettings.GenderEnabled)
                    customer.Gender = model.Gender;
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                if (_customerSettings.DateOfBirthEnabled)
                    customer.DateOfBirth = model.ParseDateOfBirth();
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                customer.StreetAddress = model.StreetAddress;
                customer.StreetAddress2 = model.StreetAddress2;
                if (_customerSettings.ZipPostalCodeEnabled)
                    customer.ZipPostalCode = model.ZipPostalCode;
                customer.City = model.City;
                customer.County = model.County;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                customer.Phone = model.Phone;
                if (_customerSettings.FaxEnabled)
                    customer.Fax = model.Fax;

                customer.CustomCustomerAttributesXML = customerAttributesXml;
                await _customerService.UpdateCustomerAsync(customer);
                await SaveCustomerMilitaryProfileAsync(customer, model);

                if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                    await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SignatureAttribute, model.Signature);

                //GDPR
                if (_gdprSettings.GdprEnabled)
                    await LogGdprAsync(customer, oldCustomerModel, model, form);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.CustomerInfo.Updated"));

                return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
            }
        }
        catch (Exception exc)
        {
            ModelState.AddModelError("", exc.Message);
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, customer, true, customerAttributesXml);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> RemoveExternalAssociation(int id)
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        //ensure it's our record
        var ear = await _externalAuthenticationService.GetExternalAuthenticationRecordByIdAsync(id);

        if (ear == null)
        {
            return Json(new
            {
                redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_INFO),
            });
        }

        await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(ear);

        return Json(new
        {
            redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_INFO),
        });
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> EmailRevalidation(string token, string email, Guid guid)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new EmailRevalidationModel { ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE) };
        var cToken = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute);
        if (string.IsNullOrEmpty(cToken))
        {
            model.Result = await _localizationService.GetResourceAsync("Account.EmailRevalidation.AlreadyChanged");
            return View(model);
        }

        if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (string.IsNullOrEmpty(customer.EmailToRevalidate))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //change email
        try
        {
            await _customerRegistrationService.SetEmailAsync(customer, customer.EmailToRevalidate, false);
        }
        catch (Exception exc)
        {
            model.Result = await _localizationService.GetResourceAsync(exc.Message);
            return View(model);
        }

        customer.EmailToRevalidate = null;
        await _customerService.UpdateCustomerAsync(customer);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, "");

        //authenticate customer after changing email
        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

        model.Result = await _localizationService.GetResourceAsync("Account.EmailRevalidation.Changed");
        return View(model);
    }

    #endregion

    #region My account / Addresses

    public virtual async Task<IActionResult> Addresses()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddressDelete(int addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        return Json(new
        {
            redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT),
        });
    }

    public virtual async Task<IActionResult> AddressAdd()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddressAdd(CustomerAddressEditModel model, IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT);
    }

    public virtual async Task<IActionResult> AddressEdit(int addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddressEdit(CustomerAddressEditModel model, IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT);
    }

    #endregion

    #region My account / Downloadable products

    public virtual async Task<IActionResult> DownloadableProducts()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_PROFILE_ACCOUNT);
    }

    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public virtual IActionResult UserAgreement(Guid orderItemId)
    {
        return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
    }

    #endregion

    #region My account / Change password

    public virtual async Task<IActionResult> ChangePassword()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var model = await _customerModelFactory.PrepareChangePasswordModelAsync(customer);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> ChangePassword(ChangePasswordModel model, string returnUrl)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (ModelState.IsValid)
        {
            var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
            var changePasswordResult = await _customerRegistrationService.ChangePasswordAsync(changePasswordRequest);
            if (changePasswordResult.Success)
            {
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.ChangePassword.Success"));

                await _customerActivityService.InsertActivityAsync(customer, "PublicStore.PasswordChanged", await
                    _localizationService.GetResourceAsync("ActivityLog.PublicStore.PasswordChanged"));

                //authenticate customer after changing password
                await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                if (string.IsNullOrEmpty(returnUrl))
                    return View(model);

                //prevent open redirection attack
                if (!Url.IsLocalUrl(returnUrl))
                    returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

                return new RedirectResult(returnUrl);
            }

            //errors
            foreach (var error in changePasswordResult.Errors)
                ModelState.AddModelError("", error);
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareChangePasswordModelAsync(customer);

        return View(model);
    }

    #endregion

    #region My account / Avatar

    public virtual async Task<IActionResult> Avatar()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
    }

    [HttpPost, ActionName("Avatar")]
    [FormValueRequired("upload-avatar")]
    public virtual async Task<IActionResult> UploadAvatar(CustomerAvatarModel model, IFormFile uploadedFile, string returnUrl = null)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var contentType = uploadedFile?.ContentType.ToLowerInvariant();

        if (uploadedFile == null || string.IsNullOrEmpty(uploadedFile.FileName))
            ModelState.AddModelError("", "Vui lòng chọn ảnh đại diện.");
        else if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/gif" && contentType != "image/webp")
            ModelState.AddModelError("", "Ảnh đại diện chỉ hỗ trợ JPG, PNG, GIF hoặc WEBP.");

        if (ModelState.IsValid)
        {
            try
            {
                var customerAvatar = await _pictureService.GetPictureByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
                if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                {
                    var avatarMaxSize = Math.Max(_customerSettings.AvatarMaximumSizeBytes, 5 * 1024 * 1024);
                    if (uploadedFile.Length > avatarMaxSize)
                        throw new NopException($"Dung lượng ảnh đại diện tối đa là {avatarMaxSize / 1024 / 1024} MB.");

                    var customerPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                    if (customerAvatar != null)
                        customerAvatar = await _pictureService.UpdatePictureAsync(customerAvatar.Id, customerPictureBinary, contentType, null);
                    else
                        customerAvatar = await _pictureService.InsertPictureAsync(customerPictureBinary, contentType, null);
                }

                var customerAvatarId = 0;
                if (customerAvatar != null)
                    customerAvatarId = customerAvatar.Id;

                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatarId);

                model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
                    await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize,
                    false);

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    TempData["ProfileAvatarMessage"] = "Đã cập nhật ảnh đại diện.";
                    return Redirect(returnUrl);
                }

                TempData["ProfileAvatarMessage"] = "Đã cập nhật ảnh đại diện.";
                return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            TempData["ProfileAvatarError"] = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).FirstOrDefault() ?? "Không thể cập nhật ảnh đại diện.";
            return Redirect(returnUrl);
        }

        //If we got this far, something failed, redisplay form
        TempData["ProfileAvatarError"] = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).FirstOrDefault() ?? "Không thể cập nhật ảnh đại diện.";
        return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
    }

    [HttpPost, ActionName("Avatar")]
    [FormValueRequired("remove-avatar")]
    public virtual async Task<IActionResult> RemoveAvatar(CustomerAvatarModel model, string returnUrl = null)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var customerAvatar = await _pictureService.GetPictureByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
        if (customerAvatar != null)
            await _pictureService.DeletePictureAsync(customerAvatar);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AvatarPictureIdAttribute, 0);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            TempData["ProfileAvatarMessage"] = "Đã xóa ảnh đại diện.";
            return Redirect(returnUrl);
        }

        TempData["ProfileAvatarMessage"] = "Đã xóa ảnh đại diện.";
        return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
    }

    #endregion

    #region GDPR tools

    public virtual async Task<IActionResult> GdprTools()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (!_gdprSettings.GdprEnabled)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = await _customerModelFactory.PrepareGdprToolsModelAsync();

        return View(model);
    }

    [HttpPost, ActionName("GdprTools")]
    [FormValueRequired("export-data")]
    public virtual async Task<IActionResult> GdprToolsExport()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (!_gdprSettings.GdprEnabled)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        //log
        await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.ExportData, await _localizationService.GetResourceAsync("Gdpr.Exported"));

        var store = await _storeContext.GetCurrentStoreAsync();

        //export
        var bytes = await _exportManager.ExportCustomerGdprInfoToXlsxAsync(customer, store.Id);

        return File(bytes, MimeTypes.TextXlsx, "customerdata.xlsx");
    }

    [HttpPost, ActionName("GdprTools")]
    [FormValueRequired("delete-account")]
    public virtual async Task<IActionResult> GdprToolsDelete()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (!_gdprSettings.GdprEnabled)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        //log
        await _gdprService.InsertLogAsync(customer, 0, GdprRequestType.DeleteCustomer, await _localizationService.GetResourceAsync("Gdpr.DeleteRequested"));

        //await _workflowMessageService.SendDeleteCustomerRequestStoreOwnerNotificationAsync(customer, _localizationSettings.DefaultAdminLanguageId);

        var model = await _customerModelFactory.PrepareGdprToolsModelAsync();
        model.Result = await _localizationService.GetResourceAsync("Gdpr.DeleteRequested.Success");

        return View(model);
    }

    #endregion

    #region Check gift card balance

    //check gift card balance page
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> CheckGiftCardBalance()
    {
        if (!_customerSettings.AllowCustomersToCheckGiftCardBalance)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = await _customerModelFactory.PrepareCheckGiftCardBalanceModelAsync();

        return View(model);
    }

    [HttpPost, ActionName("CheckGiftCardBalance")]
    [FormValueRequired("checkbalancegiftcard")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> CheckBalance(CheckGiftCardBalanceModel model, bool captchaValid)
    {
        if (!_customerSettings.AllowCustomersToCheckGiftCardBalance)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnCheckGiftCardBalance && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            var giftCard = (await _giftCardService.GetAllGiftCardsAsync(giftCardCouponCode: model.GiftCardCode)).FirstOrDefault();
            if (giftCard != null && await _giftCardService.IsGiftCardValidAsync(giftCard))
            {
                var remainingAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(await _giftCardService.GetGiftCardRemainingAmountAsync(giftCard), await _workContext.GetWorkingCurrencyAsync());
                model.Result = await _priceFormatter.FormatPriceAsync(remainingAmount, true, false);
            }
            else
                model.Message = await _localizationService.GetResourceAsync("CheckGiftCardBalance.GiftCardCouponCode.Invalid");
        }

        return View(model);
    }

    #endregion

    #region Multi-factor Authentication

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> MultiFactorAuthentication()
    {
        if (!await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = new MultiFactorAuthenticationModel();
        model = await _customerModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> MultiFactorAuthentication(MultiFactorAuthenticationModel model, IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        try
        {
            if (ModelState.IsValid)
            {
                //save MultiFactorIsEnabledAttribute
                if (!model.IsEnabled)
                {
                    if (!_multiFactorAuthenticationSettings.ForceMultifactorAuthentication)
                    {
                        await _genericAttributeService
                            .SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

                        //raise change multi-factor authentication provider event       
                        await _eventPublisher.PublishAsync(new CustomerChangeMultiFactorAuthenticationProviderEvent(customer));
                    }
                    else
                    {
                        model = await _customerModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
                        model.Message = await _localizationService.GetResourceAsync("Account.MultiFactorAuthentication.Warning.ForceActivation");
                        return View(model);
                    }
                }
                else
                {
                    //save selected multi-factor authentication provider
                    var selectedProvider = await ParseSelectedProviderAsync(form);
                    var lastSavedProvider = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
                    if (string.IsNullOrEmpty(selectedProvider) && !string.IsNullOrEmpty(lastSavedProvider))
                        selectedProvider = lastSavedProvider;

                    if (selectedProvider != lastSavedProvider)
                    {
                        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, selectedProvider);

                        //raise change multi-factor authentication provider event       
                        await _eventPublisher.PublishAsync(new CustomerChangeMultiFactorAuthenticationProviderEvent(customer));
                    }
                }

                return RedirectToRoute(NopRouteNames.Standard.MULTI_FACTOR_AUTHENTICATION_SETTINGS);
            }
        }
        catch (Exception exc)
        {
            ModelState.AddModelError("", exc.Message);
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
        return View(model);
    }

    public virtual async Task<IActionResult> ConfigureMultiFactorAuthenticationProvider(string providerSysName)
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = new MultiFactorAuthenticationProviderModel();
        model = await _customerModelFactory.PrepareMultiFactorAuthenticationProviderModelAsync(model, providerSysName);

        return View(model);
    }

    #endregion

    #endregion
}
