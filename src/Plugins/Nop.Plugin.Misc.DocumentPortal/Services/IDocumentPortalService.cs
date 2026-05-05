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
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool showHidden = false,
        Customer? customer = null);

    Task<IPagedList<Document>> SearchAdminAsync(
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
        Customer? customer = null);

    Task<Document?> GetByIdAsync(int id);
    Task<Document?> GetBySlugAsync(string slug);
    Task InsertAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Document document);
    Task<bool> IsDownloadInUseAsync(int downloadId);
    Task IncrementViewCountAsync(Document document);
    Task IncrementDownloadCountAsync(Document document);

    Task<IList<DocumentComment>> GetDocumentCommentsAsync(int documentId, bool? approved = true);
    Task<DocumentComment?> GetDocumentCommentByIdAsync(int commentId);
    Task<int> GetDocumentCommentsCountAsync(int documentId, bool? approved = true);
    Task InsertDocumentCommentAsync(DocumentComment comment);

    Task<bool> CanAccessDocumentAsync(Document document, Customer customer);
    Task<bool> CanViewDocumentAsync(Document document, Customer customer);
    Task<bool> CanManageDocumentAsync(Document document, Customer customer);
    Task<bool> IsSystemAdminAsync(Customer? customer);
    Task<bool> HasUnitDocumentManageAccessAsync(Customer? customer);
    Task<bool> CanAccessDocumentManagementAsync(Customer? customer);

    Task<bool> CanDeleteAsync(Document document, Customer customer);
    Task<bool> IsDocumentInVendorScopeAsync(Document document, Customer customer);

    Task<IList<Document>> GetRelatedDocumentsAsync(Document currentDocument, int limit = 5, Customer? customer = null);

    Task<IList<DocumentCategory>> GetAllCategoriesAsync();
    Task<IList<DocumentType>> GetAllTypesAsync();
    Task<IList<DocumentIssuer>> GetAllIssuersAsync();

    Task<IList<DocumentCategory>> GetAllCategoriesForAdminAsync();
    Task<IList<DocumentType>> GetAllTypesForAdminAsync();
    Task<IList<DocumentIssuer>> GetAllIssuersForAdminAsync();

    Task<DocumentCategory?> GetCategoryByIdAsync(int id);
    Task<DocumentType?> GetTypeByIdAsync(int id);
    Task<DocumentIssuer?> GetIssuerByIdAsync(int id);

    Task InsertCategoryAsync(DocumentCategory category);
    Task UpdateCategoryAsync(DocumentCategory category);
    Task DeleteCategoryAsync(DocumentCategory category);
    Task<bool> IsCategoryInUseAsync(int categoryId);

    Task InsertTypeAsync(DocumentType type);
    Task UpdateTypeAsync(DocumentType type);
    Task DeleteTypeAsync(DocumentType type);
    Task<bool> IsTypeInUseAsync(int typeId);

    Task InsertIssuerAsync(DocumentIssuer issuer);
    Task UpdateIssuerAsync(DocumentIssuer issuer);
    Task DeleteIssuerAsync(DocumentIssuer issuer);
    Task<bool> IsIssuerInUseAsync(int issuerId);
}
