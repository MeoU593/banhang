using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Services;

public interface IDocumentPortalService
{
    Task<IPagedList<Document>> SearchAsync(
        string? keywords = null,
        int? categoryId = null,
        int? typeId = null,
        int? issuerId = null,
        int? year = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool showHidden = false);

    Task<IPagedList<Document>> SearchAdminAsync(
        string? title = null,
        string? code = null,
        int? categoryId = null,
        int? typeId = null,
        int? issuerId = null,
        int publishedId = 0,
        int pageIndex = 0,
        int pageSize = 20);

    Task<IPagedList<Document>> SearchVendorAsync(
        int vendorId,
        string? keywords = null,
        int? categoryId = null,
        int? typeId = null,
        int publishedId = 0,
        int pageIndex = 0,
        int pageSize = 20);

    Task<bool> IsVendorManagerAsync(int vendorId, Customer customer);

    Task<Document?> GetByIdAsync(int id);
    Task<Document?> GetBySlugAsync(string slug);
    Task InsertAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Document document);
    Task IncrementViewCountAsync(Document document);
    Task IncrementDownloadCountAsync(Document document);

    Task<bool> CanDeleteAsync(Document document, Customer customer);

    Task SaveRoleMappingsAsync(int documentId, IList<int> selectedRoleIds);
    Task<IList<int>> GetRoleMappingIdsAsync(int documentId);
    Task<IList<Document>> GetRelatedDocumentsAsync(Document currentDocument, int limit = 5);

    Task<IList<DocumentCategory>> GetAllCategoriesAsync();
    Task<IList<DocumentType>> GetAllTypesAsync();
    Task<IList<DocumentIssuer>> GetAllIssuersAsync();
}
