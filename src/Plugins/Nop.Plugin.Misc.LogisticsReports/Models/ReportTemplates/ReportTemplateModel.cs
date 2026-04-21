using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.LogisticsReports.Models.ReportTemplates;

public class ReportTemplateModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(128)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(512)]
    public string Name { get; set; } = string.Empty;

    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;
    public int PeriodTypeId { get; set; }

    [Required]
    [StringLength(64)]
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
    public DateTime? EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    public IFormFile? TemplateFile { get; set; }
    public string ExistingTemplateFileName { get; set; } = string.Empty;
}
