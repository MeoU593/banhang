using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-08 15:30:00", "Add wishlist item schema")]
public class AddWishlistItemSchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<WishlistItem>();

        const string indexName = "IX_WishlistItem_Customer_Product";
        var tableName = nameof(WishlistItem);

        if (!Schema.Table(tableName).Index(indexName).Exists())
        {
            Create.Index(indexName)
                .OnTable(tableName)
                .OnColumn(nameof(WishlistItem.CustomerId)).Ascending()
                .OnColumn(nameof(WishlistItem.ProductId)).Ascending()
                .WithOptions().NonClustered();
        }
    }
}
