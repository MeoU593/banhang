using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Admin;

public record DocumentIssuerManageModel : BaseNopEntityModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}
