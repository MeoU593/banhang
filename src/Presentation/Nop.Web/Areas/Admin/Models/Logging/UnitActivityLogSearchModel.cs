using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Logging;

public partial record UnitActivityLogSearchModel : BaseSearchModel
{
    public UnitActivityLogSearchModel()
    {
        ActivityLogType = new List<SelectListItem>();
    }

    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.ActivityLogType")]
    public int ActivityLogTypeId { get; set; }

    public IList<SelectListItem> ActivityLogType { get; set; }

    [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.CustomerEmail")]
    public string SearchCustomerEmail { get; set; }
}
