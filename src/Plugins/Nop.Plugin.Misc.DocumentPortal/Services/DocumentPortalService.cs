using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Services.Security;
using Nop.Services.Vendors;

namespace Nop.Plugin.Misc.DocumentPortal.Services;

public class DocumentPortalService : IDocumentPortalService
{
    private readonly IRepository<Document> _documentRepository;
    private readonly IRepository<DocumentCategory> _documentCategoryRepository;
    private readonly IRepository<DocumentType> _documentTypeRepository;
    private readonly IRepository<DocumentIssuer> _documentIssuerRepository;
    private readonly IRepository<DocumentCustomerRoleMapping> _documentRoleMappingRepository;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IPermissionService _permissionService;

    public DocumentPortalService(
        IRepository<Document> documentRepository,
        IRepository<DocumentCategory> documentCategoryRepository,
        IRepository<DocumentType> documentTypeRepository,
        IRepository<DocumentIssuer> documentIssuerRepository,
        IRepository<DocumentCustomerRoleMapping> documentRoleMappingRepository,
        IRepository<Vendor> vendorRepository,
        IPermissionService permissionService)
    {
        _documentRepository = documentRepository;
        _documentCategoryRepository = documentCategoryRepository;
        _documentTypeRepository = documentTypeRepository;
        _documentIssuerRepository = documentIssuerRepository;
        _documentRoleMappingRepository = documentRoleMappingRepository;
        _vendorRepository = vendorRepository;
        _permissionService = permissionService;
    }

    public async Task<IPagedList<Document>> SearchAsync(
        string? keywords = null,
        int? categoryId = null,
        int? typeId = null,
        int? issuerId = null,
        int? year = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool showHidden = false)
    {
        var query = _documentRepository.Table;

        if (!showHidden)
            query = query.Where(x => x.Published && !x.Deleted);

        if (categoryId.HasValue)
            query = query.Where(x => x.DocumentCategoryId == categoryId.Value);

        if (typeId.HasValue)
            query = query.Where(x => x.DocumentTypeId == typeId.Value);

        if (issuerId.HasValue)
            query = query.Where(x => x.IssuerId == issuerId.Value);

        if (year.HasValue)
            query = query.Where(x => x.IssuedDate.HasValue && x.IssuedDate.Value.Year == year.Value);

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

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<IPagedList<Document>> SearchAdminAsync(
        string? title = null,
        string? code = null,
        int? categoryId = null,
        int? typeId = null,
        int? issuerId = null,
        int publishedId = 0,
        int pageIndex = 0,
        int pageSize = 20)
    {
        var query = _documentRepository.Table.Where(x => !x.Deleted);

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

    public async Task<bool> CanDeleteAsync(Document document, Customer customer)
    {
        if (await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENTS, customer))
            return true;

        if (document.UploadedByCustomerId.HasValue && document.UploadedByCustomerId == customer.Id)
            return true;

        if (document.UploadedByVendorId.HasValue && customer.VendorId > 0 &&
            document.UploadedByVendorId == customer.VendorId &&
            await IsVendorManagerAsync(customer.VendorId, customer))
            return true;

        return false;
    }

    public async Task<bool> IsVendorManagerAsync(int vendorId, Customer customer)
    {
        if (vendorId <= 0 || customer.VendorId != vendorId)
            return false;

        var vendor = await _vendorRepository.Table
            .FirstOrDefaultAsync(v => v.Id == vendorId && !v.Deleted);
        return vendor?.PmCustomerId == customer.Id;
    }

    public async Task<IPagedList<Document>> SearchVendorAsync(
        int vendorId,
        string? keywords = null,
        int? categoryId = null,
        int? typeId = null,
        int publishedId = 0,
        int pageIndex = 0,
        int pageSize = 20)
    {
        var query = _documentRepository.Table
            .Where(x => !x.Deleted && x.UploadedByVendorId == vendorId);

        if (!string.IsNullOrWhiteSpace(keywords))
        {
            var q = keywords.Trim();
            query = query.Where(x =>
                (x.Code != null && x.Code.Contains(q)) ||
                x.Title.Contains(q) ||
                (x.Summary != null && x.Summary.Contains(q)));
        }

        if (categoryId.HasValue && categoryId > 0)
            query = query.Where(x => x.DocumentCategoryId == categoryId.Value);

        if (typeId.HasValue && typeId > 0)
            query = query.Where(x => x.DocumentTypeId == typeId.Value);

        if (publishedId == 1)
            query = query.Where(x => x.Published);
        else if (publishedId == 2)
            query = query.Where(x => !x.Published);

        query = query.OrderByDescending(x => x.CreatedOnUtc);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task SaveRoleMappingsAsync(int documentId, IList<int> selectedRoleIds)
    {
        var existing = await _documentRoleMappingRepository.Table
            .Where(x => x.DocumentId == documentId).ToListAsync();

        foreach (var item in existing)
            await _documentRoleMappingRepository.DeleteAsync(item);

        selectedRoleIds ??= new List<int>();
        foreach (var roleId in selectedRoleIds.Distinct())
        {
            await _documentRoleMappingRepository.InsertAsync(new DocumentCustomerRoleMapping
            {
                DocumentId = documentId,
                CustomerRoleId = roleId
            });
        }
    }

    public async Task<IList<int>> GetRoleMappingIdsAsync(int documentId)
    {
        return await _documentRoleMappingRepository.Table
            .Where(x => x.DocumentId == documentId)
            .Select(x => x.CustomerRoleId)
            .ToListAsync();
    }

    public async Task<IList<Document>> GetRelatedDocumentsAsync(Document currentDocument, int limit = 5)
    {
        var query = _documentRepository.Table
            .Where(x => x.Id != currentDocument.Id && x.Published && !x.Deleted);

        if (currentDocument.DocumentCategoryId.HasValue)
            query = query.Where(x => x.DocumentCategoryId == currentDocument.DocumentCategoryId);
        else if (currentDocument.DocumentTypeId.HasValue)
            query = query.Where(x => x.DocumentTypeId == currentDocument.DocumentTypeId);

        return await query
            .OrderByDescending(x => x.IssuedDate)
            .ThenBy(x => x.DisplayOrder)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IList<DocumentCategory>> GetAllCategoriesAsync()
    {
        return await _documentCategoryRepository.Table
            .Where(x => x.Published)
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IList<DocumentType>> GetAllTypesAsync()
    {
        return await _documentTypeRepository.Table
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IList<DocumentIssuer>> GetAllIssuersAsync()
    {
        return await _documentIssuerRepository.Table
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name)
            .ToListAsync();
    }
}
