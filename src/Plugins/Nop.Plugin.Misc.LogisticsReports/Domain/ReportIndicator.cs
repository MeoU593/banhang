using Nop.Core;

namespace Nop.Plugin.Misc.LogisticsReports.Domain;

public class ReportIndicator : BaseEntity
{
    public int ReportTemplateId { get; set; }
    public string Code { get; set; } = string.Empty;
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
    public bool IsActive { get; set; }
}
