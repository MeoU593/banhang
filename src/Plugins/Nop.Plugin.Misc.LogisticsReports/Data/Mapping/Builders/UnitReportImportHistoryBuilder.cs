using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class UnitReportImportHistoryBuilder : NopEntityBuilder<UnitReportImportHistory>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(UnitReportImportHistory.UnitReportId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportImportHistory.FileName)).AsString(512).NotNullable()
            .WithColumn(nameof(UnitReportImportHistory.StoredFilePath)).AsString(1000).NotNullable()
            .WithColumn(nameof(UnitReportImportHistory.ImportedByCustomerId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitReportImportHistory.ImportedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(UnitReportImportHistory.SuccessCount)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(UnitReportImportHistory.ErrorCount)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(UnitReportImportHistory.ErrorLogPath)).AsString(1000).Nullable();
    }
}
