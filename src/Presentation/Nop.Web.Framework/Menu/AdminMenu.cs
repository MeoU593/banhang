using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Menu;

/// <summary>
/// Admin menu
/// </summary>
public partial class AdminMenu : IAdminMenu
{
    #region Fields

    protected AdminMenuItem _baseRootMenuItem;
    protected AdminMenuItem _rootItem;

    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
#pragma warning disable CS0618 // Type or member is obsolete
    protected readonly IPluginManager<IAdminMenuPlugin> _adminMenuPluginManager;
#pragma warning restore CS0618 // Type or member is obsolete
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public AdminMenu(FilterLevelSettings filterLevelSettings,
        IActionContextAccessor actionContextAccessor,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        IPermissionService permissionService,
#pragma warning disable CS0618 // Type or member is obsolete
        IPluginManager<IAdminMenuPlugin> adminMenuPluginManager,
#pragma warning restore CS0618 // Type or member is obsolete
        IUrlHelperFactory urlHelperFactory,
        IWorkContext workContext)
    {
        _filterLevelSettings = filterLevelSettings;
        _actionContextAccessor = actionContextAccessor;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _adminMenuPluginManager = adminMenuPluginManager;
        _urlHelperFactory = urlHelperFactory;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Fills the base root menu item data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task FillBaseRootAsync()
    {
        if (_baseRootMenuItem != null)
            return;

        _baseRootMenuItem = new AdminMenuItem
        {
            SystemName = "Home",
            Title = await _localizationService.GetResourceAsync("Admin.Home"),
            Url = GetMenuItemUrl("Home", "Overview"),
            ChildNodes = new List<AdminMenuItem>
            {
                //unit management
                new()
                {
                    SystemName = "UnitManagement",
                    Title = "Quản lý đơn vị",
                    IconClass = "fas fa-sitemap",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Vendors",
                            Title = "Danh sách đơn vị",
                            PermissionNames = new List<string> { StandardPermission.Customers.VENDORS_VIEW },
                            Url = GetMenuItemUrl("Vendor", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Vendor attributes",
                            Title = "Thuộc tính đơn vị",
                            PermissionNames = new List<string> { StandardPermission.Customers.VENDORS_VIEW },
                            Url = GetMenuItemUrl("VendorAttribute", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Customers list",
                            Title = "Tài khoản khách hàng",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("Customer", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Customer roles",
                            Title = "Vai trò khách hàng",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMER_ROLES_VIEW },
                            Url = GetMenuItemUrl("CustomerRole", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Pending customers",
                            Title = "Tài khoản chờ duyệt",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("Customer", "PendingApproval"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Online customers",
                            Title = "Khách hàng trực tuyến",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("OnlineCustomer", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Activity logs",
                            Title = "Nhật ký hoạt động",
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityLogs"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Activity types",
                            Title = "Loại hoạt động",
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityTypes"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //product management
                new()
                {
                    SystemName = "ProductManagement",
                    Title = "Quản lý sản phẩm",
                    IconClass = "fas fa-box-open",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Products",
                            Title = "Danh mục hàng hóa",
                            PermissionNames = new List<string> { StandardPermission.Catalog.PRODUCTS_VIEW },
                            Url = GetMenuItemUrl("Product", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Categories",
                            Title = "Nhóm hàng hóa",
                            PermissionNames = new List<string> { StandardPermission.Catalog.CATEGORIES_VIEW },
                            Url = GetMenuItemUrl("Category", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Product reviews",
                            Title = "Đánh giá sản phẩm",
                            PermissionNames = new List<string> { StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW },
                            Url = GetMenuItemUrl("ProductReview", "List"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //document/report roots are populated by plugins
                new()
                {
                    SystemName = "DocumentPortal.Root",
                    Title = "Quản lý tài liệu",
                    IconClass = "fas fa-folder-open",
                    ChildNodes = new List<AdminMenuItem>()
                },
                new()
                {
                    SystemName = "LogisticsReports.Root",
                    Title = "Quản lý báo cáo",
                    IconClass = "fas fa-chart-bar",
                    ChildNodes = new List<AdminMenuItem>()
                },
                //forum management
                new()
                {
                    SystemName = "ForumManagement",
                    Title = "Quản lý diễn đàn",
                    IconClass = "fas fa-comments",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Manage forums",
                            Title = "Diễn đàn",
                            PermissionNames = new List<string> { StandardPermission.ContentManagement.FORUMS_VIEW },
                            Url = GetMenuItemUrl("Forum", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Forums settings",
                            Title = "Cài đặt diễn đàn",
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_SETTINGS },
                            Url = GetMenuItemUrl("Setting", "Forum"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //system management
                new()
                {
                    SystemName = "SystemManagement",
                    Title = "Quản lý hệ thống",
                    IconClass = "fas fa-cogs",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Settings",
                            Title = "Cài đặt",
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_SETTINGS },
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "General settings",
                                    Title = "Cài đặt chung",
                                    Url = GetMenuItemUrl("Setting", "GeneralCommon"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Customer and user settings",
                                    Title = "Cài đặt người dùng",
                                    Url = GetMenuItemUrl("Setting", "CustomerUser"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Catalog settings",
                                    Title = "Cài đặt danh mục",
                                    Url = GetMenuItemUrl("Setting", "Catalog"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Blog settings",
                                    Title = "Cài đặt tin tức",
                                    Url = GetMenuItemUrl("Setting", "Blog"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Forums settings",
                                    Title = "Cài đặt diễn đàn",
                                    Url = GetMenuItemUrl("Setting", "Forum"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Media settings",
                                    Title = "Cài đặt media",
                                    Url = GetMenuItemUrl("Setting", "Media"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "App settings",
                                    Title = "Cấu hình ứng dụng",
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.System.MANAGE_APP_SETTINGS
                                        },
                                    Url = GetMenuItemUrl("Setting", "AppSettings"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "All settings",
                                    Title = "Tất cả cài đặt",
                                    Url = GetMenuItemUrl("Setting", "AllSettings"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Duty message",
                                    Title = "Thông điệp trực ban",
                                    PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_SETTINGS },
                                    Url = GetMenuItemUrl("Setting", "DutyMessage"),
                                    IconClass = "far fa-circle"
                                }
                            }
                        },
                        new()
                        {
                            SystemName = "Stores",
                            Title = "Cửa hàng",
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_STORES },
                            Url = GetMenuItemUrl("Store",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Access control list",
                            Title = "Phân quyền truy cập",
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_ACL },
                            Url = GetMenuItemUrl("Security", "Permissions"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "System information",
                            Title = "Thông tin hệ thống",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "SystemInfo"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Log",
                            Title = "Nhật ký hệ thống",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SYSTEM_LOG },
                            Url = GetMenuItemUrl("Log", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Warnings",
                            Title = "Cảnh báo",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "Warnings"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Maintenance",
                            Title = "Bảo trì",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "Maintenance"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Schedule tasks",
                            Title = "Tác vụ định kỳ",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SCHEDULE_TASKS },
                            Url = GetMenuItemUrl("ScheduleTask",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Search engine friendly names",
                            Title = "Đường dẫn thân thiện",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "SeNames"),
                            IconClass = "far fa-dot-circle"
                        },
                    }
                },
                new()
                {
                    SystemName = "Third party plugins",
                    Title = await _localizationService.GetResourceAsync("Admin.Plugins"),
                    IconClass = "fas fa-bars"
                }
            }
        };
    }

    /// <summary>
    /// Loads admin menu
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records (Visible == false)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the root menu item for admin menu
    /// </returns>
    protected virtual async Task<AdminMenuItem> LoadMenuAsync(bool showHidden)
    {
        await FillBaseRootAsync();

        AdminMenuItem cloneMenuItem(AdminMenuItem item)
        {
            return new AdminMenuItem
            {
                PermissionNames = item.PermissionNames,
                ChildNodes = item.ChildNodes.Select(cloneMenuItem).ToList(),
                IconClass = item.IconClass,
                Visible = item.Visible,
                OpenUrlInNewTab = item.OpenUrlInNewTab,
                SystemName = item.SystemName,
                Title = item.Title,
                Url = item.Url
            };
        }

        var root = cloneMenuItem(_baseRootMenuItem);

        var customer = await _workContext.GetCurrentCustomerAsync();

        await _eventPublisher.PublishAsync(new AdminMenuCreatedEvent(this, root));

        if (await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS, customer))
        {
            await _eventPublisher.PublishAsync(new ThirdPartyPluginsMenuItemCreatedEvent(this, root.GetItemBySystemName("Third party plugins")));

            var adminMenuPlugins = await _adminMenuPluginManager.LoadAllPluginsAsync(customer);

            foreach (var adminMenuPlugin in adminMenuPlugins)
                await adminMenuPlugin.ManageSiteMapAsync(root);
        }

        async ValueTask<bool> authorizePermission(string permissionName) => await _permissionService.AuthorizeAsync(permissionName.Trim());

        async Task checkPermissions(AdminMenuItem menuItem, AdminMenuItem rootItem = null)
        {
            if (menuItem.Visible)
            {
                var permissions = (menuItem.PermissionNames.Any() ? menuItem.PermissionNames : (rootItem?.PermissionNames ?? new List<string>())).Distinct().Where(p => !string.IsNullOrEmpty(p)).ToList();

                if (permissions.Any())
                    menuItem.Visible = menuItem.ChildNodes.Any() ? await permissions.AnyAwaitAsync(authorizePermission) : await permissions.AllAwaitAsync(authorizePermission);
            }

            foreach (var childNode in menuItem.ChildNodes)
                await checkPermissions(childNode, menuItem);
        }

        await checkPermissions(root);

        if (showHidden)
            return root;

        void checkVisible(AdminMenuItem menuItem)
        {
            if (!menuItem.ChildNodes.Any())
            {
                menuItem.Visible = menuItem.Visible && !string.IsNullOrEmpty(menuItem.Url);

                return;
            }

            foreach (var childNode in menuItem.ChildNodes)
                checkVisible(childNode);

            menuItem.Visible = menuItem.ChildNodes.Any(n => n.Visible);
        }

        checkVisible(root);

        return root;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the root node
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records (Visible == false)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the root menu item
    /// </returns>
    public virtual async Task<AdminMenuItem> GetRootNodeAsync(bool showHidden = false)
    {
        if (_rootItem != null)
            return _rootItem;

        _rootItem = await LoadMenuAsync(showHidden);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext ?? throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext)));

        void transformUrl(AdminMenuItem node)
        {
            if (node.Url?.StartsWith("~/", StringComparison.Ordinal) ?? false)
                node.Url = urlHelper.Content(node.Url);

            foreach (var childNode in node.ChildNodes)
                transformUrl(childNode);
        }

        transformUrl(_rootItem);

        return _rootItem;
    }

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">The name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    /// <returns>Menu item URL</returns>
    public virtual string GetMenuItemUrl(string controllerName, string actionName)
    {
        if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
            return null;

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext ?? throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext)));

        return urlHelper.Action(actionName, controllerName, new RouteValueDictionary { { "area", AreaNames.ADMIN } }, null, null);
    }

    #endregion
}
