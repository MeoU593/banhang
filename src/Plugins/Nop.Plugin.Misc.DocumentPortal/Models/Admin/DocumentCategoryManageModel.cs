using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Admin;

public record DocumentCategoryManageModel : BaseNopEntityModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Published { get; set; } = true;
    public int DisplayOrder { get; set; }
}
