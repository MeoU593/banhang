using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-04-20 22:40:00", "Cleanup product schema for simplified admin fields")]
public class ProductAdminSimplificationSchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var productTableName = nameof(Product);
        var productSearchIndexName = "IX_Product_Search";

        if (Schema.Table(productTableName).Index(productSearchIndexName).Exists())
            Delete.Index(productSearchIndexName).OnTable(productTableName);

        this.DeleteColumnsIfExists<Product>(
        [
            nameof(Product.ManufacturerPartNumber),
            nameof(Product.Gtin),
            nameof(Product.RequireOtherProducts),
            nameof(Product.RequiredProductIds),
            nameof(Product.AutomaticallyAddRequiredProducts),
            nameof(Product.AgeVerification),
            nameof(Product.MinimumAgeToPurchase)
        ]);

        const string databaseType = "sqlserver";
        if (!Schema.Table(productTableName).Index(productSearchIndexName).Exists())
        {
            IfDatabase(databaseType).Create.Index(productSearchIndexName)
                .OnTable(productTableName)
                .OnColumn(nameof(Product.Name)).Ascending()
                .OnColumn(nameof(Product.Sku)).Ascending()
                .OnColumn(nameof(Product.Deleted)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(Product.Id));
        }
    }
}
