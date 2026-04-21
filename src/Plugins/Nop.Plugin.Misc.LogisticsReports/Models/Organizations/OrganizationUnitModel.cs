using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Misc.LogisticsReports.Models.Organizations;

public class OrganizationUnitModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(128)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(512)]
    public string Name { get; set; } = string.Empty;

    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsRootUnit { get; set; }

    public IList<SelectListItem> AvailableParentUnits { get; set; } = new List<SelectListItem>();
}
