using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Services.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.DocumentPortal.Services;

public class DocumentPortalService : IDocumentPortalService
{
    private readonly IRepository<Document> _documentRepository;
    private readonly IRepository<DocumentComment> _documentCommentRepository;
    private readonly IRepository<DocumentCategory> _documentCategoryRepository;
    private readonly IRepository<DocumentType> _documentTypeRepository;
    private readonly IRepository<DocumentIssuer> _documentIssuerRepository;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IPermissionService _permissionService;
    private readonly ICustomerService _customerService;

    public DocumentPortalService(
        IRepository<Document> documentRepository,
        IRepository<DocumentComment> documentCommentRepository,
        IRepository<DocumentCategory> documentCategoryRepository,
        IRepository<DocumentType> documentTypeRepository,
        IRepository<DocumentIssuer> documentIssuerRepository,
        IRepository<Vendor> vendorRepository,
        IPermissionService permissionService,
        ICustomerService customerService)
    {
        _documentRepository = documentRepository;
        _documentCommentRepository = documentCommentRepository;
        _documentCategoryRepository = documentCategoryRepository;
        _documentTypeRepository = documentTypeRepository;
        _documentIssuerRepository = documentIssuerRepository;
        _vendorRepository = vendorRepository;
        _permissionService = permissionService;
        _customerService = customerService;
    }

    public async Task<bool> IsSystemAdminAsync(Customer? customer)
    {
        return customer != null && await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName);
    }

    public async Task<bool> HasUnitDocumentManageAccessAsync(Customer? customer)
    {
        if (customer == null || customer.VendorId <= 0)
            return false;

        var vendor = await _vendorRepository.GetByIdAsync(customer.VendorId);
        if (vendor == null || vendor.Deleted || !vendor.Active)
            return false;

        if (vendor.PmCustomerId == customer.Id)
            return true;

        return await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.VendorsRoleName);
    }

    public async Task<bool> CanAccessDocumentManagementAsync(Customer? customer)
    {
        return await IsSystemAdminAsync(customer) || await HasUnitDocumentManageAccessAsync(customer);
    }

    public async Task<IPagedList<Document>> SearchAsync(
        string? keywords = null,
        int? categoryId = null,
        int? typeId = null,
        int? issuerId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool showHidden = false,
        Customer? customer = null)
    {
        if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
            (dateFrom, dateTo) = (dateTo, dateFrom);

        var query = _documentRepository.Table;

        if (!showHidden)
            query = query.Where(x => x.Published && !x.Deleted);

        if (categoryId.HasValue)
            query = query.Where(x => x.DocumentCategoryId == categoryId.Value);

        if (typeId.HasValue)
            query = query.Where(x => x.DocumentTypeId == typeId.Value);

        if (issuerId.HasValue)
            query = query.Where(x => x.IssuerId == issuerId.Value);

        if (dateFrom.HasValue)
        {
            var issuedFrom = dateFrom.Value.Date;
            query = query.Where(x => x.IssuedDate.HasValue && x.IssuedDate.Value >= issuedFrom);
        }

        if (dateTo.HasValue)
        {
            var issuedTo = dateTo.Value.Date.AddDays(1).AddSeconds(-1);
            query = query.Where(x => x.IssuedDate.HasValue && x.IssuedDate.Value <= issuedTo);
        }

        var isGuest = customer == null || await _customerService.IsGuestAsync(customer);
        if (!showHidden && isGuest)
            query = query.Where(x => x.AccessScopeId == (int)DocumentAccessScope.Public);

        if (!string.IsNullOrWhiteSpace(keywords))
        {
            var q = keywords.Trim();
            query = query.Where(x =>
                (x.Code != null && x.Code.Contains(q)) ||
                x.Title.Contains(q) ||
                (x.Keywords != null && x.Keywords.Contains(q)) ||
                (x.Summary != null && x.Summary.Contains(q)));

            query = query
                .OrderBy(x => x.Code == q ? 0 : 1)
                .ThenBy(x => x.Title.StartsWith(q) ? 0 : 1)
                .ThenByDescending(x => x.IssuedDate)
                .ThenBy(x => x.DisplayOrder);
        }
        else
        {
            query = query
                .OrderByDescending(x => x.IssuedDate)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.Title);
        }

        return await query.ToPagedListAsync(Math.Max(0, pageIndex), Math.Max(1, pageSize));
    }

    public async Task<bool> CanAccessDocumentAsync(Document document, Customer customer)
    {
        return await CanViewDocumentAsync(document, customer);
    }

    public async Task<bool> CanViewDocumentAsync(Document document, Customer customer)
    {
        if (document == null || document.Deleted || !document.Published)
            return false;

        if (await IsSystemAdminAsync(customer))
            return true;

        if ((DocumentAccessScope)document.AccessScopeId == DocumentAccessScope.Public)
            return true;

        return !await _customerService.IsGuestAsync(customer);
    }

    public async Task<bool> CanManageDocumentAsync(Document document, Customer customer)
    {
        if (document == null || document.Deleted)
            return false;

        if (await IsSystemAdminAsync(customer))
            return true;

        if (document.UploadedByCustomerId == customer.Id)
            return true;

        return document.OwnerVendorId.HasValue &&
            document.OwnerVendorId == customer.VendorId &&
            await HasUnitDocumentManageAccessAsync(customer);
    }

    public async Task<IPagedList<Document>> SearchAdminAsync(
        string? title = null,
        string? code = null,
        int? categoryId = null,
        int? typeId = null,
        int? issuerId = null,
        int? ownerVendorId = null,
        int accessScopeId = 0,
        int publishedId = 0,
        int pageIndex = 0,
        int pageSize = 20,
        Customer? customer = null)
    {
        var query = _documentRepository.Table.Where(x => !x.Deleted);

        if (customer == null)
            return new PagedList<Document>(new List<Document>(), pageIndex, pageSize);

        if (await IsSystemAdminAsync(customer))
        {
            if (ownerVendorId.HasValue)
                query = query.Where(x => x.OwnerVendorId == ownerVendorId.Value);
        }
        else if (await HasUnitDocumentManageAccessAsync(customer))
            query = query.Where(x => x.OwnerVendorId == customer.VendorId);
        else
            query = query.Where(x => x.UploadedByCustomerId == customer.Id);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(x => x.Code != null && x.Code.Contains(code));

        if (categoryId.HasValue)
            query = query.Where(x => x.DocumentCategoryId == categoryId.Value);

        if (typeId.HasValue)
            query = query.Where(x => x.DocumentTypeId == typeId.Value);

        if (issuerId.HasValue)
            query = query.Where(x => x.IssuerId == issuerId.Value);

        if (accessScopeId > 0)
        {
            if (accessScopeId == (int)DocumentAccessScope.Public)
                query = query.Where(x => x.AccessScopeId == (int)DocumentAccessScope.Public);
            else
                query = query.Where(x => x.AccessScopeId != (int)DocumentAccessScope.Public);
        }

        if (publishedId == 1)
            query = query.Where(x => x.Published);
        else if (publishedId == 2)
            query = query.Where(x => !x.Published);

        query = query.OrderByDescending(x => x.CreatedOnUtc);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _documentRepository.GetByIdAsync(id);
    }

    public async Task<Document?> GetBySlugAsync(string slug)
    {
        return await _documentRepository.Table.FirstOrDefaultAsync(x => x.Slug == slug && !x.Deleted);
    }

    public async Task InsertAsync(Document document)
    {
        document.CreatedOnUtc = DateTime.UtcNow;
        document.UpdatedOnUtc = DateTime.UtcNow;
        await _documentRepository.InsertAsync(document);
    }

    public async Task UpdateAsync(Document document)
    {
        document.UpdatedOnUtc = DateTime.UtcNow;
        await _documentRepository.UpdateAsync(document);
    }

    public async Task DeleteAsync(Document document)
    {
        document.Deleted = true;
        document.UpdatedOnUtc = DateTime.UtcNow;
        await _documentRepository.UpdateAsync(document);
    }

    public async Task<bool> IsDownloadInUseAsync(int downloadId)
    {
        return await _documentRepository.Table.AnyAsync(document => document.DownloadId == downloadId && !document.Deleted);
    }

    public async Task IncrementViewCountAsync(Document document)
    {
        document.ViewCount += 1;
        document.UpdatedOnUtc = DateTime.UtcNow;
        await _documentRepository.UpdateAsync(document);
    }

    public async Task IncrementDownloadCountAsync(Document document)
    {
        document.DownloadCount += 1;
        document.UpdatedOnUtc = DateTime.UtcNow;
        await _documentRepository.UpdateAsync(document);
    }

    public async Task<IList<DocumentComment>> GetDocumentCommentsAsync(int documentId, bool? approved = true)
    {
        return await _documentCommentRepository.GetAllAsync(query =>
        {
            query = query.Where(comment => comment.DocumentId == documentId);

            if (approved.HasValue)
                query = query.Where(comment => comment.IsApproved == approved.Value);

            return query.OrderBy(comment => comment.CreatedOnUtc);
        });
    }

    public async Task<DocumentComment?> GetDocumentCommentByIdAsync(int commentId)
    {
        return await _documentCommentRepository.GetByIdAsync(commentId);
    }

    public async Task<int> GetDocumentCommentsCountAsync(int documentId, bool? approved = true)
    {
        var query = _documentCommentRepository.Table.Where(comment => comment.DocumentId == documentId);

        if (approved.HasValue)
            query = query.Where(comment => comment.IsApproved == approved.Value);

        return await query.CountAsync();
    }

    public async Task InsertDocumentCommentAsync(DocumentComment comment)
    {
        await _documentCommentRepository.InsertAsync(comment);
    }

    public async Task<bool> CanDeleteAsync(Document document, Customer customer)
    {
        return await CanManageDocumentAsync(document, customer);
    }

    public async Task<bool> IsDocumentInVendorScopeAsync(Document document, Customer customer)
    {
        return await CanManageDocumentAsync(document, customer);
    }

    public async Task<IList<Document>> GetRelatedDocumentsAsync(Document currentDocument, int limit = 5, Customer? customer = null)
    {
        var query = _documentRepository.Table
            .Where(x => x.Id != currentDocument.Id && x.Published && !x.Deleted);

        if (currentDocument.DocumentCategoryId.HasValue)
            query = query.Where(x => x.DocumentCategoryId == currentDocument.DocumentCategoryId);
        else if (currentDocument.DocumentTypeId.HasValue)
            query = query.Where(x => x.DocumentTypeId == currentDocument.DocumentTypeId);

        var related = await query
            .OrderByDescending(x => x.IssuedDate)
            .ThenBy(x => x.DisplayOrder)
            .Take(customer == null ? limit : Math.Max(limit * 5, limit))
            .ToListAsync();

        if (customer == null)
            return related.Take(limit).ToList();

        var visibleRelated = new List<Document>();
        foreach (var document in related)
        {
            if (await CanViewDocumentAsync(document, customer))
                visibleRelated.Add(document);

            if (visibleRelated.Count >= limit)
                break;
        }

        return visibleRelated;
    }

    public async Task<IList<DocumentCategory>> GetAllCategoriesAsync()
    {
        return await _documentCategoryRepository.Table
            .Where(x => x.Published)
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IList<DocumentCategory>> GetAllCategoriesForAdminAsync()
    {
        return await _documentCategoryRepository.Table
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IList<DocumentType>> GetAllTypesAsync()
    {
        return await _documentTypeRepository.Table
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IList<DocumentType>> GetAllTypesForAdminAsync()
    {
        return await _documentTypeRepository.Table
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IList<DocumentIssuer>> GetAllIssuersAsync()
    {
        return await _documentIssuerRepository.Table
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IList<DocumentIssuer>> GetAllIssuersForAdminAsync()
    {
        return await _documentIssuerRepository.Table
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<DocumentCategory?> GetCategoryByIdAsync(int id)
    {
        return await _documentCategoryRepository.GetByIdAsync(id);
    }

    public async Task<DocumentType?> GetTypeByIdAsync(int id)
    {
        return await _documentTypeRepository.GetByIdAsync(id);
    }

    public async Task<DocumentIssuer?> GetIssuerByIdAsync(int id)
    {
        return await _documentIssuerRepository.GetByIdAsync(id);
    }

    public async Task InsertCategoryAsync(DocumentCategory category)
    {
        await _documentCategoryRepository.InsertAsync(category);
    }

    public async Task UpdateCategoryAsync(DocumentCategory category)
    {
        await _documentCategoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(DocumentCategory category)
    {
        await _documentCategoryRepository.DeleteAsync(category);
    }

    public async Task<bool> IsCategoryInUseAsync(int categoryId)
    {
        return await _documentRepository.Table.AnyAsync(x => !x.Deleted && x.DocumentCategoryId == categoryId);
    }

    public async Task InsertTypeAsync(DocumentType type)
    {
        await _documentTypeRepository.InsertAsync(type);
    }

    public async Task UpdateTypeAsync(DocumentType type)
    {
        await _documentTypeRepository.UpdateAsync(type);
    }

    public async Task DeleteTypeAsync(DocumentType type)
    {
        await _documentTypeRepository.DeleteAsync(type);
    }

    public async Task<bool> IsTypeInUseAsync(int typeId)
    {
        return await _documentRepository.Table.AnyAsync(x => !x.Deleted && x.DocumentTypeId == typeId);
    }

    public async Task InsertIssuerAsync(DocumentIssuer issuer)
    {
        await _documentIssuerRepository.InsertAsync(issuer);
    }

    public async Task UpdateIssuerAsync(DocumentIssuer issuer)
    {
        await _documentIssuerRepository.UpdateAsync(issuer);
    }

    public async Task DeleteIssuerAsync(DocumentIssuer issuer)
    {
        await _documentIssuerRepository.DeleteAsync(issuer);
    }

    public async Task<bool> IsIssuerInUseAsync(int issuerId)
    {
        return await _documentRepository.Table.AnyAsync(x => !x.Deleted && x.IssuerId == issuerId);
    }
}
