using Nop.Core.Domain.Customers;
using static Nop.Services.Security.StandardPermission;

namespace Nop.Services.Security;

/// <summary>
/// Default permission config manager
/// </summary>
public partial class DefaultPermissionConfigManager : IPermissionConfigManager
{
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        #region Security
        
        new ("Bảo mật. Bật xác thực đa yếu tố", StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION, nameof(StandardPermission.Security), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName),
        new ("Truy cập trang quản trị", StandardPermission.Security.ACCESS_ADMIN_PANEL, nameof(StandardPermission.Security), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),

        #endregion

        #region Customers
        
        new ("Quản trị. Tài khoản. Xem", StandardPermission.Customers.CUSTOMERS_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tài khoản. Thêm, sửa, xóa", StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tài khoản. Nhập, xuất dữ liệu", StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tài khoản. Đăng nhập thay", StandardPermission.Customers.CUSTOMERS_IMPERSONATION, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Phân quyền tài khoản. Xem", StandardPermission.Customers.CUSTOMER_ROLES_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Phân quyền tài khoản. Thêm, sửa, xóa", StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Đơn vị. Xem", StandardPermission.Customers.VENDORS_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Đơn vị. Thêm, sửa, xóa", StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhật ký hoạt động. Xem", StandardPermission.Customers.ACTIVITY_LOG_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhật ký hoạt động. Xóa", StandardPermission.Customers.ACTIVITY_LOG_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhật ký hoạt động. Quản lý loại", StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Dữ liệu cá nhân. Quản lý", StandardPermission.Customers.GDPR_MANAGE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Orders
        
        new ("Quản trị. Yêu cầu cấp phát. Xem", StandardPermission.Orders.ORDERS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Yêu cầu cấp phát. Thêm, sửa, xóa", StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Yêu cầu cấp phát. Nhập, xuất dữ liệu", StandardPermission.Orders.ORDERS_IMPORT_EXPORT, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Vận chuyển. Xem", StandardPermission.Orders.SHIPMENTS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Vận chuyển. Thêm, sửa, xóa", StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Yêu cầu trả hàng. Xem", StandardPermission.Orders.RETURN_REQUESTS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Yêu cầu trả hàng. Thêm, sửa, xóa", StandardPermission.Orders.RETURN_REQUESTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Thanh toán định kỳ. Xem", StandardPermission.Orders.RECURRING_PAYMENTS_VIEW, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Thanh toán định kỳ. Thêm, sửa, xóa", StandardPermission.Orders.RECURRING_PAYMENTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
        #endregion

        #region Reports
        
        new ("Quản trị. Báo cáo. Tổng hợp cấp phát", Reports.SALES_SUMMARY, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Báo cáo. Theo quốc gia", Reports.COUNTRY_SALES, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Báo cáo. Tồn kho thấp", Reports.LOW_STOCK, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Báo cáo. Hàng hóa nổi bật", Reports.BESTSELLERS, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Báo cáo. Hàng hóa chưa phát sinh", Reports.PRODUCTS_NEVER_PURCHASED, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Báo cáo. Tài khoản đăng ký", Reports.REGISTERED_CUSTOMERS, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Báo cáo. Tài khoản theo tổng cấp phát", Reports.CUSTOMERS_BY_ORDER_TOTAL, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Báo cáo. Tài khoản theo số yêu cầu", Reports.CUSTOMERS_BY_NUMBER_OF_ORDERS, nameof(Reports), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Catalog
        
        new ("Quản trị. Hàng hóa. Xem", StandardPermission.Catalog.PRODUCTS_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Hàng hóa. Thêm, sửa, xóa", StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Hàng hóa. Nhập, xuất dữ liệu", StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Nhóm hàng hóa. Xem", StandardPermission.Catalog.CATEGORIES_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhóm hàng hóa. Thêm, sửa, xóa", StandardPermission.Catalog.CATEGORIES_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhóm hàng hóa. Nhập, xuất dữ liệu", StandardPermission.Catalog.CATEGORIES_IMPORT_EXPORT, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhà sản xuất. Xem", StandardPermission.Catalog.MANUFACTURER_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhà sản xuất. Thêm, sửa, xóa", StandardPermission.Catalog.MANUFACTURER_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Nhà sản xuất. Nhập, xuất dữ liệu", StandardPermission.Catalog.MANUFACTURER_IMPORT_EXPORT, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Đánh giá hàng hóa. Xem", StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Đánh giá hàng hóa. Thêm, sửa, xóa", StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Thẻ hàng hóa. Xem", StandardPermission.Catalog.PRODUCT_TAGS_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Thẻ hàng hóa. Thêm, sửa, xóa", StandardPermission.Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Cấp lọc hàng hóa. Xem", StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Cấp lọc hàng hóa. Thêm, sửa, xóa", StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Cấp lọc hàng hóa. Nhập, xuất dữ liệu", StandardPermission.Catalog.FILTER_LEVEL_VALUE_IMPORT_EXPORT, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Thuộc tính hàng hóa. Xem", StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Thuộc tính hàng hóa. Thêm, sửa, xóa", StandardPermission.Catalog.PRODUCT_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Thông số kỹ thuật. Xem", StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_VIEW, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Thông số kỹ thuật. Thêm, sửa, xóa", StandardPermission.Catalog.SPECIFICATION_ATTRIBUTES_CREATE_EDIT_DELETE, nameof(StandardPermission.Catalog), NopCustomerDefaults.AdministratorsRoleName),
        #endregion

        #region Promotions
        
        new ("Quản trị. Khuyến mại. Xem", StandardPermission.Promotions.DISCOUNTS_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Khuyến mại. Thêm, sửa, xóa", StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.VendorsRoleName),
        new ("Quản trị. Liên kết. Xem", StandardPermission.Promotions.AFFILIATES_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Liên kết. Thêm, sửa, xóa", StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Người đăng ký nhận tin. Xem", StandardPermission.Promotions.SUBSCRIBERS_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Người đăng ký nhận tin. Thêm, sửa, xóa", StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Người đăng ký nhận tin. Nhập, xuất dữ liệu", StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Loại đăng ký nhận tin. Xem", StandardPermission.Promotions.SUBSCRIPTION_TYPE_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Loại đăng ký nhận tin. Thêm, sửa, xóa", StandardPermission.Promotions.SUBSCRIPTION_TYPE_CREATE_EDIT_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Chiến dịch. Xem", StandardPermission.Promotions.CAMPAIGNS_VIEW, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Chiến dịch. Thêm, sửa", StandardPermission.Promotions.CAMPAIGNS_CREATE_EDIT, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Chiến dịch. Xóa", StandardPermission.Promotions.CAMPAIGNS_DELETE, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Chiến dịch. Gửi email", StandardPermission.Promotions.CAMPAIGNS_SEND_EMAILS, nameof(StandardPermission.Promotions), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Content management
        
        new ("Quản trị. Chủ đề nội dung. Xem", StandardPermission.ContentManagement.TOPICS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Chủ đề nội dung. Thêm, sửa, xóa", StandardPermission.ContentManagement.TOPICS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Mẫu thông báo. Xem", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Mẫu thông báo. Thêm, sửa, xóa", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tin tức. Xem", StandardPermission.ContentManagement.BLOG_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tin tức. Thêm, sửa, xóa", StandardPermission.ContentManagement.BLOG_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Bình luận tin tức. Xem", StandardPermission.ContentManagement.BLOG_COMMENTS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Bình luận tin tức. Thêm, sửa, xóa", StandardPermission.ContentManagement.BLOG_COMMENTS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Diễn đàn. Xem", StandardPermission.ContentManagement.FORUMS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Diễn đàn. Thêm, sửa, xóa", StandardPermission.ContentManagement.FORUMS_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Menu. Xem", StandardPermission.ContentManagement.MENU_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Menu. Thêm, sửa, xóa", StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Configuration
        
        new ("Quản trị. Thành phần giao diện. Quản lý", StandardPermission.Configuration.MANAGE_WIDGETS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Quốc gia. Quản lý", StandardPermission.Configuration.MANAGE_COUNTRIES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Ngôn ngữ. Quản lý", StandardPermission.Configuration.MANAGE_LANGUAGES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Cấu hình. Quản lý", StandardPermission.Configuration.MANAGE_SETTINGS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Phương thức thanh toán. Quản lý", StandardPermission.Configuration.MANAGE_PAYMENT_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Xác thực ngoài. Quản lý", StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Phương thức xác thực đa yếu tố. Quản lý", StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Thuế. Quản lý", StandardPermission.Configuration.MANAGE_TAX_SETTINGS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Vận chuyển. Quản lý", StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tiền tệ. Quản lý", StandardPermission.Configuration.MANAGE_CURRENCIES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Phân quyền truy cập. Quản lý", StandardPermission.Configuration.MANAGE_ACL, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tài khoản email. Quản lý", StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Cửa hàng. Quản lý", StandardPermission.Configuration.MANAGE_STORES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Plugin. Quản lý", StandardPermission.Configuration.MANAGE_PLUGINS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        
        #endregion

        #region System
        
        new ("Quản trị. Nhật ký hệ thống. Quản lý", StandardPermission.System.MANAGE_SYSTEM_LOG, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Hàng đợi thông báo. Quản lý", StandardPermission.System.MANAGE_MESSAGE_QUEUE, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Bảo trì. Quản lý", StandardPermission.System.MANAGE_MAINTENANCE, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Trình soạn thảo. Quản lý ảnh", StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Tác vụ định kỳ. Quản lý", StandardPermission.System.MANAGE_SCHEDULE_TASKS, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Quản trị. Cấu hình ứng dụng. Quản lý", StandardPermission.System.MANAGE_APP_SETTINGS, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),

        #endregion
        
        #region Public store
        
        new ("Trang người dùng. Hiển thị giá", StandardPermission.PublicStore.DISPLAY_PRICES, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName, NopCustomerDefaults.GuestsRoleName, NopCustomerDefaults.ForumModeratorsRoleName),
        new ("Trang người dùng. Bật sản phẩm quan tâm", StandardPermission.PublicStore.ENABLE_WISHLIST, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName, NopCustomerDefaults.GuestsRoleName, NopCustomerDefaults.ForumModeratorsRoleName),
        new ("Trang người dùng. Cho phép truy cập", StandardPermission.PublicStore.PUBLIC_STORE_ALLOW_NAVIGATION, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName, NopCustomerDefaults.GuestsRoleName, NopCustomerDefaults.ForumModeratorsRoleName),
        new ("Trang người dùng. Truy cập khi hệ thống đóng", StandardPermission.PublicStore.ACCESS_CLOSED_STORE, nameof(StandardPermission.PublicStore), NopCustomerDefaults.AdministratorsRoleName),

        #endregion
    };
}
