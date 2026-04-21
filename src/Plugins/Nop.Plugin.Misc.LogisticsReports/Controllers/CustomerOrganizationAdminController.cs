using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Models.Organizations;
using Nop.Plugin.Misc.LogisticsReports.Models.Shared;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class CustomerOrganizationAdminController : BasePluginController
{
    private readonly ICustomerOrganizationUnitService _mappingService;
    private readonly IOrganizationUnitService _organizationUnitService;
    private readonly ICustomerService _customerService;
    private readonly IPermissionService _permissionService;

    public CustomerOrganizationAdminController(
        ICustomerOrganizationUnitService mappingService,
        IOrganizationUnitService organizationUnitService,
        ICustomerService customerService,
        IPermissionService permissionService)
    {
        _mappingService = mappingService;
        _organizationUnitService = organizationUnitService;
        _customerService = customerService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var mappings = await _mappingService.GetAllAsync();
        var normalizedMappings = mappings
            .GroupBy(x => x.CustomerId)
            .Select(g => g.First())
            .ToList();

        if (normalizedMappings.Count != mappings.Count)
        {
            foreach (var duplicate in mappings.Except(normalizedMappings).ToList())
                await _mappingService.DeleteAsync(duplicate);
        }

        var organizations = (await _organizationUnitService.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var customers = (await _customerService.GetAllCustomersAsync()).ToDictionary(x => x.Id, x => $"{x.Email} ({x.Username})");

        var model = normalizedMappings.Select(x => new CustomerOrganizationUnitModel
        {
            Id = x.Id,
            CustomerId = x.CustomerId,
            OrganizationUnitId = x.OrganizationUnitId,
            IsPrimary = x.IsPrimary,
            CanInputReport = x.CanInputReport,
            CanReviewChildReports = x.CanReviewChildReports,
            CanSubmitReport = x.CanSubmitReport,
            CanLockReport = x.CanLockReport,
            CustomerDisplay = customers.TryGetValue(x.CustomerId, out var c) ? c : x.CustomerId.ToString(),
            OrganizationDisplay = organizations.TryGetValue(x.OrganizationUnitId, out var o) ? o : x.OrganizationUnitId.ToString()
        }).ToList();

        return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/List.cshtml", model);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();
        return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(new CustomerOrganizationUnitModel()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerOrganizationUnitModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));

        var duplicated = (await _mappingService.GetAllAsync()).Any(x => x.CustomerId == model.CustomerId && x.OrganizationUnitId == model.OrganizationUnitId);
        if (duplicated)
        {
            ModelState.AddModelError(string.Empty, "Ánh xạ này đã tồn tại.");
            return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
        }

        var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (customer == null)
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng.");
            return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
        }

        customer.VendorId = model.OrganizationUnitId;
        await _customerService.UpdateCustomerAsync(customer);

        var existingByCustomer = (await _mappingService.GetAllAsync())
            .Where(x => x.CustomerId == model.CustomerId)
            .ToList();
        foreach (var stale in existingByCustomer)
            await _mappingService.DeleteAsync(stale);

        var entity = new CustomerOrganizationUnit
        {
            CustomerId = model.CustomerId,
            OrganizationUnitId = model.OrganizationUnitId,
            IsPrimary = model.IsPrimary,
            CanInputReport = model.CanInputReport,
            CanReviewChildReports = model.CanReviewChildReports,
            CanSubmitReport = model.CanSubmitReport,
            CanLockReport = model.CanLockReport
        };

        await _mappingService.InsertAsync(entity);
        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var entity = await _mappingService.GetByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        var model = new CustomerOrganizationUnitModel
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            OrganizationUnitId = entity.OrganizationUnitId,
            IsPrimary = entity.IsPrimary,
            CanInputReport = entity.CanInputReport,
            CanReviewChildReports = entity.CanReviewChildReports,
            CanSubmitReport = entity.CanSubmitReport,
            CanLockReport = entity.CanLockReport
        };

        return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CustomerOrganizationUnitModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));

        var duplicated = (await _mappingService.GetAllAsync()).Any(x => x.Id != model.Id && x.CustomerId == model.CustomerId && x.OrganizationUnitId == model.OrganizationUnitId);
        if (duplicated)
        {
            ModelState.AddModelError(string.Empty, "Ánh xạ này đã tồn tại.");
            return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
        }

        var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (customer == null)
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng.");
            return View("~/Plugins/Misc.LogisticsReports/Views/CustomerOrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
        }

        customer.VendorId = model.OrganizationUnitId;
        await _customerService.UpdateCustomerAsync(customer);

        var existingByCustomer = (await _mappingService.GetAllAsync())
            .Where(x => x.CustomerId == model.CustomerId && x.Id != model.Id)
            .ToList();
        foreach (var stale in existingByCustomer)
            await _mappingService.DeleteAsync(stale);

        var entity = await _mappingService.GetByIdAsync(model.Id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        entity.CustomerId = model.CustomerId;
        entity.OrganizationUnitId = model.OrganizationUnitId;
        entity.IsPrimary = model.IsPrimary;
        entity.CanInputReport = model.CanInputReport;
        entity.CanReviewChildReports = model.CanReviewChildReports;
        entity.CanSubmitReport = model.CanSubmitReport;
        entity.CanLockReport = model.CanLockReport;

        await _mappingService.UpdateAsync(entity);
        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var entity = await _mappingService.GetByIdAsync(id);
        if (entity != null)
        {
            var customer = await _customerService.GetCustomerByIdAsync(entity.CustomerId);
            if (customer != null && customer.VendorId == entity.OrganizationUnitId)
            {
                customer.VendorId = 0;
                await _customerService.UpdateCustomerAsync(customer);
            }

            await _mappingService.DeleteAsync(entity);
        }

        return RedirectToAction(nameof(List));
    }

    private async Task<CustomerOrganizationUnitModel> PrepareModelAsync(CustomerOrganizationUnitModel model)
    {
        model.Customers = (await _customerService.GetAllCustomersAsync())
            .Take(300)
            .Select(x => new SelectItem { Id = x.Id, Name = $"{x.Email} ({x.Username})" }).ToList();

        model.Organizations = (await _organizationUnitService.GetAllAsync())
            .Select(x => new SelectItem { Id = x.Id, Name = x.Path + " - " + x.Name }).ToList();

        return model;
    }
}

