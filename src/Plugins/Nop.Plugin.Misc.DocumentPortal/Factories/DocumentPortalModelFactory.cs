using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Core;
using Nop.Web.Framework.Models.Extensions;
using AdminModels = Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using PublicModels = Nop.Plugin.Misc.DocumentPortal.Models.Public;

namespace Nop.Plugin.Misc.DocumentPortal.Factories;

public class DocumentPortalModelFactory : IDocumentPortalModelFactory
{
    private static readonly Regex ReplyToTokenRegex = new(@"^\[replyto:(\d+)\]\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly IDocumentPortalService _documentPortalService;
    private readonly IRepository<DocumentCategory> _documentCategoryRepository;
    private readonly IRepository<DocumentType> _documentTypeRepository;
    private readonly IRepository<DocumentIssuer> _documentIssuerRepository;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly CustomerSettings _customerSettings;
    private readonly ICustomerMilitaryProfileService _customerMilitaryProfileService;
    private readonly ICustomerService _customerService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IPictureService _pictureService;
    private readonly ISettingService _settingService;
    private readonly IWorkContext _workContext;
    private readonly MediaSettings _mediaSettings;

    public DocumentPortalModelFactory(
        IDocumentPortalService documentPortalService,
        IRepository<DocumentCategory> documentCategoryRepository,
        IRepository<DocumentType> documentTypeRepository,
        IRepository<DocumentIssuer> documentIssuerRepository,
        IRepository<Vendor> vendorRepository,
        CustomerSettings customerSettings,
        ICustomerMilitaryProfileService customerMilitaryProfileService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        IPictureService pictureService,
        ISettingService settingService,
        IWorkContext workContext,
        MediaSettings mediaSettings)
    {
        _documentPortalService = documentPortalService;
        _documentCategoryRepository = documentCategoryRepository;
        _documentTypeRepository = documentTypeRepository;
        _documentIssuerRepository = documentIssuerRepository;
        _vendorRepository = vendorRepository;
        _customerSettings = customerSettings;
        _customerMilitaryProfileService = customerMilitaryProfileService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _pictureService = pictureService;
        _settingService = settingService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
    }

    private static string GetAccessScopeName(int accessScopeId)
    {
        return accessScopeId == (int)DocumentAccessScope.Public ? "Công khai" : "Nội bộ";
    }

    private static void PrepareAccessScopeOptions(IList<SelectListItem> items, int selectedValue = 0, bool includeDefault = false)
    {
        if (includeDefault)
            items.Add(new SelectListItem { Value = "0", Text = "-- Chọn phạm vi --", Selected = selectedValue == 0 });

        items.Add(new SelectListItem
        {
            Value = ((int)DocumentAccessScope.Public).ToString(),
            Text = GetAccessScopeName((int)DocumentAccessScope.Public),
            Selected = selectedValue == (int)DocumentAccessScope.Public
        });
        items.Add(new SelectListItem
        {
            Value = ((int)DocumentAccessScope.Internal).ToString(),
            Text = GetAccessScopeName((int)DocumentAccessScope.Internal),
            Selected = selectedValue == (int)DocumentAccessScope.Internal
        });
    }

    private async Task PrepareOwnerVendorOptionsAsync(IList<SelectListItem> items, int? selectedValue, bool includeGlobal = true)
    {
        if (includeGlobal)
            items.Add(new SelectListItem { Value = "", Text = "Tài liệu hệ thống/công khai", Selected = !selectedValue.HasValue });

        var vendors = await _vendorRepository.Table
            .Where(x => !x.Deleted && x.Active)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();

        foreach (var vendor in vendors)
            items.Add(new SelectListItem { Value = vendor.Id.ToString(), Text = vendor.Name, Selected = selectedValue == vendor.Id });
    }

    private async Task<string?> GetVendorNameAsync(int? vendorId)
    {
        if (!vendorId.HasValue || vendorId <= 0)
            return null;

        return await _vendorRepository.Table
            .Where(x => x.Id == vendorId.Value)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();
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

    private async Task<PublicModels.DocumentCommentModel> PrepareDocumentCommentModelAsync(DocumentComment comment)
    {
        var customer = await _customerService.GetCustomerByIdAsync(comment.CustomerId);
        var customerIsGuest = customer == null || await _customerService.IsGuestAsync(customer);
        var militaryProfile = customerIsGuest ? null : await _customerMilitaryProfileService.GetByCustomerIdAsync(comment.CustomerId);
        var (replyToCommentId, displayCommentText) = ParseReplyData(comment.CommentText);

        var model = new PublicModels.DocumentCommentModel
        {
            Id = comment.Id,
            ReplyToCommentId = replyToCommentId,
            CustomerId = comment.CustomerId,
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            CommentText = displayCommentText,
            Rank = militaryProfile?.Rank,
            UnitName = militaryProfile?.UnitName,
            PositionTitle = militaryProfile?.PositionTitle,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(comment.CreatedOnUtc, DateTimeKind.Utc),
            AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !customerIsGuest
        };

        if (_customerSettings.AllowCustomersToUploadAvatars && customer != null)
        {
            model.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                _customerSettings.DefaultAvatarEnabled,
                defaultPictureType: PictureType.Avatar);
        }

        return model;
    }


    public async Task<AdminModels.DocumentSearchModel> PrepareDocumentSearchModelAsync(AdminModels.DocumentSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.SetGridPageSize();
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "1", Text = "Đã xuất bản" });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "2", Text = "Chưa xuất bản" });

        PrepareAccessScopeOptions(searchModel.AvailableAccessScopes);
        searchModel.AvailableAccessScopes.Insert(0, new SelectListItem { Value = "0", Text = "Tất cả" });
        await PrepareOwnerVendorOptionsAsync(searchModel.AvailableOwnerVendors, null);
        searchModel.AvailableOwnerVendors.Insert(0, new SelectListItem { Value = "0", Text = "Tất cả" });

        searchModel.AvailableCategories.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        foreach (var item in await _documentCategoryRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            searchModel.AvailableCategories.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        searchModel.AvailableTypes.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        foreach (var item in await _documentTypeRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            searchModel.AvailableTypes.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        searchModel.AvailableIssuers.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        foreach (var item in await _documentIssuerRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            searchModel.AvailableIssuers.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        return searchModel;
    }

    public async Task<AdminModels.DocumentListModel> PrepareDocumentListModelAsync(AdminModels.DocumentSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        var data = await _documentPortalService.SearchAdminAsync(
            searchModel.SearchTitle,
            searchModel.SearchCode,
            searchModel.SearchCategoryId > 0 ? searchModel.SearchCategoryId : null,
            searchModel.SearchTypeId > 0 ? searchModel.SearchTypeId : null,
            searchModel.SearchIssuerId > 0 ? searchModel.SearchIssuerId : null,
            searchModel.SearchOwnerVendorId > 0 ? searchModel.SearchOwnerVendorId : null,
            searchModel.SearchAccessScopeId,
            searchModel.SearchPublishedId,
            searchModel.Page - 1,
            searchModel.PageSize,
            currentCustomer);

        return new AdminModels.DocumentListModel().PrepareToGrid(searchModel, data, () =>
        {
            return data.Select(entity => new AdminModels.DocumentModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Code = entity.Code,
                Published = entity.Published,
                IssuedDate = entity.IssuedDate,
                ViewCount = entity.ViewCount,
                DownloadCount = entity.DownloadCount,
                DisplayOrder = entity.DisplayOrder,
                OwnerVendorId = entity.OwnerVendorId,
                OwnerVendorName = GetVendorNameAsync(entity.OwnerVendorId).GetAwaiter().GetResult() ?? "Hệ thống",
                AccessScopeId = entity.AccessScopeId == (int)DocumentAccessScope.Public
                    ? (int)DocumentAccessScope.Public
                    : (int)DocumentAccessScope.Internal,
                AccessScopeName = GetAccessScopeName(entity.AccessScopeId),
                CreatedOnUtc = entity.CreatedOnUtc,
                UpdatedOnUtc = entity.UpdatedOnUtc
            });
        });
    }

    public async Task<AdminModels.DocumentModel> PrepareDocumentModelAsync(AdminModels.DocumentModel? model, Document? entity, bool excludeProperties = false)
    {
        if (entity != null && model == null)
        {
            model = new AdminModels.DocumentModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Slug = entity.Slug,
                Code = entity.Code,
                Summary = entity.Summary,
                Content = entity.Content,
                Keywords = entity.Keywords,
                DocumentCategoryId = entity.DocumentCategoryId,
                DocumentTypeId = entity.DocumentTypeId,
                IssuerId = entity.IssuerId,
                IssuedDate = entity.IssuedDate,
                EffectiveDate = entity.EffectiveDate,
                DownloadId = entity.DownloadId,
                ThumbnailPictureId = entity.ThumbnailPictureId,
                OwnerVendorId = entity.OwnerVendorId,
                OwnerVendorName = await GetVendorNameAsync(entity.OwnerVendorId),
                AccessScopeId = entity.AccessScopeId == (int)DocumentAccessScope.Public
                    ? (int)DocumentAccessScope.Public
                    : (int)DocumentAccessScope.Internal,
                Published = entity.Published,
                AllowDownload = entity.AllowDownload,
                ShowOnHomepage = entity.ShowOnHomepage,
                DisplayOrder = entity.DisplayOrder,
                ViewCount = entity.ViewCount,
                DownloadCount = entity.DownloadCount,
                CreatedOnUtc = entity.CreatedOnUtc,
                UpdatedOnUtc = entity.UpdatedOnUtc
            };
        }

        model ??= new AdminModels.DocumentModel
        {
            Published = true,
            AllowDownload = true,
            AccessScopeId = (int)DocumentAccessScope.Public
        };

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        model.CanSelectOwnerVendor = currentCustomer.VendorId <= 0;
        if (currentCustomer.VendorId > 0)
            model.OwnerVendorId = currentCustomer.VendorId;

        model.AvailableCategories.Add(new SelectListItem { Value = "", Text = "-- Không chọn --" });
        foreach (var item in await _documentCategoryRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            model.AvailableCategories.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        model.AvailableTypes.Add(new SelectListItem { Value = "", Text = "-- Không chọn --" });
        foreach (var item in await _documentTypeRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            model.AvailableTypes.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        model.AvailableIssuers.Add(new SelectListItem { Value = "", Text = "-- Không chọn --" });
        foreach (var item in await _documentIssuerRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            model.AvailableIssuers.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        await PrepareOwnerVendorOptionsAsync(model.AvailableOwnerVendors, model.OwnerVendorId, model.CanSelectOwnerVendor);
        PrepareAccessScopeOptions(model.AvailableAccessScopes, model.AccessScopeId, includeDefault: true);

        return model;
    }

    public async Task<AdminModels.ConfigurationModel> PrepareConfigurationModelAsync()
    {
        var settings = await _settingService.LoadSettingAsync<DocumentPortalSettings>();
        return new AdminModels.ConfigurationModel
        {
            DefaultPageSize = settings.DefaultPageSize,
            SearchInContent = settings.SearchInContent,
            ShowRelatedDocuments = settings.ShowRelatedDocuments,
            ShowDownloadCount = settings.ShowDownloadCount,
            AutoGenerateSlug = settings.AutoGenerateSlug
        };
    }

    public async Task<PublicModels.DocumentListPageModel> PrepareDocumentListPageModelAsync(PublicModels.DocumentSearchModel search, Customer customer)
    {
        var settings = await _settingService.LoadSettingAsync<DocumentPortalSettings>();
        var pageIndex = search.Page <= 0 ? 0 : search.Page - 1;
        var pageSize = settings.DefaultPageSize > 0 ? settings.DefaultPageSize : 20;

        var documents = await _documentPortalService.SearchAsync(
            search.Q,
            search.CategoryId,
            search.TypeId,
            search.IssuerId,
            search.DateFrom,
            search.DateTo,
            pageIndex,
            pageSize,
            customer: customer);

        search.Page = documents.PageIndex + 1;
        search.PageSize = documents.PageSize;

        var categories = await _documentPortalService.GetAllCategoriesAsync();
        var types = await _documentPortalService.GetAllTypesAsync();
        var issuers = await _documentPortalService.GetAllIssuersAsync();

        var categoryDict = categories.ToDictionary(x => x.Id, x => x.Name);
        var typeDict = types.ToDictionary(x => x.Id, x => x.Name);
        var issuerDict = issuers.ToDictionary(x => x.Id, x => x.Name);

        var items = new List<PublicModels.DocumentListItemModel>();
        foreach (var doc in documents)
        {
            items.Add(new PublicModels.DocumentListItemModel
            {
                Id = doc.Id,
                Title = doc.Title,
                Slug = doc.Slug,
                Code = doc.Code,
                Summary = doc.Summary,
                CategoryName = doc.DocumentCategoryId.HasValue && categoryDict.TryGetValue(doc.DocumentCategoryId.Value, out var cat) ? cat : null,
                TypeName = doc.DocumentTypeId.HasValue && typeDict.TryGetValue(doc.DocumentTypeId.Value, out var typ) ? typ : null,
                IssuerName = doc.IssuerId.HasValue && issuerDict.TryGetValue(doc.IssuerId.Value, out var iss) ? iss : null,
                IssuedDate = doc.IssuedDate,
                DownloadCount = doc.DownloadCount,
                AllowDownload = doc.DownloadId.HasValue,
                UploadedByCustomerId = doc.UploadedByCustomerId,
                OwnerVendorId = doc.OwnerVendorId,
                OwnerVendorName = await GetVendorNameAsync(doc.OwnerVendorId),
                AccessScopeName = GetAccessScopeName(doc.AccessScopeId),
                CanDelete = await _documentPortalService.CanDeleteAsync(doc, customer)
            });
        }

        var model = new PublicModels.DocumentListPageModel
        {
            Documents = items,
            Search = search,
            TotalCount = documents.TotalCount,
            TotalPages = documents.TotalPages,
            IsAuthenticated = !await _customerService.IsGuestAsync(customer)
        };

        model.AvailableCategories.Add(new SelectListItem { Value = "", Text = "Tất cả danh mục" });
        foreach (var cat in categories)
            model.AvailableCategories.Add(new SelectListItem { Value = cat.Id.ToString(), Text = cat.Name, Selected = search.CategoryId == cat.Id });

        model.AvailableTypes.Add(new SelectListItem { Value = "", Text = "Tất cả loại" });
        foreach (var typ in types)
            model.AvailableTypes.Add(new SelectListItem { Value = typ.Id.ToString(), Text = typ.Name, Selected = search.TypeId == typ.Id });

        model.AvailableIssuers.Add(new SelectListItem { Value = "", Text = "Tất cả cơ quan ban hành" });
        foreach (var iss in issuers)
            model.AvailableIssuers.Add(new SelectListItem { Value = iss.Id.ToString(), Text = iss.Name, Selected = search.IssuerId == iss.Id });

        return model;
    }

    public async Task<PublicModels.DocumentDetailModel> PreparePublicDetailModelAsync(Document entity, Customer customer)
    {
        var categoryName = entity.DocumentCategoryId.HasValue
            ? await _documentCategoryRepository.Table.Where(x => x.Id == entity.DocumentCategoryId.Value).Select(x => x.Name).FirstOrDefaultAsync()
            : null;
        var typeName = entity.DocumentTypeId.HasValue
            ? await _documentTypeRepository.Table.Where(x => x.Id == entity.DocumentTypeId.Value).Select(x => x.Name).FirstOrDefaultAsync()
            : null;
        var issuerName = entity.IssuerId.HasValue
            ? await _documentIssuerRepository.Table.Where(x => x.Id == entity.IssuerId.Value).Select(x => x.Name).FirstOrDefaultAsync()
            : null;

        var related = await _documentPortalService.GetRelatedDocumentsAsync(entity, 20, customer);
        var comments = await _documentPortalService.GetDocumentCommentsAsync(entity.Id, approved: true);
        var commentModels = new List<PublicModels.DocumentCommentModel>();
        foreach (var comment in comments)
            commentModels.Add(await PrepareDocumentCommentModelAsync(comment));

        var visibleRelated = new List<PublicModels.DocumentListItemModel>();
        foreach (var item in related)
        {
            if (!await _documentPortalService.CanAccessDocumentAsync(item, customer))
                continue;

            visibleRelated.Add(new PublicModels.DocumentListItemModel
            {
                Id = item.Id,
                Title = item.Title,
                Slug = item.Slug,
                Code = item.Code,
                Summary = item.Summary,
                IssuedDate = item.IssuedDate,
                DownloadCount = item.DownloadCount,
                OwnerVendorId = item.OwnerVendorId,
                OwnerVendorName = await GetVendorNameAsync(item.OwnerVendorId),
                AccessScopeName = GetAccessScopeName(item.AccessScopeId),
                AllowDownload = item.DownloadId.HasValue
            });

            if (visibleRelated.Count >= 5)
                break;
        }

        return new PublicModels.DocumentDetailModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Slug = entity.Slug,
            Code = entity.Code,
            Summary = entity.Summary,
            Content = entity.Content,
            CategoryName = categoryName,
            TypeName = typeName,
            IssuerName = issuerName,
            IssuedDate = entity.IssuedDate,
            EffectiveDate = entity.EffectiveDate,
            PreviewUrl = entity.DownloadId.HasValue ? $"/tai-lieu/xem/{entity.Id}" : null,
            DownloadUrl = entity.DownloadId.HasValue ? $"/tai-lieu/tai/{entity.Id}" : null,
            HasContent = !string.IsNullOrWhiteSpace(entity.Content),
            HasDownload = entity.DownloadId.HasValue,
            OwnerVendorId = entity.OwnerVendorId,
            OwnerVendorName = await GetVendorNameAsync(entity.OwnerVendorId),
            AccessScopeName = GetAccessScopeName(entity.AccessScopeId),
            ViewCount = entity.ViewCount,
            DownloadCount = entity.DownloadCount,
            CanDelete = await _documentPortalService.CanDeleteAsync(entity, customer),
            CanAddComments = !await _customerService.IsGuestAsync(customer),
            NumberOfComments = commentModels.Count,
            Comments = commentModels,
            RelatedDocuments = visibleRelated
        };
    }

    public async Task<PublicModels.DocumentUploadModel> PrepareUploadModelAsync(PublicModels.DocumentUploadModel? model = null)
    {
        model ??= new PublicModels.DocumentUploadModel();
        PrepareAccessScopeOptions(model.AvailableAccessScopes, model.AccessScopeId, includeDefault: true);

        try
        {
            var categories = await _documentPortalService.GetAllCategoriesAsync();
            model.AvailableCategories.Add(new SelectListItem { Value = "", Text = "-- Chọn danh mục --" });
            if (categories != null)
            {
                foreach (var cat in categories)
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Value = cat.Id.ToString(),
                        Text = cat.Name ?? $"Danh mục {cat.Id}",
                        Selected = model.DocumentCategoryId == cat.Id
                    });
            }
        }
        catch
        {
            model.AvailableCategories.Add(new SelectListItem { Value = "", Text = "-- Lỗi dữ liệu --" });
        }

        try
        {
            var types = await _documentPortalService.GetAllTypesAsync();
            model.AvailableTypes.Add(new SelectListItem { Value = "", Text = "-- Chọn loại tài liệu --" });
            if (types != null)
            {
                foreach (var typ in types)
                    model.AvailableTypes.Add(new SelectListItem
                    {
                        Value = typ.Id.ToString(),
                        Text = typ.Name ?? $"Loại {typ.Id}",
                        Selected = model.DocumentTypeId == typ.Id
                    });
            }
        }
        catch
        {
            model.AvailableTypes.Add(new SelectListItem { Value = "", Text = "-- Lỗi dữ liệu --" });
        }

        try
        {
            var issuers = await _documentPortalService.GetAllIssuersAsync();
            model.AvailableIssuers.Add(new SelectListItem { Value = "", Text = "-- Chọn cơ quan ban hành --" });
            if (issuers != null)
            {
                foreach (var iss in issuers)
                    model.AvailableIssuers.Add(new SelectListItem
                    {
                        Value = iss.Id.ToString(),
                        Text = iss.Name ?? $"Cơ quan {iss.Id}",
                        Selected = model.IssuerId == iss.Id
                    });
            }
        }
        catch
        {
            model.AvailableIssuers.Add(new SelectListItem { Value = "", Text = "-- Lỗi dữ liệu --" });
        }

        return model;
    }
}
