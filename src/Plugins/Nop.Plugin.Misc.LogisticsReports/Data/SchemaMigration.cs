using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data;

[NopMigration("2026/04/19 00:00:00", "Misc.LogisticsReports base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<ReportTemplate>();
        this.CreateTableIfNotExists<ReportIndicator>();
        this.CreateTableIfNotExists<ReportPeriod>();
        this.CreateTableIfNotExists<UnitReport>();
        this.CreateTableIfNotExists<UnitReportValue>();
        this.CreateTableIfNotExists<UnitReportImportHistory>();
        this.CreateTableIfNotExists<UnitReportWorkflowLog>();

        var reportIndicatorTable = NameCompatibilityManager.GetTableName(typeof(ReportIndicator));
        var reportPeriodTable = NameCompatibilityManager.GetTableName(typeof(ReportPeriod));
        var unitReportTable = NameCompatibilityManager.GetTableName(typeof(UnitReport));
        var unitReportValueTable = NameCompatibilityManager.GetTableName(typeof(UnitReportValue));

        if (!Schema.Table(reportIndicatorTable).Index("IX_LR_ReportIndicator_ReportTemplateId").Exists())
            Create.Index("IX_LR_ReportIndicator_ReportTemplateId").OnTable(reportIndicatorTable).OnColumn(nameof(ReportIndicator.ReportTemplateId));

        if (!Schema.Table(reportPeriodTable).Index("IX_LR_ReportPeriod_ReportTemplateId").Exists())
            Create.Index("IX_LR_ReportPeriod_ReportTemplateId").OnTable(reportPeriodTable).OnColumn(nameof(ReportPeriod.ReportTemplateId));

        if (!Schema.Table(unitReportTable).Index("IX_LR_UnitReport_ReportPeriodId").Exists())
            Create.Index("IX_LR_UnitReport_ReportPeriodId").OnTable(unitReportTable).OnColumn(nameof(UnitReport.ReportPeriodId));

        if (!Schema.Table(unitReportTable).Index("IX_LR_UnitReport_VendorId").Exists())
            Create.Index("IX_LR_UnitReport_VendorId").OnTable(unitReportTable).OnColumn(nameof(UnitReport.VendorId));

        if (!Schema.Table(unitReportValueTable).Index("IX_LR_UnitReportValue_UnitReportId").Exists())
            Create.Index("IX_LR_UnitReportValue_UnitReportId").OnTable(unitReportValueTable).OnColumn(nameof(UnitReportValue.UnitReportId));

        if (!Schema.Table(unitReportValueTable).Index("IX_LR_UnitReportValue_ReportIndicatorId").Exists())
            Create.Index("IX_LR_UnitReportValue_ReportIndicatorId").OnTable(unitReportValueTable).OnColumn(nameof(UnitReportValue.ReportIndicatorId));
    }
}
