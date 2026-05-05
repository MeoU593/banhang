using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopMigration("2026/04/26 22:30:00", "Misc.DocumentPortal add document access scope", MigrationProcessType.Update)]
public class AddDocumentAccessScopeMigration : MigrationBase
{
    public override void Up()
    {
        if (!Schema.Table(nameof(Document)).Column(nameof(Document.OwnerVendorId)).Exists())
            Alter.Table(nameof(Document)).AddColumn(nameof(Document.OwnerVendorId)).AsInt32().Nullable();

        if (!Schema.Table(nameof(Document)).Column(nameof(Document.AccessScopeId)).Exists())
            Alter.Table(nameof(Document)).AddColumn(nameof(Document.AccessScopeId)).AsInt32().NotNullable().WithDefaultValue((int)DocumentAccessScope.Public);

        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var documentRepository = EngineContext.Current.Resolve<IRepository<Document>>();
        var customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();

        var documents = documentRepository.Table.Where(d => !d.Deleted).ToList();
        foreach (var document in documents)
        {
            if (document.UploadedByCustomerId.HasValue)
            {
                var vendorId = customerRepository.Table
                    .Where(c => c.Id == document.UploadedByCustomerId.Value)
                    .Select(c => c.VendorId)
                    .FirstOrDefault();

                document.OwnerVendorId = vendorId > 0 ? vendorId : null;
            }

            document.AccessScopeId = (int)DocumentAccessScope.Public;
            documentRepository.UpdateAsync(document).GetAwaiter().GetResult();
        }
    }

    public override void Down()
    {
    }
}
