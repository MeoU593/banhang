using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Models.Organizations;
using Nop.Plugin.Misc.LogisticsReports.Services;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
public class OrganizationAdminController : BaseAdminController
{
    private readonly IOrganizationUnitService _organizationUnitService;
    private readonly ICustomerOrganizationUnitService _customerOrganizationUnitService;
    private readonly IPermissionService _permissionService;

    public OrganizationAdminController(
        IOrganizationUnitService organizationUnitService,
        ICustomerOrganizationUnitService customerOrganizationUnitService,
        IPermissionService permissionService)
    {
        _organizationUnitService = organizationUnitService;
        _customerOrganizationUnitService = customerOrganizationUnitService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var items = await _organizationUnitService.GetAllAsync();
        return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/List.cshtml", items);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var root = await _organizationUnitService.EnsureRootUnitAsync();
        var model = await PrepareModelAsync(new OrganizationUnitModel
        {
            ParentId = root.Id,
            IsActive = true
        });

        return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/CreateOrEdit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrganizationUnitModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var root = await _organizationUnitService.EnsureRootUnitAsync();

        if (!model.ParentId.HasValue)
            model.ParentId = root.Id;

        var parent = await _organizationUnitService.GetByIdAsync(model.ParentId.Value);
        if (parent == null)
            ModelState.AddModelError(nameof(model.ParentId), "Đơn vị cấp trên không tồn tại.");

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));

        await _organizationUnitService.InsertAsync(new OrganizationUnit
        {
            Code = model.Code,
            Name = model.Name,
            ParentId = model.ParentId,
            DisplayOrder = model.DisplayOrder,
            IsActive = model.IsActive
        });

        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var entity = await _organizationUnitService.GetByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        var root = await _organizationUnitService.EnsureRootUnitAsync();
        var model = new OrganizationUnitModel
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            ParentId = entity.ParentId,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive,
            IsRootUnit = entity.Id == root.Id
        };

        model = await PrepareModelAsync(model);
        return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/CreateOrEdit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(OrganizationUnitModel model)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var root = await _organizationUnitService.EnsureRootUnitAsync();
        model.IsRootUnit = model.Id == root.Id;

        if (model.IsRootUnit)
            model.ParentId = null;
        else if (!model.ParentId.HasValue)
            model.ParentId = root.Id;

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));

        if (!model.IsRootUnit && model.ParentId == model.Id)
        {
            ModelState.AddModelError(nameof(model.ParentId), "Đơn vị cấp trên không được trùng với chính nó.");
            return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
        }

        if (!model.IsRootUnit)
        {
            var parent = await _organizationUnitService.GetByIdAsync(model.ParentId!.Value);
            if (parent == null)
            {
                ModelState.AddModelError(nameof(model.ParentId), "Đơn vị cấp trên không tồn tại.");
                return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
            }

            var descendants = await _organizationUnitService.GetDescendantsAsync(model.Id);
            if (descendants.Any(x => x.Id == model.ParentId.Value))
            {
                ModelState.AddModelError(nameof(model.ParentId), "Đơn vị cấp trên không được là đơn vị cấp dưới của chính nó.");
                return View("~/Plugins/Misc.LogisticsReports/Views/OrganizationAdmin/CreateOrEdit.cshtml", await PrepareModelAsync(model));
            }
        }

        var entity = await _organizationUnitService.GetByIdAsync(model.Id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        entity.Code = model.Code;
        entity.Name = model.Name;
        entity.ParentId = model.ParentId;
        entity.DisplayOrder = model.DisplayOrder;
        entity.IsActive = model.IsActive;
        await _organizationUnitService.UpdateAsync(entity);
        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(LogisticsReportsPermissionConfigManager.ManageOrganizationUnits))
            return AccessDeniedView();

        var entity = await _organizationUnitService.GetByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        var root = await _organizationUnitService.EnsureRootUnitAsync();
        if (entity.Id == root.Id)
            return RedirectToAction(nameof(List));

        var children = await _organizationUnitService.GetChildrenAsync(entity.Id);
        if (children.Any())
            return RedirectToAction(nameof(List));

        var mappings = await _customerOrganizationUnitService.GetByOrganizationUnitIdAsync(entity.Id);
        if (mappings.Any())
            return RedirectToAction(nameof(List));

        await _organizationUnitService.DeleteAsync(entity);
        return RedirectToAction(nameof(List));
    }

    private async Task<OrganizationUnitModel> PrepareModelAsync(OrganizationUnitModel model)
    {
        var root = await _organizationUnitService.EnsureRootUnitAsync();
        var organizations = await _organizationUnitService.GetAllAsync();

        var availableParents = organizations
            .Where(x => x.Id != model.Id)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{new string('-', Math.Max(0, x.Level) * 2)} {x.Name}".TrimStart()
            })
            .ToList();

        model.AvailableParentUnits = availableParents;

        if (!model.IsRootUnit && !model.ParentId.HasValue)
            model.ParentId = root.Id;

        return model;
    }
}

