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
using Nop.Services.Vendors;
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
    protected readonly IVendorService _vendorService;
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
        IVendorService vendorService,
        IWorkContext workContext)
    {
        _filterLevelSettings = filterLevelSettings;
        _actionContextAccessor = actionContextAccessor;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _adminMenuPluginManager = adminMenuPluginManager;
        _urlHelperFactory = urlHelperFactory;
        _vendorService = vendorService;
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
                            SystemName = "UnitDashboard",
                            Title = "Tổng quan",
                            PermissionNames = new List<string> { StandardPermission.Security.ACCESS_ADMIN_PANEL },
                            Url = GetMenuItemUrl("Home", "Index"),
                            IconClass = "fas fa-chart-line"
                        },
                        new()
                        {
                            SystemName = "Vendors",
                            Title = "Danh sách đơn vị",
                            PermissionNames = new List<string> { StandardPermission.Customers.VENDORS_VIEW },
                            IconClass = "fas fa-building"
                        },
                        new()
                        {
                            SystemName = "UnitDirectory",
                            Title = "Quản lý nhân sự",
                            PermissionNames = new List<string> { StandardPermission.Security.ACCESS_ADMIN_PANEL },
                            Url = GetMenuItemUrl("UnitDirectory", "List"),
                            IconClass = "fas fa-address-book"
                        },
                        new()
                        {
                            SystemName = "UnitStaff",
                            Title = "Quản lý tài khoản",
                            PermissionNames = new List<string> { StandardPermission.Security.ACCESS_ADMIN_PANEL },
                            Url = GetMenuItemUrl("UnitStaff", "List"),
                            IconClass = "fas fa-user-shield"
                        },
                        new()
                        {
                            SystemName = "UnitActivityLog",
                            Title = "Nhật ký hệ thống",
                            PermissionNames = new List<string> { StandardPermission.Security.ACCESS_ADMIN_PANEL },
                            Url = GetMenuItemUrl("UnitActivityLog", "List"),
                            IconClass = "fas fa-clock-rotate-left"
                        }
                    }
                },
                //account management
                new()
                {
                    SystemName = "AccountManagement",
                    Title = "Quản lý tài khoản",
                    IconClass = "fas fa-users-gear",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Customers list",
                            Title = "Danh sách tài khoản",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("Customer", "List"),
                            IconClass = "fas fa-user-group"
                        },
                        new()
                        {
                            SystemName = "Customer roles",
                            Title = "Phân quyền tài khoản",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMER_ROLES_VIEW },
                            Url = GetMenuItemUrl("CustomerRole", "List"),
                            IconClass = "fas fa-user-shield"
                        },
                        new()
                        {
                            SystemName = "Pending customers",
                            Title = "Tài khoản chờ duyệt",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("Customer", "PendingApproval"),
                            IconClass = "fas fa-user-clock"
                        },
                        new()
                        {
                            SystemName = "Online customers",
                            Title = "Tài khoản trực tuyến",
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("OnlineCustomer", "List"),
                            IconClass = "fas fa-signal"
                        }
                    }
                },
                //system logs
                new()
                {
                    SystemName = "SystemLogs",
                    Title = "Nhật ký hệ thống",
                    IconClass = "fas fa-clipboard-list",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Activity logs",
                            Title = "Nhật ký hoạt động",
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityLogs"),
                            IconClass = "fas fa-list-check"
                        },
                        new()
                        {
                            SystemName = "Activity types",
                            Title = "Loại hoạt động",
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityTypes"),
                            IconClass = "fas fa-tags"
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
                            IconClass = "fas fa-boxes-stacked"
                        },
                        new()
                        {
                            SystemName = "Categories",
                            Title = "Nhóm hàng hóa",
                            PermissionNames = new List<string> { StandardPermission.Catalog.CATEGORIES_VIEW },
                            Url = GetMenuItemUrl("Category", "List"),
                            IconClass = "fas fa-layer-group"
                        },
                        new()
                        {
                            SystemName = "Product reviews",
                            Title = "Đánh giá sản phẩm",
                            PermissionNames = new List<string> { StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW },
                            Url = GetMenuItemUrl("ProductReview", "List"),
                            IconClass = "fas fa-star-half-stroke"
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
                            SystemName = "Access control list",
                            Title = "Phân quyền truy cập",
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_ACL },
                            Url = GetMenuItemUrl("Security", "Permissions"),
                            IconClass = "fas fa-key"
                        },
                        new()
                        {
                            SystemName = "Log",
                            Title = "Nhật ký hệ thống",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SYSTEM_LOG },
                            Url = GetMenuItemUrl("Log", "List"),
                            IconClass = "fas fa-file-lines"
                        },
                        new()
                        {
                            SystemName = "Maintenance",
                            Title = "Bảo trì",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "Maintenance"),
                            IconClass = "fas fa-screwdriver-wrench"
                        },
                        new()
                        {
                            SystemName = "Schedule tasks",
                            Title = "Tác vụ định kỳ",
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SCHEDULE_TASKS },
                            Url = GetMenuItemUrl("ScheduleTask",
                            "List"),
                            IconClass = "fas fa-calendar-days"
                        },
                        new()
                        {
                            SystemName = "Topics",
                            Title = "Tài liệu hướng dẫn",
                            PermissionNames = new List<string> { StandardPermission.ContentManagement.TOPICS_VIEW },
                            Url = GetMenuItemUrl("Topic", "EditUserGuide"),
                            IconClass = "fas fa-book-open"
                        }
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

    protected virtual async Task PopulateVendorTreeMenuAsync(AdminMenuItem root)
    {
        var vendorsMenu = root.GetItemBySystemName("Vendors");
        if (vendorsMenu == null)
            return;

        var vendorListUrl = GetMenuItemUrl("Vendor", "List");
        var vendorDetailsUrl = GetMenuItemUrl("Vendor", "Details");
        if (string.IsNullOrWhiteSpace(vendorListUrl))
            return;

        vendorsMenu.ChildNodes.Clear();

        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var vendors = (await _vendorService.GetAllVendorsAsync(showHidden: true, pageSize: int.MaxValue)).ToList();

        if (currentVendor != null)
        {
            var allowedVendorIds = (await _vendorService.GetDescendantVendorIdsAsync(currentVendor.Id, includeSelf: true)).ToHashSet();
            vendors = vendors.Where(vendor => allowedVendorIds.Contains(vendor.Id)).ToList();
        }

        vendorsMenu.ChildNodes.Add(new AdminMenuItem
        {
            SystemName = "Vendors.All",
            Title = currentVendor == null ? "Tất cả đơn vị" : "Tất cả đơn vị thuộc quyền",
            PermissionNames = new List<string> { StandardPermission.Customers.VENDORS_VIEW },
            Url = currentVendor == null ? vendorListUrl : $"{vendorListUrl}?SearchRootVendorId={currentVendor.Id}",
            IconClass = "fas fa-list"
        });

        AdminMenuItem buildVendorNode(Nop.Core.Domain.Vendors.Vendor vendor)
        {
            var children = vendors
                .Where(child => child.ParentId == vendor.Id)
                .OrderBy(child => child.DisplayOrder)
                .ThenBy(child => child.Name)
                .Select(buildVendorNode)
                .ToList();

            return new AdminMenuItem
            {
                SystemName = $"Vendors.{vendor.Id}",
                Title = vendor.Name,
                PermissionNames = new List<string> { StandardPermission.Customers.VENDORS_VIEW },
                Url = string.IsNullOrWhiteSpace(vendorDetailsUrl) ? $"{vendorListUrl}?SearchRootVendorId={vendor.Id}" : $"{vendorDetailsUrl}?id={vendor.Id}",
                IconClass = children.Any() ? "fas fa-building" : "far fa-building",
                ChildNodes = children
            };
        }

        var topVendors = currentVendor != null
            ? vendors.Where(vendor => vendor.Id == currentVendor.Id)
            : vendors.Where(vendor => !vendor.ParentId.HasValue || vendor.ParentId.Value <= 0);

        foreach (var vendor in topVendors.OrderBy(vendor => vendor.DisplayOrder).ThenBy(vendor => vendor.Name))
            vendorsMenu.ChildNodes.Add(buildVendorNode(vendor));
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
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var canManageAllVendors = currentVendor == null && await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, customer);
        var isUnitLeader = currentVendor != null && currentVendor.PmCustomerId == customer.Id;

        await PopulateVendorTreeMenuAsync(root);

        var unitDirectoryMenu = root.GetItemBySystemName("UnitDirectory");
        if (unitDirectoryMenu != null)
            unitDirectoryMenu.Visible = canManageAllVendors || isUnitLeader;

        var unitActivityLogMenu = root.GetItemBySystemName("UnitActivityLog");
        if (unitActivityLogMenu != null)
            unitActivityLogMenu.Visible = isUnitLeader;

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
