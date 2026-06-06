using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-24 19:00:00", "Add unit directory entries table")]
public class UnitDirectoryEntrySchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<UnitDirectoryEntry>();

        if (!Schema.Table(nameof(UnitDirectoryEntry)).Index("IX_UnitDirectoryEntry_VendorId_Published").Exists())
            Create.Index("IX_UnitDirectoryEntry_VendorId_Published")
                .OnTable(nameof(UnitDirectoryEntry))
                .OnColumn(nameof(UnitDirectoryEntry.VendorId)).Ascending()
                .OnColumn(nameof(UnitDirectoryEntry.Published)).Ascending();
    }
}
