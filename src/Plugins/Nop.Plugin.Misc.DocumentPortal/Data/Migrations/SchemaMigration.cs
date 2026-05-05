using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopMigration("2026/04/17 12:00:00:0000000", "Misc.DocumentPortal schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<Document>();
        this.CreateTableIfNotExists<DocumentComment>();
        this.CreateTableIfNotExists<DocumentCategory>();
        this.CreateTableIfNotExists<DocumentType>();
        this.CreateTableIfNotExists<DocumentIssuer>();

        if (!Schema.Table(nameof(Document)).Index("IX_Document_Published_Deleted").Exists())
            Create.Index("IX_Document_Published_Deleted").OnTable(nameof(Document))
                .OnColumn(nameof(Document.Published)).Ascending()
                .OnColumn(nameof(Document.Deleted)).Ascending();

        if (!Schema.Table(nameof(Document)).Index("IX_Document_Slug").Exists())
            Create.Index("IX_Document_Slug").OnTable(nameof(Document)).OnColumn(nameof(Document.Slug));

        if (!Schema.Table(nameof(Document)).Index("IX_Document_Code").Exists())
            Create.Index("IX_Document_Code").OnTable(nameof(Document)).OnColumn(nameof(Document.Code));
    }
}
