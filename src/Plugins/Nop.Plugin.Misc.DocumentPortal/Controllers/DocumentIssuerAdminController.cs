using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.DocumentPortal.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
public class DocumentIssuerAdminController : BasePluginController
{
    private readonly IDocumentPortalService _documentPortalService;
    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;

    public DocumentIssuerAdminController(
        IDocumentPortalService documentPortalService,
        IPermissionService permissionService,
        INotificationService notificationService,
        ILocalizationService localizationService)
    {
        _documentPortalService = documentPortalService;
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
    }

    private async Task<bool> CheckPermissionAsync()
    {
        return await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENT_SETTINGS);
    }

    public async Task<IActionResult> List()
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var items = await _documentPortalService.GetAllIssuersForAdminAsync();
        return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Issuer/List.cshtml", items);
    }

    public async Task<IActionResult> Create()
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Issuer/CreateOrEdit.cshtml", new DocumentIssuerManageModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(DocumentIssuerManageModel model)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        model.Name = model.Name?.Trim() ?? string.Empty;

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Issuer/CreateOrEdit.cshtml", model);

        var all = await _documentPortalService.GetAllIssuersForAdminAsync();
        if (all.Any(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Name), "Tên cơ quan ban hành đã tồn tại.");
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Issuer/CreateOrEdit.cshtml", model);
        }

        await _documentPortalService.InsertIssuerAsync(new DocumentIssuer
        {
            Name = model.Name,
            Description = model.Description?.Trim(),
            DisplayOrder = model.DisplayOrder
        });

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataSaved"));
        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var entity = await _documentPortalService.GetIssuerByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        var model = new DocumentIssuerManageModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            DisplayOrder = entity.DisplayOrder
        };

        return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Issuer/CreateOrEdit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DocumentIssuerManageModel model)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var entity = await _documentPortalService.GetIssuerByIdAsync(model.Id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        model.Name = model.Name?.Trim() ?? string.Empty;

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Issuer/CreateOrEdit.cshtml", model);

        var all = await _documentPortalService.GetAllIssuersForAdminAsync();
        if (all.Any(x => x.Id != model.Id && x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Name), "Tên cơ quan ban hành đã tồn tại.");
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Issuer/CreateOrEdit.cshtml", model);
        }

        entity.Name = model.Name;
        entity.Description = model.Description?.Trim();
        entity.DisplayOrder = model.DisplayOrder;
        await _documentPortalService.UpdateIssuerAsync(entity);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataSaved"));
        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var entity = await _documentPortalService.GetIssuerByIdAsync(id);
        if (entity == null)
            return RedirectToAction(nameof(List));

        if (await _documentPortalService.IsIssuerInUseAsync(id))
        {
            _notificationService.ErrorNotification("Cơ quan ban hành đang được sử dụng bởi tài liệu, không thể xóa.");
            return RedirectToAction(nameof(List));
        }

        await _documentPortalService.DeleteIssuerAsync(entity);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataDeleted"));
        return RedirectToAction(nameof(List));
    }
}
