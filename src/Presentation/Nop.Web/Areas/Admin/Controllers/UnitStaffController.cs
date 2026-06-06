using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
public partial class UnitStaffController : BaseAdminController
{
    private static readonly int[] _allowedPageSizes = [10, 20, 50, 100];

    protected readonly ICustomerMilitaryProfileService _customerMilitaryProfileService;
    protected readonly ICustomerService _customerService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Vendor> _vendorRepository;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;

    public UnitStaffController(
        ICustomerMilitaryProfileService customerMilitaryProfileService,
        ICustomerService customerService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IRepository<Customer> customerRepository,
        IRepository<Vendor> vendorRepository,
        IVendorService vendorService,
        IWorkContext workContext)
    {
        _customerMilitaryProfileService = customerMilitaryProfileService;
        _customerService = customerService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _customerRepository = customerRepository;
        _vendorRepository = vendorRepository;
        _vendorService = vendorService;
        _workContext = workContext;
    }

    public virtual async Task<IActionResult> List(int vendorId = 0, int page = 1, int pageSize = 20)
    {
        var access = await GetStaffAccessAsync(vendorId);
        if (!access.CanManage)
            return AccessDeniedView();

        pageSize = NormalizePageSize(pageSize);
        page = Math.Max(page, 1);

        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
        var vendorNames = vendors.ToDictionary(vendor => vendor.Id, vendor => vendor.Name);
        var leaderIds = vendors.Where(vendor => vendor.PmCustomerId.HasValue).Select(vendor => vendor.PmCustomerId.Value).ToHashSet();

        var query = _customerRepository.Table.Where(customer => !customer.Deleted && (customer.VendorId > 0 || customer.CompanyId > 0));
        if (access.SelectedVendorId > 0)
            query = query.Where(customer => customer.VendorId == access.SelectedVendorId || customer.CompanyId == access.SelectedVendorId);

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        page = Math.Min(page, totalPages);
        var skip = (page - 1) * pageSize;

        var customers = await query
            .OrderBy(customer => customer.VendorId > 0 ? customer.VendorId : customer.CompanyId.GetValueOrDefault())
            .ThenBy(customer => customer.LastName)
            .ThenBy(customer => customer.FirstName)
            .ThenBy(customer => customer.Email)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        var model = new UnitStaffModel
        {
            SelectedVendorId = access.SelectedVendorId,
            VendorName = access.SelectedVendorId > 0 && vendorNames.TryGetValue(access.SelectedVendorId, out var selectedVendorName) ? selectedVendorName : "Toàn hệ thống",
            IsSystemAdmin = access.IsSystemAdmin,
            CanSelectVendor = access.IsSystemAdmin,
            AvailableVendors = PrepareVendorSelectList(vendors, access.SelectedVendorId, includeAllOption: true),
            Staff = await PrepareStaffItemsAsync(customers, vendorNames, leaderIds, access),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            FirstItemIndex = totalItems == 0 ? 0 : skip + 1,
            LastItemIndex = totalItems == 0 ? 0 : skip + customers.Count,
            AvailablePageSizes = PreparePageSizeSelectList(pageSize)
        };

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Edit(UnitStaffEditModel model)
    {
        var target = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (target == null || !await CanEditCustomerInfoAsync(target))
            return AccessDeniedView();

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            _notificationService.ErrorNotification("Vui lòng nhập Gmail.");
            return RedirectToAction(nameof(List), new { vendorId = model.VendorId, page = model.Page, pageSize = model.PageSize });
        }

        var customerByEmail = await _customerService.GetCustomerByEmailAsync(model.Email.Trim());
        if (customerByEmail != null && customerByEmail.Id != target.Id)
        {
            _notificationService.ErrorNotification("Gmail đã được sử dụng bởi tài khoản khác.");
            return RedirectToAction(nameof(List), new { vendorId = model.VendorId, page = model.Page, pageSize = model.PageSize });
        }

        if (!string.IsNullOrWhiteSpace(model.Username))
        {
            var customerByUsername = await _customerService.GetCustomerByUsernameAsync(model.Username.Trim());
            if (customerByUsername != null && customerByUsername.Id != target.Id)
            {
                _notificationService.ErrorNotification("Tên đăng nhập đã được sử dụng bởi tài khoản khác.");
                return RedirectToAction(nameof(List), new { vendorId = model.VendorId, page = model.Page, pageSize = model.PageSize });
            }
        }

        target.FirstName = model.FirstName?.Trim();
        target.LastName = model.LastName?.Trim();
        target.Email = model.Email.Trim();
        target.Username = model.Username?.Trim();
        target.Phone = model.Phone?.Trim();
        target.Active = model.Active;
        await _customerService.UpdateCustomerAsync(target);

        var now = DateTime.UtcNow;
        var profile = await _customerMilitaryProfileService.GetByCustomerIdAsync(target.Id);
        if (profile == null)
        {
            profile = new CustomerMilitaryProfile
            {
                CustomerId = target.Id,
                CreatedOnUtc = now
            };
        }

        profile.MilitaryCode = model.MilitaryCode?.Trim();
        profile.Rank = model.Rank?.Trim();
        profile.PositionTitle = model.PositionTitle?.Trim();
        profile.UnitName = model.UnitName?.Trim();
        profile.UpdatedOnUtc = now;

        if (profile.Id > 0)
            await _customerMilitaryProfileService.UpdateAsync(profile);
        else
            await _customerMilitaryProfileService.InsertAsync(profile);

        _notificationService.SuccessNotification("Đã cập nhật thông tin nhân sự.");
        return RedirectToAction(nameof(List), new { vendorId = model.VendorId, page = model.Page, pageSize = model.PageSize });
    }

    [HttpPost]
    public virtual async Task<IActionResult> GrantAccess(int customerId, int vendorId = 0, int page = 1, int pageSize = 20)
    {
        var target = await _customerService.GetCustomerByIdAsync(customerId);
        if (target == null || !await CanManageCustomerAccessAsync(target))
            return AccessDeniedView();

        var vendorsRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole != null && !await _customerService.IsInCustomerRoleAsync(target, NopCustomerDefaults.VendorsRoleName))
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = target.Id, CustomerRoleId = vendorsRole.Id });

        _notificationService.SuccessNotification("Đã cấp quyền quản trị đơn vị.");
        return RedirectToAction(nameof(List), new { vendorId, page, pageSize });
    }

    [HttpPost]
    public virtual async Task<IActionResult> RevokeAccess(int customerId, int vendorId = 0, int page = 1, int pageSize = 20)
    {
        var target = await _customerService.GetCustomerByIdAsync(customerId);
        if (target == null || !await CanManageCustomerAccessAsync(target))
            return AccessDeniedView();

        var vendorsRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole != null)
            await _customerService.RemoveCustomerRoleMappingAsync(target, vendorsRole);

        _notificationService.SuccessNotification("Đã thu hồi quyền quản trị đơn vị.");
        return RedirectToAction(nameof(List), new { vendorId, page, pageSize });
    }

    protected virtual async Task<IList<UnitStaffItemModel>> PrepareStaffItemsAsync(IList<Customer> customers, IDictionary<int, string> vendorNames, ISet<int> leaderIds, StaffAccess access)
    {
        var result = new List<UnitStaffItemModel>();
        foreach (var customer in customers)
        {
            var profile = await _customerMilitaryProfileService.GetByCustomerIdAsync(customer.Id);
            var roles = await _customerService.GetCustomerRolesAsync(customer, showHidden: true);
            var isLeader = leaderIds.Contains(customer.Id);
            var effectiveVendorId = GetEffectiveVendorId(customer);
            result.Add(new UnitStaffItemModel
            {
                CustomerId = customer.Id,
                VendorId = effectiveVendorId,
                VendorName = vendorNames.TryGetValue(effectiveVendorId, out var vendorName) ? vendorName : string.Empty,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                FirstName = customer.FirstName ?? string.Empty,
                LastName = customer.LastName ?? string.Empty,
                Email = customer.Email ?? string.Empty,
                Username = customer.Username ?? string.Empty,
                Phone = customer.Phone ?? string.Empty,
                Rank = profile?.Rank ?? string.Empty,
                PositionTitle = profile?.PositionTitle ?? string.Empty,
                MilitaryCode = profile?.MilitaryCode ?? string.Empty,
                UnitName = profile?.UnitName ?? string.Empty,
                RoleNames = string.Join(", ", roles.Where(role => role.Active).Select(GetRoleDisplayName)),
                CreatedOnText = customer.CreatedOnUtc.ToString("dd/MM/yyyy"),
                LastLoginText = customer.LastLoginDateUtc?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa đăng nhập",
                Active = customer.Active,
                HasUnitAdminAccess = roles.Any(role => role.SystemName == NopCustomerDefaults.VendorsRoleName),
                IsLeader = isLeader,
                CanManageAccess = !isLeader && customer.Id != access.CurrentCustomerId,
                CanEditInfo = access.IsSystemAdmin || (!isLeader && customer.Id != access.CurrentCustomerId)
            });
        }

        return result;
    }

    protected virtual async Task<StaffAccess> GetStaffAccessAsync(int selectedVendorId)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var isSystemAdmin = currentVendor == null && await _permissionService.AuthorizeAsync(StandardPermission.Customers.CUSTOMERS_VIEW, currentCustomer);
        if (isSystemAdmin)
            return new StaffAccess(true, true, selectedVendorId, currentCustomer.Id);

        if (currentVendor != null && currentVendor.PmCustomerId == currentCustomer.Id)
            return new StaffAccess(true, false, currentVendor.Id, currentCustomer.Id);

        return new StaffAccess(false, false, 0, currentCustomer.Id);
    }

    protected virtual async Task<bool> CanManageCustomerAccessAsync(Customer target)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        if (target.Id == currentCustomer.Id || await IsVendorLeaderAsync(target.Id))
            return false;

        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            return currentVendor.PmCustomerId == currentCustomer.Id && GetEffectiveVendorId(target) == currentVendor.Id;

        return await _permissionService.AuthorizeAsync(StandardPermission.Customers.CUSTOMERS_VIEW, currentCustomer);
    }

    protected virtual async Task<bool> CanEditCustomerInfoAsync(Customer target)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            return currentVendor.PmCustomerId == currentCustomer.Id && GetEffectiveVendorId(target) == currentVendor.Id && target.Id != currentCustomer.Id;

        return await _permissionService.AuthorizeAsync(StandardPermission.Customers.CUSTOMERS_VIEW, currentCustomer);
    }

    protected virtual int GetEffectiveVendorId(Customer customer)
    {
        return customer.VendorId > 0 ? customer.VendorId : customer.CompanyId.GetValueOrDefault();
    }

    protected virtual IList<SelectListItem> PreparePageSizeSelectList(int selectedPageSize)
    {
        return _allowedPageSizes
            .Select(size => new SelectListItem
            {
                Text = size.ToString(),
                Value = size.ToString(),
                Selected = size == selectedPageSize
            })
            .ToList();
    }

    protected virtual int NormalizePageSize(int pageSize)
    {
        return _allowedPageSizes.Contains(pageSize) ? pageSize : 20;
    }

    protected virtual async Task<bool> IsVendorLeaderAsync(int customerId)
    {
        return await _vendorRepository.Table.AnyAsync(vendor => vendor.PmCustomerId == customerId && !vendor.Deleted);
    }

    protected virtual IList<SelectListItem> PrepareVendorSelectList(IList<Vendor> vendors, int selectedVendorId, bool includeAllOption)
    {
        var items = new List<SelectListItem>();
        if (includeAllOption)
            items.Add(new SelectListItem { Text = "Tất cả đơn vị", Value = "0", Selected = selectedVendorId <= 0 });

        items.AddRange(vendors
            .Where(vendor => !vendor.Deleted)
            .OrderBy(vendor => vendor.Path ?? vendor.Name)
            .ThenBy(vendor => vendor.DisplayOrder)
            .ThenBy(vendor => vendor.Name)
            .Select(vendor => new SelectListItem
            {
                Text = vendor.Name,
                Value = vendor.Id.ToString(),
                Selected = vendor.Id == selectedVendorId
            }));

        return items;
    }

    protected virtual string GetRoleDisplayName(CustomerRole role)
    {
        return role.Name;
    }

    protected readonly record struct StaffAccess(bool CanManage, bool IsSystemAdmin, int SelectedVendorId, int CurrentCustomerId);
}
