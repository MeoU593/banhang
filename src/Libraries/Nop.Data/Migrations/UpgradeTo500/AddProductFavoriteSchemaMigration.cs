using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-04-30 10:30:00", "Add product favorite schema")]
public class AddProductFavoriteSchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<ProductFavorite>();

        const string indexName = "IX_ProductFavorite_Customer_Product";
        var tableName = nameof(ProductFavorite);

        if (!Schema.Table(tableName).Index(indexName).Exists())
        {
            Create.Index(indexName)
                .OnTable(tableName)
                .OnColumn(nameof(ProductFavorite.CustomerId)).Ascending()
                .OnColumn(nameof(ProductFavorite.ProductId)).Ascending()
                .WithOptions().Unique();
        }
    }
}
