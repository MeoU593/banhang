using Nop.Web.Framework.Models;
using AdminModels = Nop.Plugin.Misc.DocumentPortal.Models.Admin;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Vendor;

public record VendorDocumentListModel : BasePagedListModel<AdminModels.DocumentModel>
{
}
