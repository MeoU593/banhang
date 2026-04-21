using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class ReportIndicatorBuilder : NopEntityBuilder<ReportIndicator>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ReportIndicator.ReportTemplateId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportIndicator.Code)).AsString(128).NotNullable()
            .WithColumn(nameof(ReportIndicator.Name)).AsString(512).NotNullable()
            .WithColumn(nameof(ReportIndicator.ParentIndicatorId)).AsInt32().Nullable()
            .WithColumn(nameof(ReportIndicator.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(ReportIndicator.UnitOfMeasure)).AsString(128).Nullable()
            .WithColumn(nameof(ReportIndicator.DataTypeId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportIndicator.InputModeId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportIndicator.AggregateMethodId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportIndicator.FormulaExpression)).AsString(1000).Nullable()
            .WithColumn(nameof(ReportIndicator.ExcelSheetName)).AsString(128).Nullable()
            .WithColumn(nameof(ReportIndicator.ExcelCellAddress)).AsString(32).Nullable()
            .WithColumn(nameof(ReportIndicator.IsRequired)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(ReportIndicator.IsActive)).AsBoolean().NotNullable().WithDefaultValue(true);
    }
}
