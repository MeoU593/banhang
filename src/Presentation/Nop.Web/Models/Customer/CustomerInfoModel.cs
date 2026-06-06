using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer;

public partial record CustomerInfoModel : BaseNopModel
{
    public CustomerInfoModel()
    {
        AvailableTimeZones = new List<SelectListItem>();
        AvailableCountries = new List<SelectListItem>();
        AvailableStates = new List<SelectListItem>();
        AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
        CustomerAttributes = new List<CustomerAttributeModel>();
        GdprConsents = new List<GdprConsentModel>();
        RecentActivities = new List<RecentActivityModel>();
        PostedDocuments = new List<PostedDocumentModel>();
    }

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Account.Fields.Email")]
    public string Email { get; set; }
    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Account.Fields.EmailToRevalidate")]
    public string EmailToRevalidate { get; set; }

    public bool CheckUsernameAvailabilityEnabled { get; set; }
    public bool AllowUsersToChangeUsernames { get; set; }
    public bool UsernamesEnabled { get; set; }
    [NopResourceDisplayName("Account.Fields.Username")]
    public string Username { get; set; }

    //form fields & properties
    public bool GenderEnabled { get; set; }
    [NopResourceDisplayName("Account.Fields.Gender")]
    public string Gender { get; set; }

    public bool NeutralGenderEnabled { get; set; }

    public bool FirstNameEnabled { get; set; }
    [NopResourceDisplayName("Account.Fields.FirstName")]
    public string FirstName { get; set; }
    public bool FirstNameRequired { get; set; }
    public bool LastNameEnabled { get; set; }
    [NopResourceDisplayName("Account.Fields.LastName")]
    public string LastName { get; set; }
    public bool LastNameRequired { get; set; }

    public bool DateOfBirthEnabled { get; set; }
    [NopResourceDisplayName("Account.Fields.DateOfBirth")]
    public int? DateOfBirthDay { get; set; }
    [NopResourceDisplayName("Account.Fields.DateOfBirth")]
    public int? DateOfBirthMonth { get; set; }
    [NopResourceDisplayName("Account.Fields.DateOfBirth")]
    public int? DateOfBirthYear { get; set; }
    public bool DateOfBirthRequired { get; set; }
    public DateTime? ParseDateOfBirth()
    {
        return CommonHelper.ParseDate(DateOfBirthYear, DateOfBirthMonth, DateOfBirthDay);
    }

    public string MilitaryCode { get; set; }

    public string Rank { get; set; }

    public string UnitName { get; set; }

    public string PositionTitle { get; set; }

    public DateTime? EnlistmentDate { get; set; }

    public string AvatarUrl { get; set; }

    public bool StreetAddressEnabled { get; set; }
    public bool StreetAddressRequired { get; set; }
    [NopResourceDisplayName("Account.Fields.StreetAddress")]
    public string StreetAddress { get; set; }

    public bool StreetAddress2Enabled { get; set; }
    public bool StreetAddress2Required { get; set; }
    [NopResourceDisplayName("Account.Fields.StreetAddress2")]
    public string StreetAddress2 { get; set; }

    public bool ZipPostalCodeEnabled { get; set; }
    public bool ZipPostalCodeRequired { get; set; }
    [NopResourceDisplayName("Account.Fields.ZipPostalCode")]
    public string ZipPostalCode { get; set; }

    public bool CityEnabled { get; set; }
    public bool CityRequired { get; set; }
    [NopResourceDisplayName("Account.Fields.City")]
    public string City { get; set; }

    public bool CountyEnabled { get; set; }
    public bool CountyRequired { get; set; }
    [NopResourceDisplayName("Account.Fields.County")]
    public string County { get; set; }

    public bool CountryEnabled { get; set; }
    public bool CountryRequired { get; set; }
    [NopResourceDisplayName("Account.Fields.Country")]
    public int CountryId { get; set; }
    public IList<SelectListItem> AvailableCountries { get; set; }

    public bool StateProvinceEnabled { get; set; }
    public bool StateProvinceRequired { get; set; }
    [NopResourceDisplayName("Account.Fields.StateProvince")]
    public int StateProvinceId { get; set; }
    public IList<SelectListItem> AvailableStates { get; set; }

    public bool PhoneEnabled { get; set; }
    public bool PhoneRequired { get; set; }
    [DataType(DataType.PhoneNumber)]
    [NopResourceDisplayName("Account.Fields.Phone")]
    public string Phone { get; set; }

    public bool FaxEnabled { get; set; }
    public bool FaxRequired { get; set; }
    [DataType(DataType.PhoneNumber)]
    [NopResourceDisplayName("Account.Fields.Fax")]
    public string Fax { get; set; }

    //preferences
    public bool SignatureEnabled { get; set; }
    [NopResourceDisplayName("Account.Fields.Signature")]
    public string Signature { get; set; }

    //time zone
    [NopResourceDisplayName("Account.Fields.TimeZone")]
    public string TimeZoneId { get; set; }
    public bool AllowCustomersToSetTimeZone { get; set; }
    public IList<SelectListItem> AvailableTimeZones { get; set; }

    //EU VAT
    [NopResourceDisplayName("Account.Fields.VatNumber")]
    public string VatNumber { get; set; }
    public string VatNumberStatusNote { get; set; }
    public bool DisplayVatNumber { get; set; }
    public bool VatNumberRequired { get; set; }

    //external authentication
    [NopResourceDisplayName("Account.AssociatedExternalAuth")]
    public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }
    public int NumberOfExternalAuthenticationProviders { get; set; }
    public bool AllowCustomersToRemoveAssociations { get; set; }

    public IList<CustomerAttributeModel> CustomerAttributes { get; set; }

    public IList<GdprConsentModel> GdprConsents { get; set; }

    public IList<RecentActivityModel> RecentActivities { get; set; }

    public IList<PostedDocumentModel> PostedDocuments { get; set; }

    public int PostedDocumentsPage { get; set; }

    public int PostedDocumentsPageSize { get; set; }

    public int PostedDocumentsTotalCount { get; set; }

    public int PostedDocumentsTotalPages { get; set; }

    #region Nested classes

    public partial record AssociatedExternalAuthModel : BaseNopEntityModel
    {
        public string Email { get; set; }

        public string ExternalIdentifier { get; set; }

        public string AuthMethodName { get; set; }
    }

    public partial record RecentActivityModel : BaseNopModel
    {
        public string ActivityTypeName { get; set; }

        public string Comment { get; set; }

        public string EntityName { get; set; }

        public string IpAddress { get; set; }

        public DateTime CreatedOnUtc { get; set; }
    }

    public partial record PostedDocumentModel : BaseNopEntityModel
    {
        public string Title { get; set; }

        public string Code { get; set; }

        public string Slug { get; set; }

        public string Summary { get; set; }

        public int AccessScopeId { get; set; }

        public string AccessScopeName { get; set; }

        public bool HasDownload { get; set; }

        public bool Published { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }
    }

    #endregion
}
