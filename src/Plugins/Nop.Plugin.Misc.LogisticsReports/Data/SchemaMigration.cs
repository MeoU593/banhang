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
        this.CreateTableIfNotExists<OrganizationUnit>();
        this.CreateTableIfNotExists<CustomerOrganizationUnit>();
        this.CreateTableIfNotExists<ReportTemplate>();
        this.CreateTableIfNotExists<ReportIndicator>();
        this.CreateTableIfNotExists<ReportPeriod>();
        this.CreateTableIfNotExists<UnitReport>();
        this.CreateTableIfNotExists<UnitReportValue>();
        this.CreateTableIfNotExists<UnitReportImportHistory>();
        this.CreateTableIfNotExists<UnitReportWorkflowLog>();

        var organizationTable = NameCompatibilityManager.GetTableName(typeof(OrganizationUnit));
        var customerOrganizationTable = NameCompatibilityManager.GetTableName(typeof(CustomerOrganizationUnit));
        var reportIndicatorTable = NameCompatibilityManager.GetTableName(typeof(ReportIndicator));
        var reportPeriodTable = NameCompatibilityManager.GetTableName(typeof(ReportPeriod));
        var unitReportTable = NameCompatibilityManager.GetTableName(typeof(UnitReport));
        var unitReportValueTable = NameCompatibilityManager.GetTableName(typeof(UnitReportValue));

        if (!Schema.Table(organizationTable).Index("IX_LR_OrganizationUnit_ParentId").Exists())
            Create.Index("IX_LR_OrganizationUnit_ParentId").OnTable(organizationTable).OnColumn(nameof(OrganizationUnit.ParentId));

        if (!Schema.Table(organizationTable).Index("IX_LR_OrganizationUnit_Path").Exists())
            Create.Index("IX_LR_OrganizationUnit_Path").OnTable(organizationTable).OnColumn(nameof(OrganizationUnit.Path));

        if (!Schema.Table(customerOrganizationTable).Index("IX_LR_CustomerOrganizationUnit_CustomerId").Exists())
            Create.Index("IX_LR_CustomerOrganizationUnit_CustomerId").OnTable(customerOrganizationTable).OnColumn(nameof(CustomerOrganizationUnit.CustomerId));

        if (!Schema.Table(customerOrganizationTable).Index("IX_LR_CustomerOrganizationUnit_OrganizationUnitId").Exists())
            Create.Index("IX_LR_CustomerOrganizationUnit_OrganizationUnitId").OnTable(customerOrganizationTable).OnColumn(nameof(CustomerOrganizationUnit.OrganizationUnitId));

        if (!Schema.Table(reportIndicatorTable).Index("IX_LR_ReportIndicator_ReportTemplateId").Exists())
            Create.Index("IX_LR_ReportIndicator_ReportTemplateId").OnTable(reportIndicatorTable).OnColumn(nameof(ReportIndicator.ReportTemplateId));

        if (!Schema.Table(reportPeriodTable).Index("IX_LR_ReportPeriod_ReportTemplateId").Exists())
            Create.Index("IX_LR_ReportPeriod_ReportTemplateId").OnTable(reportPeriodTable).OnColumn(nameof(ReportPeriod.ReportTemplateId));

        if (!Schema.Table(unitReportTable).Index("IX_LR_UnitReport_ReportPeriodId").Exists())
            Create.Index("IX_LR_UnitReport_ReportPeriodId").OnTable(unitReportTable).OnColumn(nameof(UnitReport.ReportPeriodId));

        if (!Schema.Table(unitReportTable).Index("IX_LR_UnitReport_OrganizationUnitId").Exists())
            Create.Index("IX_LR_UnitReport_OrganizationUnitId").OnTable(unitReportTable).OnColumn(nameof(UnitReport.OrganizationUnitId));

        if (!Schema.Table(unitReportValueTable).Index("IX_LR_UnitReportValue_UnitReportId").Exists())
            Create.Index("IX_LR_UnitReportValue_UnitReportId").OnTable(unitReportValueTable).OnColumn(nameof(UnitReportValue.UnitReportId));

        if (!Schema.Table(unitReportValueTable).Index("IX_LR_UnitReportValue_ReportIndicatorId").Exists())
            Create.Index("IX_LR_UnitReportValue_ReportIndicatorId").OnTable(unitReportValueTable).OnColumn(nameof(UnitReportValue.ReportIndicatorId));
    }
}
