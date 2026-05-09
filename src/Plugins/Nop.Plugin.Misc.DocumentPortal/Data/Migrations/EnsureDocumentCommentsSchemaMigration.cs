using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopSchemaMigration("2026/05/06 10:00:00", "Misc.DocumentPortal ensure document comments schema")]
public class EnsureDocumentCommentsSchemaMigration : MigrationBase
{
    public override void Up()
    {
        if (!Schema.Table(nameof(Document)).Exists())
            return;

        this.CreateTableIfNotExists<DocumentComment>();

        if (!Schema.Table(nameof(DocumentComment)).Index("IX_DocumentComment_DocumentId_IsApproved").Exists())
            Create.Index("IX_DocumentComment_DocumentId_IsApproved").OnTable(nameof(DocumentComment))
                .OnColumn(nameof(DocumentComment.DocumentId)).Ascending()
                .OnColumn(nameof(DocumentComment.IsApproved)).Ascending();
    }

    public override void Down()
    {
    }
}
