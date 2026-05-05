using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-04-30 09:20:00", "Remove commerce core schema")]
public class RemoveCommerceCoreSchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var tablesToDrop = new[]
        {
            typeof(ShipmentItem),
            typeof(Shipment),
            typeof(ShippingMethodCountryMapping),
            typeof(ShippingMethod),
            typeof(RecurringPaymentHistory),
            typeof(RecurringPayment),
            typeof(ReturnRequest),
            typeof(ReturnRequestAction),
            typeof(ReturnRequestReason),
            typeof(OrderNote),
            typeof(OrderItem),
            typeof(Order),
            typeof(CheckoutAttributeValue),
            typeof(CheckoutAttribute),
            typeof(GiftCardUsageHistory),
            typeof(GiftCard),
            typeof(ShoppingCartItem),
            typeof(CustomWishlist)
        };

        foreach (var entityType in tablesToDrop)
        {
            var tableName = NameCompatibilityManager.GetTableName(entityType);
            if (!Schema.Table(tableName).Exists())
                continue;

            Delete.Table(tableName);
        }
    }
}
