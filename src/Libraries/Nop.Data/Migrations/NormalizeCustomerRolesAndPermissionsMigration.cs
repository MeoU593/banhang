using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/26 10:30:00", "Normalize customer roles and permissions", MigrationProcessType.Update)]
public class NormalizeCustomerRolesAndPermissionsMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('CustomerRole', 'U') IS NOT NULL
            BEGIN
                UPDATE [CustomerRole]
                SET [Name] = N'Quản trị hệ thống'
                WHERE [SystemName] = N'Administrators'
                    AND [Name] IN (N'Administrators', N'Quản trị hệ thống');

                UPDATE [CustomerRole]
                SET [Name] = N'Người dùng thường'
                WHERE [SystemName] = N'Registered'
                    AND [Name] IN (N'Registered', N'Nhân viên', N'Người dùng thường');

                UPDATE [CustomerRole]
                SET [Name] = N'Người dùng đơn vị'
                WHERE [SystemName] = N'Guests'
                    AND [Name] IN (N'Guests', N'Khách truy cập');

                UPDATE [CustomerRole]
                SET [Name] = N'Quản trị đơn vị'
                WHERE [SystemName] = N'Vendors'
                    AND [Name] IN (N'Vendors', N'Quản trị đơn vị');
            END

            IF OBJECT_ID('PermissionRecord', 'U') IS NOT NULL
                AND OBJECT_ID('PermissionRecord_Role_Mapping', 'U') IS NOT NULL
            BEGIN
                DELETE FROM [PermissionRecord_Role_Mapping]
                WHERE [PermissionRecord_Id] IN
                (
                    SELECT [Id]
                    FROM [PermissionRecord]
                    WHERE [SystemName] IN
                    (
                        N'PublicStore.EnableShoppingCart',
                        N'Orders.CurrentCartsManage',
                        N'Orders.GiftCardsView',
                        N'Orders.GiftCardsCreateEditDelete',
                        N'Catalog.CheckoutAttributesView',
                        N'Catalog.CheckoutAttributesCreateEditDelete'
                    )
                );

                DELETE FROM [PermissionRecord]
                WHERE [SystemName] IN
                (
                    N'PublicStore.EnableShoppingCart',
                    N'Orders.CurrentCartsManage',
                    N'Orders.GiftCardsView',
                    N'Orders.GiftCardsCreateEditDelete',
                    N'Catalog.CheckoutAttributesView',
                    N'Catalog.CheckoutAttributesCreateEditDelete'
                );
            END

            IF OBJECT_ID('LocaleStringResource', 'U') IS NOT NULL
            BEGIN
                UPDATE [LocaleStringResource]
                SET [ResourceValue] = N'_START_-_END_ trong số _TOTAL_ mục'
                WHERE LOWER([ResourceName]) = N'admin.dt.info';

                UPDATE [LocaleStringResource]
                SET [ResourceValue] = N'Không có dữ liệu'
                WHERE LOWER([ResourceName]) = N'admin.dt.infoempty';

                UPDATE [LocaleStringResource]
                SET [ResourceValue] = N'(lọc từ _MAX_ bản ghi)'
                WHERE LOWER([ResourceName]) = N'admin.dt.infofiltered';

                UPDATE [LocaleStringResource]
                SET [ResourceValue] = N'Hiển thị _MENU_ mục'
                WHERE LOWER([ResourceName]) = N'admin.dt.lengthmenu';
            END
        ");
    }
}
