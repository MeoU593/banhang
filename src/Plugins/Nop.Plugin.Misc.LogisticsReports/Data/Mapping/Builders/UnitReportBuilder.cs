using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class UnitReportBuilder : NopEntityBuilder<UnitReport>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(UnitReport.ReportPeriodId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReport.OrganizationUnitId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReport.ParentOrganizationUnitId)).AsInt32().Nullable()
            .WithColumn(nameof(UnitReport.StatusId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReport.CurrentVersion)).AsInt32().NotNullable().WithDefaultValue(1)
            .WithColumn(nameof(UnitReport.SubmittedOnUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(UnitReport.SubmittedByCustomerId)).AsInt32().Nullable()
            .WithColumn(nameof(UnitReport.ReturnedReason)).AsString(2000).Nullable()
            .WithColumn(nameof(UnitReport.LockedOnUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(UnitReport.LockedByCustomerId)).AsInt32().Nullable()
            .WithColumn(nameof(UnitReport.LastAggregatedOnUtc)).AsDateTime2().Nullable();
    }
}
