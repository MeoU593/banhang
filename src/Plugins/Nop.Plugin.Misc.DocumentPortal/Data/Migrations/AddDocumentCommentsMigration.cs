using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopMigration("2026/05/05 12:00:00", "Misc.DocumentPortal add document comments", MigrationProcessType.Update)]
public class AddDocumentCommentsMigration : AutoReversingMigration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<DocumentComment>();

        if (!Schema.Table(nameof(DocumentComment)).Index("IX_DocumentComment_DocumentId_IsApproved").Exists())
            Create.Index("IX_DocumentComment_DocumentId_IsApproved").OnTable(nameof(DocumentComment))
                .OnColumn(nameof(DocumentComment.DocumentId)).Ascending()
                .OnColumn(nameof(DocumentComment.IsApproved)).Ascending();
    }
}
