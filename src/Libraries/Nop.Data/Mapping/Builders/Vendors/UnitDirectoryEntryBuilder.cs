using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Builders.Vendors;

public partial class UnitDirectoryEntryBuilder : NopEntityBuilder<UnitDirectoryEntry>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(UnitDirectoryEntry.VendorId)).AsInt32().NotNullable()
            .WithColumn(nameof(UnitDirectoryEntry.FullName)).AsString(255).NotNullable()
            .WithColumn(nameof(UnitDirectoryEntry.Rank)).AsString(255).Nullable()
            .WithColumn(nameof(UnitDirectoryEntry.PositionTitle)).AsString(255).Nullable()
            .WithColumn(nameof(UnitDirectoryEntry.Phone)).AsString(100).Nullable()
            .WithColumn(nameof(UnitDirectoryEntry.MilitaryCode)).AsString(100).Nullable()
            .WithColumn(nameof(UnitDirectoryEntry.UnitName)).AsString(500).Nullable()
            .WithColumn(nameof(UnitDirectoryEntry.Biography)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(UnitDirectoryEntry.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(UnitDirectoryEntry.Published)).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(nameof(UnitDirectoryEntry.CreatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(UnitDirectoryEntry.UpdatedOnUtc)).AsDateTime2().NotNullable();
    }
}
