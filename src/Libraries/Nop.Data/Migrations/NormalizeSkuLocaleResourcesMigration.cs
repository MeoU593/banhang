using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/28 09:00:00", "Normalize SKU locale resources", MigrationProcessType.Update)]
public class NormalizeSkuLocaleResourcesMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('LocaleStringResource', 'U') IS NOT NULL
            BEGIN
                UPDATE [LocaleStringResource]
                SET [ResourceValue] = CASE LOWER([ResourceName])
                    WHEN N'admin.catalog.products.fields.sku' THEN N'Mã sản phẩm'
                    WHEN N'admin.catalog.products.fields.sku.hint' THEN N'Mã sản phẩm nội bộ dùng để nhận diện và theo dõi sản phẩm.'
                    WHEN N'admin.catalog.products.fields.sku.reserved' THEN N'Mã sản phẩm đã được dùng cho sản phẩm ''{0}'''
                    WHEN N'admin.catalog.products.list.godirectlytosku' THEN N'Đi thẳng tới mã sản phẩm'
                    WHEN N'admin.catalog.products.list.godirectlytosku.hint' THEN N'Nhập mã sản phẩm và bấm đi tới.'
                    WHEN N'admin.catalog.products.productattributes.attributecombinations.fields.sku' THEN N'Mã sản phẩm'
                    WHEN N'admin.catalog.products.productattributes.attributecombinations.fields.sku.hint' THEN N'Mã sản phẩm nội bộ dùng để theo dõi tổ hợp thuộc tính này.'
                    WHEN N'admin.catalog.products.productattributes.attributecombinations.fields.sku.reserved' THEN N'Mã sản phẩm đã được dùng cho một tổ hợp của sản phẩm ''{0}'''
                    WHEN N'admin.configuration.settings.catalog.showskuoncatalogpages' THEN N'Hiển thị mã sản phẩm trên trang danh mục'
                    WHEN N'admin.configuration.settings.catalog.showskuoncatalogpages.hint' THEN N'Chọn để hiển thị mã sản phẩm trên các trang danh mục ngoài public.'
                    WHEN N'admin.configuration.settings.catalog.showskuonproductdetailspage' THEN N'Hiển thị mã sản phẩm trên trang chi tiết'
                    WHEN N'admin.configuration.settings.catalog.showskuonproductdetailspage.hint' THEN N'Chọn để hiển thị mã sản phẩm trên trang chi tiết ngoài public.'
                    WHEN N'admin.orders.products.addnew.sku' THEN N'Mã sản phẩm'
                    WHEN N'admin.orders.products.sku' THEN N'Mã sản phẩm'
                    WHEN N'admin.orders.shipments.products.sku' THEN N'Mã sản phẩm'
                    WHEN N'messages.order.product(s).sku' THEN N'Mã sản phẩm: {0}'
                    WHEN N'order.product(s).sku' THEN N'Mã sản phẩm'
                    WHEN N'order.shipments.product(s).sku' THEN N'Mã sản phẩm'
                    WHEN N'pdf.product.sku' THEN N'Mã sản phẩm'
                    WHEN N'products.sku' THEN N'Mã sản phẩm'
                    WHEN N'shoppingcart.sku' THEN N'Mã sản phẩm'
                    ELSE [ResourceValue]
                END
                WHERE LOWER([ResourceName]) IN
                (
                    N'admin.catalog.products.fields.sku',
                    N'admin.catalog.products.fields.sku.hint',
                    N'admin.catalog.products.fields.sku.reserved',
                    N'admin.catalog.products.list.godirectlytosku',
                    N'admin.catalog.products.list.godirectlytosku.hint',
                    N'admin.catalog.products.productattributes.attributecombinations.fields.sku',
                    N'admin.catalog.products.productattributes.attributecombinations.fields.sku.hint',
                    N'admin.catalog.products.productattributes.attributecombinations.fields.sku.reserved',
                    N'admin.configuration.settings.catalog.showskuoncatalogpages',
                    N'admin.configuration.settings.catalog.showskuoncatalogpages.hint',
                    N'admin.configuration.settings.catalog.showskuonproductdetailspage',
                    N'admin.configuration.settings.catalog.showskuonproductdetailspage.hint',
                    N'admin.orders.products.addnew.sku',
                    N'admin.orders.products.sku',
                    N'admin.orders.shipments.products.sku',
                    N'messages.order.product(s).sku',
                    N'order.product(s).sku',
                    N'order.shipments.product(s).sku',
                    N'pdf.product.sku',
                    N'products.sku',
                    N'shoppingcart.sku'
                );

                UPDATE [LocaleStringResource]
                SET [ResourceValue] = N'Sản phẩm nhập từ Excel được phân biệt theo mã sản phẩm. Nếu mã đã tồn tại, sản phẩm tương ứng sẽ được cập nhật.'
                WHERE LOWER([ResourceName]) = N'admin.catalog.products.list.importfromexceltip';
            END
        ");
    }
}
