using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class UnitReportValueBuilder : NopEntityBuilder<UnitReportValue>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(UnitReportValue.UnitReportId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportValue.ReportIndicatorId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportValue.SelfValueNumber)).AsDecimal(18, 4).Nullable()
            .WithColumn(nameof(UnitReportValue.SelfValueText)).AsString(2000).Nullable()
            .WithColumn(nameof(UnitReportValue.ChildAggregateValueNumber)).AsDecimal(18, 4).Nullable()
            .WithColumn(nameof(UnitReportValue.AdjustmentValueNumber)).AsDecimal(18, 4).Nullable()
            .WithColumn(nameof(UnitReportValue.FinalValueNumber)).AsDecimal(18, 4).Nullable()
            .WithColumn(nameof(UnitReportValue.FinalValueText)).AsString(2000).Nullable()
            .WithColumn(nameof(UnitReportValue.Note)).AsString(2000).Nullable()
            .WithColumn(nameof(UnitReportValue.SourceTypeId)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(UnitReportValue.LastUpdatedByCustomerId)).AsInt32().Nullable()
            .WithColumn(nameof(UnitReportValue.LastUpdatedOnUtc)).AsDateTime2().Nullable();
    }
}
