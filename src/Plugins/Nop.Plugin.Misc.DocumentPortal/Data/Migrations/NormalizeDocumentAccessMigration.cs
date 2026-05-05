using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopMigration("2026/04/29 11:20:00", "Misc.DocumentPortal normalize document access", MigrationProcessType.Update)]
public class NormalizeDocumentAccessMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var documentRepository = EngineContext.Current.Resolve<IRepository<Document>>();
        var documents = documentRepository.Table.Where(x => !x.Deleted).ToList();

        foreach (var document in documents)
        {
            document.AccessScopeId = document.AccessScopeId == (int)DocumentAccessScope.Public
                ? (int)DocumentAccessScope.Public
                : (int)DocumentAccessScope.Internal;
            document.AllowDownload = true;
            documentRepository.UpdateAsync(document).GetAwaiter().GetResult();
        }
    }

    public override void Down()
    {
    }
}
