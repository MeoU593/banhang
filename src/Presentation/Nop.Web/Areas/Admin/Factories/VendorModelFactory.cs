using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the vendor model factory implementation
/// </summary>
public partial class VendorModelFactory : IVendorModelFactory
{
    private const string VietnamCountryIsoCode = "VN";

    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ICustomerMilitaryProfileService _customerMilitaryProfileService;
    protected readonly ICustomerService _customerService;
    protected readonly ICountryService _countryService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly VendorSettings _vendorSettings;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public VendorModelFactory(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ICustomerMilitaryProfileService customerMilitaryProfileService,
        ICustomerService customerService,
        ICountryService countryService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        VendorSettings vendorSettings,
        IWorkContext workContext)
    {
        _currencySettings = currencySettings;
        _currencyService = currencyService;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _vendorAttributeParser = vendorAttributeParser;
        _vendorAttributeService = vendorAttributeService;
        _customerMilitaryProfileService = customerMilitaryProfileService;
        _customerService = customerService;
        _countryService = countryService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _vendorSettings = vendorSettings;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare vendor associated customer models
    /// </summary>
    /// <param name="models">List of vendor associated customer models</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAssociatedCustomerModelsAsync(IList<VendorAssociatedCustomerModel> models, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(vendor);

        var associatedCustomers = await _customerService.GetAllCustomersAsync(vendorId: vendor.Id);
        foreach (var customer in associatedCustomers)
        {
            var militaryProfile = await _customerMilitaryProfileService.GetByCustomerIdAsync(customer.Id);
            var roles = await _customerService.GetCustomerRolesAsync(customer, showHidden: true);
            models.Add(new VendorAssociatedCustomerModel
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Phone = customer.Phone ?? string.Empty,
                Rank = militaryProfile?.Rank ?? string.Empty,
                PositionTitle = militaryProfile?.PositionTitle ?? string.Empty,
                RoleNames = string.Join(", ", roles.Where(role => role.Active).Select(GetRoleDisplayName)),
                IsLeader = vendor.PmCustomerId.HasValue && vendor.PmCustomerId.Value == customer.Id
            });
        }
    }

    protected virtual string GetRoleDisplayName(CustomerRole role)
    {
        return role.Name;
    }

    /// <summary>
    /// Prepare vendor attribute models
    /// </summary>
    /// <param name="models">List of vendor attribute models</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareVendorAttributeModelsAsync(IList<VendorModel.VendorAttributeModel> models, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(models);

        //get available vendor attributes
        var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
        foreach (var attribute in vendorAttributes)
        {
            var attributeModel = new VendorModel.VendorAttributeModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new VendorModel.VendorAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }
            }

            //set already selected attributes
            if (vendor != null)
            {
                var selectedVendorAttributes = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopVendorDefaults.VendorAttributes);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedVendorAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = await _vendorAttributeParser.ParseAttributeValuesAsync(selectedVendorAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                {
                                    if (attributeValue.Id == item.Id)
                                        item.IsPreSelected = true;
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedVendorAttributes))
                            {
                                var enteredText = _vendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }
            }

            models.Add(attributeModel);
        }
    }

    /// <summary>
    /// Prepare vendor note search model
    /// </summary>
    /// <param name="searchModel">Vendor note search model</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>Vendor note search model</returns>
    protected virtual VendorNoteSearchModel PrepareVendorNoteSearchModel(VendorNoteSearchModel searchModel, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendor);

        searchModel.VendorId = vendor.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare vendor customer search model
    /// </summary>
    /// <param name="searchModel">Vendor customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor customer search model
    /// </returns>
    public virtual Task<VendorCustomerSearchModel> PrepareVendorCustomerSearchModelAsync(VendorCustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged vendor customer list model
    /// </summary>
    /// <param name="searchModel">Vendor customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor customer list model
    /// </returns>
    public virtual async Task<VendorCustomerListModel> PrepareVendorCustomerListModelAsync(VendorCustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get customers
        var searchCustomerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
        var customers = await _customerService.GetAllCustomersAsync(
            email: searchModel.SearchEmail,
            firstName: searchModel.SearchFirstName,
            lastName: searchModel.SearchLastName,
            company: searchModel.SearchCompany,
            customerRoleIds: searchCustomerRoleIds,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new VendorCustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer => new CustomerModel
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Company = customer.Company,
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare vendor search model
    /// </summary>
    /// <param name="searchModel">Vendor search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor search model
    /// </returns>
    public virtual Task<VendorSearchModel> PrepareVendorSearchModelAsync(VendorSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged vendor list model
    /// </summary>
    /// <param name="searchModel">Vendor search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor list model
    /// </returns>
    public virtual async Task<VendorListModel> PrepareVendorListModelAsync(VendorSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        int[] vendorIds = null;
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var rootVendorId = searchModel.SearchRootVendorId;

        if (currentVendor != null)
        {
            var allowedVendorIds = (await _vendorService.GetDescendantVendorIdsAsync(currentVendor.Id, includeSelf: true)).ToHashSet();
            if (rootVendorId <= 0 || !allowedVendorIds.Contains(rootVendorId))
                rootVendorId = currentVendor.Id;

            vendorIds = (await _vendorService.GetDescendantVendorIdsAsync(rootVendorId, includeSelf: true))
                .Where(allowedVendorIds.Contains)
                .ToArray();
        }
        else if (rootVendorId > 0)
        {
            vendorIds = (await _vendorService.GetDescendantVendorIdsAsync(rootVendorId, includeSelf: true)).ToArray();
        }

        if (rootVendorId > 0 && (vendorIds == null || vendorIds.Length == 0))
            vendorIds = [-1];

        //get vendors
        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true,
            name: searchModel.SearchName,
            email: searchModel.SearchEmail,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
            vendorIds: vendorIds);

        //prepare list model
        var model = await new VendorListModel().PrepareToGridAsync(searchModel, vendors, () =>
        {
            //fill in model values from the entity
            return vendors.SelectAwait(async vendor =>
            {
                var vendorModel = vendor.ToModel<VendorModel>();

                vendorModel.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);

                return vendorModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare vendor model
    /// </summary>
    /// <param name="model">Vendor model</param>
    /// <param name="vendor">Vendor</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor model
    /// </returns>
    public virtual async Task<VendorModel> PrepareVendorModelAsync(VendorModel model, Vendor vendor, bool excludeProperties = false)
    {
        Func<VendorLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (vendor != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = vendor.ToModel<VendorModel>();
                model.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);
            }

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Description, languageId, false, false);
                locale.MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaKeywords, languageId, false, false);
                locale.MetaDescription = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaDescription, languageId, false, false);
                locale.MetaTitle = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaTitle, languageId, false, false);
                locale.SeName = await _urlRecordService.GetSeNameAsync(vendor, languageId, false, false);
            };

            //prepare associated customers
            await PrepareAssociatedCustomerModelsAsync(model.AssociatedCustomers, vendor);

            if (vendor.PmCustomerId > 0)
            {
                var pmCustomer = await _customerService.GetCustomerByIdAsync(vendor.PmCustomerId.Value);
                model.PmCustomerInfo = pmCustomer.Email;
            }

            //prepare nested search models
            PrepareVendorNoteSearchModel(model.VendorNoteSearchModel, vendor);
        }

        //set default values for the new model
        if (vendor == null)
        {
            model.PageSize = 6;
            model.Active = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions;
            model.PriceRangeFiltering = true;
            model.ManuallyPriceRange = true;
            model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

        //prepare available parents for hierarchy
        var allVendors = await _vendorService.GetAllVendorsAsync(showHidden: true, pageSize: int.MaxValue);
        model.AvailableParents = allVendors
            .Where(v => v.Id != (vendor?.Id ?? 0))
            .OrderBy(v => v.Path ?? v.Name)
            .Select(v => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = v.Id.ToString(),
                Text = string.IsNullOrWhiteSpace(v.Path) ? v.Name : $"{v.Path} — {v.Name}",
                Selected = v.Id == (vendor?.ParentId ?? 0)
            })
            .ToList();
        model.AvailableParents.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "0", Text = "— Không có (gốc) —" });

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare model vendor attributes
        await PrepareVendorAttributeModelsAsync(model.VendorAttributes, vendor);

        //prepare address model
        var address = await _addressService.GetAddressByIdAsync(vendor?.AddressId ?? 0);
        var vietnamCountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(VietnamCountryIsoCode))?.Id;

        if (!excludeProperties && address != null)
            model.Address = address.ToModel(model.Address);

        if (vietnamCountryId.HasValue)
            model.Address.CountryId = vietnamCountryId.Value;

        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);

        if (vietnamCountryId.HasValue)
        {
            var vietnamCountryIdValue = vietnamCountryId.Value.ToString();
            model.Address.CountryId = vietnamCountryId.Value;
            model.Address.AvailableCountries = model.Address.AvailableCountries
                .Where(x => x.Value == vietnamCountryIdValue)
                .ToList();
        }

        return model;
    }

    /// <summary>
    /// Prepare paged vendor note list model
    /// </summary>
    /// <param name="searchModel">Vendor note search model</param>
    /// <param name="vendor">Vendor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor note list model
    /// </returns>
    public virtual async Task<VendorNoteListModel> PrepareVendorNoteListModelAsync(VendorNoteSearchModel searchModel, Vendor vendor)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(vendor);

        //get vendor notes
        var vendorNotes = await _vendorService.GetVendorNotesByVendorAsync(vendor.Id, searchModel.Page - 1, searchModel.PageSize);

        //prepare list model
        var model = await new VendorNoteListModel().PrepareToGridAsync(searchModel, vendorNotes, () =>
        {
            //fill in model values from the entity
            return vendorNotes.SelectAwait(async note =>
            {
                //fill in model values from the entity        
                var vendorNoteModel = note.ToModel<VendorNoteModel>();

                //convert dates to the user time
                vendorNoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(note.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                vendorNoteModel.Note = _vendorService.FormatVendorNoteText(note);

                return vendorNoteModel;
            });
        });

        return model;
    }

    #endregion
}
