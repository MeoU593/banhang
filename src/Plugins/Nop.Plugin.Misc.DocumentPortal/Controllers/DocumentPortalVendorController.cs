using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.DocumentPortal.Factories;
using Nop.Plugin.Misc.DocumentPortal.Models.Vendor;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.DocumentPortal.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
public class DocumentPortalVendorController : BasePluginController
{
    private readonly IDocumentPortalService _documentPortalService;
    private readonly IDocumentPortalModelFactory _documentPortalModelFactory;
    private readonly IWorkContext _workContext;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;

    public DocumentPortalVendorController(
        IDocumentPortalService documentPortalService,
        IDocumentPortalModelFactory documentPortalModelFactory,
        IWorkContext workContext,
        INotificationService notificationService,
        ILocalizationService localizationService)
    {
        _documentPortalService = documentPortalService;
        _documentPortalModelFactory = documentPortalModelFactory;
        _workContext = workContext;
        _notificationService = notificationService;
        _localizationService = localizationService;
    }

    public async Task<IActionResult> List()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer.VendorId <= 0)
            return AccessDeniedView();

        var model = await _documentPortalModelFactory.PrepareVendorDocumentSearchModelAsync(
            new VendorDocumentSearchModel(), customer);
        return View("~/Plugins/Misc.DocumentPortal/Views/Vendor/List.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> List(VendorDocumentSearchModel searchModel)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer.VendorId <= 0)
            return await AccessDeniedJsonAsync();

        var model = await _documentPortalModelFactory.PrepareVendorDocumentListModelAsync(searchModel, customer);
        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer.VendorId <= 0)
            return AccessDeniedView();

        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || document.Deleted)
            return RedirectToAction(nameof(List));

        if (!await _documentPortalService.CanDeleteAsync(document, customer))
            return AccessDeniedView();

        await _documentPortalService.DeleteAsync(document);
        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Admin.Common.DataDeleted"));

        return RedirectToAction(nameof(List));
    }
}
