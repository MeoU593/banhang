using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-04-20 23:10:00", "5.00", UpdateMigrationType.Data)]
public class ProductAdminSimplificationDataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public ProductAdminSimplificationDataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        var removedSettingKeys = new[]
        {
            "ProductEditorSettings.ManufacturerPartNumber",
            "ProductEditorSettings.GTIN",
            "ProductEditorSettings.RequireOtherProductsAddedToCart",
            "ProductEditorSettings.AgeVerification",
            "ProductEditorSettings.ProductTags",
            "ProductEditorSettings.ProductType",
            "ProductEditorSettings.ProductTemplate",
            "ProductEditorSettings.VisibleIndividually",
            "ProductEditorSettings.Stores",
            "ProductEditorSettings.ACL",
            "ProductEditorSettings.IsGiftCard",
            "ProductEditorSettings.DownloadableProduct",
            "ProductEditorSettings.IsRental",
            "ProductEditorSettings.RecurringProduct",
            "ProductEditorSettings.ShipSeparately",
            "ProductEditorSettings.AdditionalShippingCharge",
            "ProductEditorSettings.DeliveryDate",
            "ProductEditorSettings.ProductAttributes",
            "ProductEditorSettings.SpecificationAttributes",
            "ProductEditorSettings.CrossSellsProducts",
            "ProductEditorSettings.PurchasedWithOrders"
        };

        var settingRowsToDelete = _dataProvider.GetTable<Setting>()
            .Where(setting => removedSettingKeys.Contains(setting.Name))
            .ToList();

        if (settingRowsToDelete.Count > 0)
            _dataProvider.BulkDeleteEntities(settingRowsToDelete);

        var removedCustomerAttributeKeys = new[]
        {
            "ProductPage.HideShippingBlock",
            "ProductPage.HideProductAttributesBlock",
            "ProductPage.HideSpecificationAttributeBlock",
            "ProductPage.HideGiftCardBlock",
            "ProductPage.HideDownloadableBlock",
            "ProductPage.HideRentalBlock",
            "ProductPage.HideRecurringBlock",
            "ProductPage.HideCrossSellsProductsBlock",
            "ProductPage.HidePurchasedWithOrdersBlock"
        };

        var genericAttributesToDelete = _dataProvider.GetTable<GenericAttribute>()
            .Where(attribute => attribute.KeyGroup == nameof(Customer) && removedCustomerAttributeKeys.Contains(attribute.Key))
            .ToList();

        if (genericAttributesToDelete.Count > 0)
            _dataProvider.BulkDeleteEntities(genericAttributesToDelete);
    }

    public override void Down()
    {
    }
}
