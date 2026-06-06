using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/27 09:00:00", "Normalize admin permission labels", MigrationProcessType.Update)]
public class NormalizeAdminPermissionLabelsMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('CustomerRole', 'U') IS NOT NULL
            BEGIN
                UPDATE [CustomerRole]
                SET [Name] = N'Người dùng thường'
                WHERE [SystemName] = N'Registered'
                    AND [Name] IN (N'Registered', N'Nhân viên', N'Người dùng thường');

                UPDATE [CustomerRole]
                SET [Name] = N'Người dùng đơn vị'
                WHERE [SystemName] = N'Guests'
                    AND [Name] IN (N'Guests', N'Khách truy cập', N'Người dùng đơn vị');

                UPDATE [CustomerRole]
                SET [Name] = N'Quản trị hệ thống'
                WHERE [SystemName] = N'Administrators'
                    AND [Name] IN (N'Administrators', N'Quản trị hệ thống');

                UPDATE [CustomerRole]
                SET [Name] = N'Quản trị đơn vị'
                WHERE [SystemName] = N'Vendors'
                    AND [Name] IN (N'Vendors', N'Quản trị đơn vị');
            END

            IF OBJECT_ID('PermissionRecord', 'U') IS NOT NULL
            BEGIN
                UPDATE [PermissionRecord] SET [Name] = N'Bảo mật. Bật xác thực đa yếu tố' WHERE [SystemName] = N'Security.EnableMultiFactorAuthentication';
                UPDATE [PermissionRecord] SET [Name] = N'Truy cập trang quản trị' WHERE [SystemName] = N'Security.AccessAdminPanel';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Tài khoản. Xem' WHERE [SystemName] = N'Customers.View';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Tài khoản. Thêm, sửa, xóa' WHERE [SystemName] = N'Customers.CreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Tài khoản. Nhập, xuất dữ liệu' WHERE [SystemName] = N'Customers.ImportExport';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Tài khoản. Đăng nhập thay' WHERE [SystemName] = N'Customers.Impersonation';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Phân quyền tài khoản. Xem' WHERE [SystemName] = N'Customers.CustomerRolesView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Phân quyền tài khoản. Thêm, sửa, xóa' WHERE [SystemName] = N'Customers.CustomerRolesCreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Đơn vị. Xem' WHERE [SystemName] = N'Customers.VendorsView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Đơn vị. Thêm, sửa, xóa' WHERE [SystemName] = N'Customers.VendorsCreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Nhật ký hoạt động. Xem' WHERE [SystemName] = N'Customers.ActivityLogView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Nhật ký hoạt động. Xóa' WHERE [SystemName] = N'Customers.ActivityLogDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Nhật ký hoạt động. Quản lý loại' WHERE [SystemName] = N'Customers.ActivityLogManageTypes';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Dữ liệu cá nhân. Quản lý' WHERE [SystemName] = N'Customers.GDPRManage';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Hàng hóa. Xem' WHERE [SystemName] = N'Catalog.ProductsView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Hàng hóa. Thêm, sửa, xóa' WHERE [SystemName] = N'Catalog.ProductsCreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Hàng hóa. Nhập, xuất dữ liệu' WHERE [SystemName] = N'Catalog.ProductsImportExport';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Nhóm hàng hóa. Xem' WHERE [SystemName] = N'Catalog.CategoriesView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Nhóm hàng hóa. Thêm, sửa, xóa' WHERE [SystemName] = N'Catalog.CategoriesCreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Nhóm hàng hóa. Nhập, xuất dữ liệu' WHERE [SystemName] = N'Catalog.CategoriesImportExport';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Đánh giá hàng hóa. Xem' WHERE [SystemName] = N'Catalog.ProductReviewsView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Đánh giá hàng hóa. Thêm, sửa, xóa' WHERE [SystemName] = N'Catalog.ProductReviewsCreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Chủ đề nội dung. Xem' WHERE [SystemName] = N'ContentManagement.TopicsView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Chủ đề nội dung. Thêm, sửa, xóa' WHERE [SystemName] = N'ContentManagement.TopicsCreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Tin tức. Xem' WHERE [SystemName] = N'ContentManagement.BlogView';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Tin tức. Thêm, sửa, xóa' WHERE [SystemName] = N'ContentManagement.BlogCreateEditDelete';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Phân quyền truy cập. Quản lý' WHERE [SystemName] = N'Configuration.ManageACL';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Nhật ký hệ thống. Quản lý' WHERE [SystemName] = N'System.ManageSystemLog';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Bảo trì. Quản lý' WHERE [SystemName] = N'System.ManageMaintenance';
                UPDATE [PermissionRecord] SET [Name] = N'Quản trị. Tác vụ định kỳ. Quản lý' WHERE [SystemName] = N'System.ManageScheduleTasks';
                UPDATE [PermissionRecord] SET [Name] = N'Trang người dùng. Hiển thị giá' WHERE [SystemName] = N'PublicStore.DisplayPrices';
                UPDATE [PermissionRecord] SET [Name] = N'Trang người dùng. Bật sản phẩm quan tâm' WHERE [SystemName] = N'PublicStore.EnableWishlist';
                UPDATE [PermissionRecord] SET [Name] = N'Trang người dùng. Cho phép truy cập' WHERE [SystemName] = N'PublicStore.PublicStoreAllowNavigation';
                UPDATE [PermissionRecord] SET [Name] = N'Trang người dùng. Truy cập khi hệ thống đóng' WHERE [SystemName] = N'PublicStore.AccessClosedStore';
            END

            IF OBJECT_ID('PermissionRecord_Role_Mapping', 'U') IS NOT NULL
                AND OBJECT_ID('PermissionRecord', 'U') IS NOT NULL
                AND OBJECT_ID('CustomerRole', 'U') IS NOT NULL
            BEGIN
                DELETE prm
                FROM [PermissionRecord_Role_Mapping] prm
                INNER JOIN [PermissionRecord] pr ON pr.[Id] = prm.[PermissionRecord_Id]
                INNER JOIN [CustomerRole] cr ON cr.[Id] = prm.[CustomerRole_Id]
                WHERE pr.[SystemName] = N'Security.AccessAdminPanel'
                    AND cr.[SystemName] NOT IN (N'Administrators', N'Vendors');
            END
        ");
    }
}
