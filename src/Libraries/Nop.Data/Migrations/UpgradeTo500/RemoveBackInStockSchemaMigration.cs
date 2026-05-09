using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-08 15:10:00", "Remove back in stock subscription schema")]
public class RemoveBackInStockSchemaMigration : ForwardOnlyMigration
{
    private const string BackInStockSubscriptionTableName = "BackInStockSubscription";

    public override void Up()
    {
        if (Schema.Table(BackInStockSubscriptionTableName).Exists())
            Delete.Table(BackInStockSubscriptionTableName);

        if (Schema.Table(nameof(Product)).Column("AllowBackInStockSubscriptions").Exists())
            Delete.Column("AllowBackInStockSubscriptions").FromTable(nameof(Product));
    }
}
