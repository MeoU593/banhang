using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-31 09:30:00", "Ensure biography field on unit directory entries")]
public class EnsureUnitDirectoryBiographyColumnMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        if (!Schema.Table(nameof(UnitDirectoryEntry)).Column(nameof(UnitDirectoryEntry.Biography)).Exists())
        {
            Alter.Table(nameof(UnitDirectoryEntry))
                .AddColumn(nameof(UnitDirectoryEntry.Biography))
                .AsString(int.MaxValue)
                .Nullable();
        }
    }
}
