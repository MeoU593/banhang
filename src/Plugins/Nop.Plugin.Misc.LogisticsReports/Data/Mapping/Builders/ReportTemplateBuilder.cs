using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class ReportTemplateBuilder : NopEntityBuilder<ReportTemplate>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ReportTemplate.Code)).AsString(128).NotNullable()
            .WithColumn(nameof(ReportTemplate.Name)).AsString(512).NotNullable()
            .WithColumn(nameof(ReportTemplate.Description)).AsString(4000).Nullable()
            .WithColumn(nameof(ReportTemplate.PeriodTypeId)).AsInt32().NotNullable()
            .WithColumn(nameof(ReportTemplate.TemplateStoredFileName)).AsString(1000).Nullable()
            .WithColumn(nameof(ReportTemplate.Version)).AsString(64).NotNullable()
            .WithColumn(nameof(ReportTemplate.IsActive)).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(nameof(ReportTemplate.EffectiveFromUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(ReportTemplate.EffectiveToUtc)).AsDateTime2().Nullable();
    }
}
