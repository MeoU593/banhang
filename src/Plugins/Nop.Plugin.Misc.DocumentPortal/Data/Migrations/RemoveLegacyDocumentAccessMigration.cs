using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopMigration("2026/05/04 18:00:00", "Misc.DocumentPortal remove legacy document access", MigrationProcessType.Update)]
public class RemoveLegacyDocumentAccessMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        if (Schema.Table(nameof(Document)).Column("RequiredViewerTypeId").Exists())
            Delete.Column("RequiredViewerTypeId").FromTable(nameof(Document));

        if (Schema.Table("DocumentCustomerRoleMapping").Exists())
            Delete.Table("DocumentCustomerRoleMapping");
    }
}
