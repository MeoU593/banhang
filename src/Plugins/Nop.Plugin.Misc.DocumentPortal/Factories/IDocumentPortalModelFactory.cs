using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using AdminModels = Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using PublicModels = Nop.Plugin.Misc.DocumentPortal.Models.Public;
using VendorModels = Nop.Plugin.Misc.DocumentPortal.Models.Vendor;

namespace Nop.Plugin.Misc.DocumentPortal.Factories;

public interface IDocumentPortalModelFactory
{
    Task<AdminModels.DocumentSearchModel> PrepareDocumentSearchModelAsync(AdminModels.DocumentSearchModel searchModel);
    Task<AdminModels.DocumentListModel> PrepareDocumentListModelAsync(AdminModels.DocumentSearchModel searchModel);
    Task<AdminModels.DocumentModel> PrepareDocumentModelAsync(AdminModels.DocumentModel? model, Document? entity, bool excludeProperties = false);
    Task<AdminModels.ConfigurationModel> PrepareConfigurationModelAsync();

    Task<PublicModels.DocumentListPageModel> PrepareDocumentListPageModelAsync(PublicModels.DocumentSearchModel search, Customer customer);
    Task<PublicModels.DocumentDetailModel> PreparePublicDetailModelAsync(Document entity, Customer customer);
    Task<PublicModels.DocumentUploadModel> PrepareUploadModelAsync(PublicModels.DocumentUploadModel? model = null);

    Task<VendorModels.VendorDocumentSearchModel> PrepareVendorDocumentSearchModelAsync(VendorModels.VendorDocumentSearchModel searchModel, Customer customer);
    Task<VendorModels.VendorDocumentListModel> PrepareVendorDocumentListModelAsync(VendorModels.VendorDocumentSearchModel searchModel, Customer customer);
}
