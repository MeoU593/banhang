using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Models.VendorStaff;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
public class VendorStaffAccessController : Nop.Web.Areas.Admin.Controllers.BaseAdminController
{
    private readonly ICustomerService _customerService;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IWorkContext _workContext;

    public VendorStaffAccessController(
        ICustomerService customerService,
        IRepository<Vendor> vendorRepository,
        IWorkContext workContext)
    {
        _customerService = customerService;
        _vendorRepository = vendorRepository;
        _workContext = workContext;
    }

    public async Task<IActionResult> Index()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        var model = await BuildModelAsync(currentVendor.Id, currentVendor.PmCustomerId);
        return View("~/Plugins/Misc.LogisticsReports/Views/VendorStaffAccess/Index.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> GrantAccess(int customerId)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        var target = await _customerService.GetCustomerByIdAsync(customerId);
        if (target == null || target.VendorId != currentVendor.Id || target.Id == currentCustomer.Id)
            return RedirectToAction(nameof(Index));

        if (await IsVendorLeaderAsync(target.Id, currentVendor.Id))
            return RedirectToAction(nameof(Index));

        var vendorsRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole != null && !await _customerService.IsInCustomerRoleAsync(target, NopCustomerDefaults.VendorsRoleName))
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = target.Id, CustomerRoleId = vendorsRole.Id });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RevokeAccess(int customerId)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        var target = await _customerService.GetCustomerByIdAsync(customerId);
        if (target == null || target.VendorId != currentVendor.Id || target.Id == currentCustomer.Id)
            return RedirectToAction(nameof(Index));

        if (await IsVendorLeaderAsync(target.Id))
            return RedirectToAction(nameof(Index));

        var vendorsRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole != null)
            await _customerService.RemoveCustomerRoleMappingAsync(target, vendorsRole);

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<VendorStaffAccessModel>> BuildModelAsync(int vendorId, int? pmCustomerId)
    {
        var staff = await _customerService.GetAllCustomersAsync(vendorId: vendorId);
        var model = new List<VendorStaffAccessModel>();

        foreach (var customer in staff)
        {
            model.Add(new VendorStaffAccessModel
            {
                CustomerId = customer.Id,
                Email = customer.Email,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                HasAccess = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.VendorsRoleName),
                IsLeader = customer.Id == pmCustomerId
            });
        }

        return model;
    }

    private async Task<bool> IsVendorLeaderAsync(int customerId, int? exceptVendorId = null)
    {
        return await _vendorRepository.Table.AnyAsync(v => v.PmCustomerId == customerId && (!exceptVendorId.HasValue || v.Id != exceptVendorId.Value));
    }
}
