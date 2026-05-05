using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-04-27 16:20:00", "Add CreatedByCustomerId column to product")]
public class AddProductCreatedByCustomerSchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var tableName = nameof(Product);
        var columnName = nameof(Product.CreatedByCustomerId);

        if (Schema.Table(tableName).Column(columnName).Exists())
            return;

        Alter.Table(tableName)
            .AddColumn(columnName)
            .AsInt32()
            .NotNullable()
            .SetExistingRowsTo(0);
    }
}
