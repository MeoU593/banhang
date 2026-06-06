using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-31 09:00:00", "Add biography fields to unit directory entries")]
public class AddUnitDirectoryBiographyMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        AddTextColumnIfMissing(nameof(UnitDirectoryEntry.Biography));
    }

    protected virtual void AddTextColumnIfMissing(string columnName)
    {
        if (!Schema.Table(nameof(UnitDirectoryEntry)).Column(columnName).Exists())
        {
            Alter.Table(nameof(UnitDirectoryEntry))
                .AddColumn(columnName)
                .AsString(int.MaxValue)
                .Nullable();
        }
    }
}
