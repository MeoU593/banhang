using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.LogisticsReports.Models.ReportTemplates;

public class ReportIndicatorModel
{
    public int Id { get; set; }
    public int ReportTemplateId { get; set; }

    [Required]
    [StringLength(128)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(512)]
    public string Name { get; set; } = string.Empty;
    public int? ParentIndicatorId { get; set; }
    public int DisplayOrder { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int DataTypeId { get; set; }
    public int InputModeId { get; set; }
    public int AggregateMethodId { get; set; }
    public string FormulaExpression { get; set; } = string.Empty;
    public string ExcelSheetName { get; set; } = string.Empty;
    public string ExcelCellAddress { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
}
