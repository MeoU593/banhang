using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors;

/// <summary>
/// Represents a vendor associated customer model
/// </summary>
public partial record VendorAssociatedCustomerModel : BaseNopEntityModel
{
    #region Properties

    public string Email { get; set; }

    public string FullName { get; set; }

    public string Phone { get; set; }

    public string Rank { get; set; }

    public string PositionTitle { get; set; }

    public string RoleNames { get; set; }

    public bool IsLeader { get; set; }

    #endregion
}
