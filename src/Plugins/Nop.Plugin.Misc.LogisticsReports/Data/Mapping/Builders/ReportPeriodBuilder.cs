using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class ReportPeriodBuilder : NopEntityBuilder<ReportPeriod>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ReportPeriod.ReportTemplateId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportPeriod.Code)).AsString(128).NotNullable()
            .WithColumn(nameof(ReportPeriod.Name)).AsString(512).NotNullable()
            .WithColumn(nameof(ReportPeriod.PeriodTypeId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportPeriod.FromDateUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(ReportPeriod.ToDateUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(ReportPeriod.DeadlineUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(ReportPeriod.StatusId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportPeriod.IsLocked)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(ReportPeriod.CreatedOnUtc)).AsDateTime2().NotNullable();
    }
}
