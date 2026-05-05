using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Migrations;

[NopMigration("2026/04/26 17:00:00", "Misc.DocumentPortal backfill document uploader", MigrationProcessType.Update)]
public class BackfillDocumentUploaderMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var documentRepository = EngineContext.Current.Resolve<IRepository<Document>>();
        var customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();
        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();

        var documentsToBackfill = documentRepository.Table
            .Where(x => !x.Deleted && !x.UploadedByCustomerId.HasValue)
            .ToList();

        if (!documentsToBackfill.Any())
            return;

        var activeCustomerIds = customerRepository.Table
            .Where(c => !c.Deleted && c.Active)
            .Select(c => c.Id)
            .ToHashSet();

        var managerVendors = vendorRepository.Table
            .Where(v => !v.Deleted && v.Active && v.PmCustomerId.HasValue)
            .OrderBy(v => v.Level)
            .ThenBy(v => v.Id)
            .ToList();

        var rootManagerCustomerId = managerVendors
            .Where(v => !v.ParentId.HasValue || v.ParentId == 0)
            .Select(v => v.PmCustomerId!.Value)
            .FirstOrDefault(activeCustomerIds.Contains);

        var fallbackCustomerId = rootManagerCustomerId > 0
            ? rootManagerCustomerId
            : managerVendors
                .Select(v => v.PmCustomerId!.Value)
                .FirstOrDefault(activeCustomerIds.Contains);

        if (fallbackCustomerId <= 0)
            return;

        foreach (var document in documentsToBackfill)
        {
            document.UploadedByCustomerId = fallbackCustomerId;
            documentRepository.UpdateAsync(document).GetAwaiter().GetResult();
        }
    }

    public override void Down()
    {
    }
}
