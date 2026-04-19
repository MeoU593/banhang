using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Vendors;
using Nop.Web.Framework.Models.Extensions;
using AdminModels = Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using PublicModels = Nop.Plugin.Misc.DocumentPortal.Models.Public;
using VendorModels = Nop.Plugin.Misc.DocumentPortal.Models.Vendor;

namespace Nop.Plugin.Misc.DocumentPortal.Factories;

public class DocumentPortalModelFactory : IDocumentPortalModelFactory
{
    private readonly IDocumentPortalService _documentPortalService;
    private readonly IRepository<DocumentCategory> _documentCategoryRepository;
    private readonly IRepository<DocumentType> _documentTypeRepository;
    private readonly IRepository<DocumentIssuer> _documentIssuerRepository;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly ICustomerService _customerService;
    private readonly ISettingService _settingService;

    public DocumentPortalModelFactory(
        IDocumentPortalService documentPortalService,
        IRepository<DocumentCategory> documentCategoryRepository,
        IRepository<DocumentType> documentTypeRepository,
        IRepository<DocumentIssuer> documentIssuerRepository,
        IRepository<Vendor> vendorRepository,
        ICustomerService customerService,
        ISettingService settingService)
    {
        _documentPortalService = documentPortalService;
        _documentCategoryRepository = documentCategoryRepository;
        _documentTypeRepository = documentTypeRepository;
        _documentIssuerRepository = documentIssuerRepository;
        _vendorRepository = vendorRepository;
        _customerService = customerService;
        _settingService = settingService;
    }


    public async Task<AdminModels.DocumentSearchModel> PrepareDocumentSearchModelAsync(AdminModels.DocumentSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.SetGridPageSize();
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "1", Text = "Đã xuất bản" });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "2", Text = "Chưa xuất bản" });

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

        var data = await _documentPortalService.SearchAdminAsync(
            searchModel.SearchTitle,
            searchModel.SearchCode,
            searchModel.SearchCategoryId > 0 ? searchModel.SearchCategoryId : null,
            searchModel.SearchTypeId > 0 ? searchModel.SearchTypeId : null,
            searchModel.SearchIssuerId > 0 ? searchModel.SearchIssuerId : null,
            searchModel.SearchPublishedId,
            searchModel.Page - 1,
            searchModel.PageSize);

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

        model ??= new AdminModels.DocumentModel { Published = true, AllowDownload = true };

        model.AvailableCategories.Add(new SelectListItem { Value = "", Text = "-- Không chọn --" });
        foreach (var item in await _documentCategoryRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            model.AvailableCategories.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        model.AvailableTypes.Add(new SelectListItem { Value = "", Text = "-- Không chọn --" });
        foreach (var item in await _documentTypeRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            model.AvailableTypes.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        model.AvailableIssuers.Add(new SelectListItem { Value = "", Text = "-- Không chọn --" });
        foreach (var item in await _documentIssuerRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            model.AvailableIssuers.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        var allRoles = await _customerService.GetAllCustomerRolesAsync(true);
        foreach (var role in allRoles)
            model.AvailableCustomerRoles.Add(new SelectListItem { Value = role.Id.ToString(), Text = role.Name });

        if (entity != null)
            model.SelectedCustomerRoleIds = await _documentPortalService.GetRoleMappingIdsAsync(entity.Id);

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
            AutoGenerateSlug = settings.AutoGenerateSlug,
            RequireLoginForPrivateDocuments = settings.RequireLoginForPrivateDocuments
        };
    }

    public async Task<PublicModels.DocumentListPageModel> PrepareDocumentListPageModelAsync(PublicModels.DocumentSearchModel search, Customer customer)
    {
        var settings = await _settingService.LoadSettingAsync<DocumentPortalSettings>();
        var pageIndex = search.Page <= 0 ? 0 : search.Page - 1;
        var pageSize = settings.DefaultPageSize;

        var documents = await _documentPortalService.SearchAsync(
            search.Q,
            search.CategoryId,
            search.TypeId,
            search.IssuerId,
            search.Year,
            pageIndex,
            pageSize);

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
                AllowDownload = doc.AllowDownload,
                UploadedByCustomerId = doc.UploadedByCustomerId,
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

    public async Task<VendorModels.VendorDocumentSearchModel> PrepareVendorDocumentSearchModelAsync(
        VendorModels.VendorDocumentSearchModel searchModel, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.SetGridPageSize();

        var vendor = customer.VendorId > 0
            ? await _vendorRepository.Table.FirstOrDefaultAsync(v => v.Id == customer.VendorId && !v.Deleted)
            : null;

        searchModel.VendorName = vendor?.Name ?? string.Empty;
        searchModel.IsVendorManager = vendor?.PmCustomerId == customer.Id;

        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "1", Text = "Đã xuất bản" });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem { Value = "2", Text = "Chưa xuất bản" });

        searchModel.AvailableCategories.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        foreach (var item in await _documentCategoryRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            searchModel.AvailableCategories.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        searchModel.AvailableTypes.Add(new SelectListItem { Value = "0", Text = "Tất cả" });
        foreach (var item in await _documentTypeRepository.Table.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync())
            searchModel.AvailableTypes.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

        return searchModel;
    }

    public async Task<VendorModels.VendorDocumentListModel> PrepareVendorDocumentListModelAsync(
        VendorModels.VendorDocumentSearchModel searchModel, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        if (customer.VendorId <= 0)
            return new VendorModels.VendorDocumentListModel();

        var data = await _documentPortalService.SearchVendorAsync(
            customer.VendorId,
            searchModel.SearchKeywords,
            searchModel.SearchCategoryId > 0 ? searchModel.SearchCategoryId : null,
            searchModel.SearchTypeId > 0 ? searchModel.SearchTypeId : null,
            searchModel.SearchPublishedId,
            searchModel.Page - 1,
            searchModel.PageSize);

        return new VendorModels.VendorDocumentListModel().PrepareToGrid(searchModel, data, () =>
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
                CreatedOnUtc = entity.CreatedOnUtc,
                UpdatedOnUtc = entity.UpdatedOnUtc
            });
        });
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

        var related = await _documentPortalService.GetRelatedDocumentsAsync(entity, 5);
        var mappedRoles = await _documentPortalService.GetRoleMappingIdsAsync(entity.Id);

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
            AllowDownload = entity.AllowDownload,
            ViewCount = entity.ViewCount,
            DownloadCount = entity.DownloadCount,
            IsRestricted = mappedRoles.Any(),
            CanDelete = await _documentPortalService.CanDeleteAsync(entity, customer),
            RelatedDocuments = related.Select(x => new PublicModels.DocumentListItemModel
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                Code = x.Code,
                Summary = x.Summary,
                IssuedDate = x.IssuedDate,
                DownloadCount = x.DownloadCount
            }).ToList()
        };
    }

    public async Task<PublicModels.DocumentUploadModel> PrepareUploadModelAsync(PublicModels.DocumentUploadModel? model = null)
    {
        model ??= new PublicModels.DocumentUploadModel();

        model.AvailableCategories.Add(new SelectListItem { Value = "", Text = "-- Chọn danh mục --" });
        foreach (var cat in await _documentPortalService.GetAllCategoriesAsync())
            model.AvailableCategories.Add(new SelectListItem { Value = cat.Id.ToString(), Text = cat.Name });

        model.AvailableTypes.Add(new SelectListItem { Value = "", Text = "-- Chọn loại tài liệu --" });
        foreach (var typ in await _documentPortalService.GetAllTypesAsync())
            model.AvailableTypes.Add(new SelectListItem { Value = typ.Id.ToString(), Text = typ.Name });

        model.AvailableIssuers.Add(new SelectListItem { Value = "", Text = "-- Chọn cơ quan ban hành --" });
        foreach (var iss in await _documentPortalService.GetAllIssuersAsync())
            model.AvailableIssuers.Add(new SelectListItem { Value = iss.Id.ToString(), Text = iss.Name });

        return model;
    }
}
