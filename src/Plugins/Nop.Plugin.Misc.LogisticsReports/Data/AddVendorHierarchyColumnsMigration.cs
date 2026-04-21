using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.LogisticsReports.Data;

[NopMigration("2026/04/20 09:00:00", "Misc.LogisticsReports add vendor hierarchy columns", MigrationProcessType.Update)]
public class AddVendorHierarchyColumnsMigration : AutoReversingMigration
{
    public override void Up()
    {
        const string vendorTable = "Vendor";

        if (!Schema.Table(vendorTable).Column("Code").Exists())
            Alter.Table(vendorTable).AddColumn("Code").AsString(128).Nullable();

        if (!Schema.Table(vendorTable).Column("ParentId").Exists())
            Alter.Table(vendorTable).AddColumn("ParentId").AsInt32().Nullable();

        if (!Schema.Table(vendorTable).Column("Level").Exists())
            Alter.Table(vendorTable).AddColumn("Level").AsInt32().NotNullable().WithDefaultValue(0);

        if (!Schema.Table(vendorTable).Column("Path").Exists())
            Alter.Table(vendorTable).AddColumn("Path").AsString(2000).Nullable();
    }
}
