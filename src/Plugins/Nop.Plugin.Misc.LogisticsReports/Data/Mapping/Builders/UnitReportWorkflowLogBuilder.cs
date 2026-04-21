using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class UnitReportWorkflowLogBuilder : NopEntityBuilder<UnitReportWorkflowLog>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(UnitReportWorkflowLog.UnitReportId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportWorkflowLog.Action)).AsString(128).NotNullable()
            .WithColumn(nameof(UnitReportWorkflowLog.FromStatusId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportWorkflowLog.ToStatusId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportWorkflowLog.ActionByCustomerId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportWorkflowLog.ActionOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(UnitReportWorkflowLog.Comment)).AsString(2000).Nullable();
    }
}
