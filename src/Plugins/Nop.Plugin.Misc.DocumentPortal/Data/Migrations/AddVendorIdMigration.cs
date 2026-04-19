using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopMigration("2026/04/19 10:00:00:0000000", "Misc.DocumentPortal add UploadedByVendorId", MigrationProcessType.Update)]
public class AddVendorIdMigration : AutoReversingMigration
{
    public override void Up()
    {
        if (!Schema.Table("Document").Column("UploadedByVendorId").Exists())
            Alter.Table("Document").AddColumn("UploadedByVendorId").AsInt32().Nullable();
    }
}
