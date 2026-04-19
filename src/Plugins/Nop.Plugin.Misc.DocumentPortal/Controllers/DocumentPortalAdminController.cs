using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Plugin.Misc.DocumentPortal.Factories;
using Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Configuration;
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
public class DocumentPortalAdminController : BasePluginController
{
    private readonly IDocumentPortalService _documentPortalService;
    private readonly IDocumentPortalModelFactory _documentPortalModelFactory;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;

    public DocumentPortalAdminController(
        IDocumentPortalService documentPortalService,
        IDocumentPortalModelFactory documentPortalModelFactory,
        IPermissionService permissionService,
        ISettingService settingService,
        ILocalizationService localizationService,
        INotificationService notificationService)
    {
        _documentPortalService = documentPortalService;
        _documentPortalModelFactory = documentPortalModelFactory;
        _permissionService = permissionService;
        _settingService = settingService;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    protected virtual async Task<bool> CheckPermissionAsync()
    {
        return await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENTS);
    }

    public async Task<IActionResult> List()
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var model = await _documentPortalModelFactory.PrepareDocumentSearchModelAsync(new DocumentSearchModel());
        return View("~/Plugins/Misc.DocumentPortal/Views/Admin/List.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> List(DocumentSearchModel searchModel)
    {
        if (!await CheckPermissionAsync())
            return await AccessDeniedJsonAsync();

        var model = await _documentPortalModelFactory.PrepareDocumentListModelAsync(searchModel);
        return Json(model);
    }

    public async Task<IActionResult> Create()
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var model = await _documentPortalModelFactory.PrepareDocumentModelAsync(null, null);
        return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Create.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Create(DocumentModel model, bool continueEditing)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        if (!ModelState.IsValid)
        {
            model = await _documentPortalModelFactory.PrepareDocumentModelAsync(model, null, true);
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Create.cshtml", model);
        }

        var entity = new Document
        {
            Title = model.Title,
            Slug = model.Slug,
            Code = model.Code,
            Summary = model.Summary,
            Content = model.Content,
            Keywords = model.Keywords,
            DocumentCategoryId = model.DocumentCategoryId,
            DocumentTypeId = model.DocumentTypeId,
            IssuerId = model.IssuerId,
            IssuedDate = model.IssuedDate,
            EffectiveDate = model.EffectiveDate,
            DownloadId = model.DownloadId,
            ThumbnailPictureId = model.ThumbnailPictureId,
            Published = model.Published,
            AllowDownload = model.AllowDownload,
            ShowOnHomepage = model.ShowOnHomepage,
            DisplayOrder = model.DisplayOrder
        };

        await _documentPortalService.InsertAsync(entity);
        await _documentPortalService.SaveRoleMappingsAsync(entity.Id, model.SelectedCustomerRoleIds);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataSaved"));
        return continueEditing
            ? RedirectToAction(nameof(Edit), new { id = entity.Id })
            : RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var entity = await _documentPortalService.GetByIdAsync(id);
        if (entity == null || entity.Deleted)
            return RedirectToAction(nameof(List));

        var model = await _documentPortalModelFactory.PrepareDocumentModelAsync(null, entity);
        return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Edit(DocumentModel model, bool continueEditing)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var entity = await _documentPortalService.GetByIdAsync(model.Id);
        if (entity == null || entity.Deleted)
            return RedirectToAction(nameof(List));

        if (!ModelState.IsValid)
        {
            model = await _documentPortalModelFactory.PrepareDocumentModelAsync(model, entity, true);
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Edit.cshtml", model);
        }

        entity.Title = model.Title;
        entity.Slug = model.Slug;
        entity.Code = model.Code;
        entity.Summary = model.Summary;
        entity.Content = model.Content;
        entity.Keywords = model.Keywords;
        entity.DocumentCategoryId = model.DocumentCategoryId;
        entity.DocumentTypeId = model.DocumentTypeId;
        entity.IssuerId = model.IssuerId;
        entity.IssuedDate = model.IssuedDate;
        entity.EffectiveDate = model.EffectiveDate;
        entity.DownloadId = model.DownloadId;
        entity.ThumbnailPictureId = model.ThumbnailPictureId;
        entity.Published = model.Published;
        entity.AllowDownload = model.AllowDownload;
        entity.ShowOnHomepage = model.ShowOnHomepage;
        entity.DisplayOrder = model.DisplayOrder;

        await _documentPortalService.UpdateAsync(entity);
        await _documentPortalService.SaveRoleMappingsAsync(entity.Id, model.SelectedCustomerRoleIds);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataSaved"));
        return continueEditing
            ? RedirectToAction(nameof(Edit), new { id = entity.Id })
            : RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await CheckPermissionAsync())
            return AccessDeniedView();

        var entity = await _documentPortalService.GetByIdAsync(id);
        if (entity != null && !entity.Deleted)
        {
            await _documentPortalService.DeleteAsync(entity);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataDeleted"));
        }

        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Settings()
    {
        if (!await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENT_SETTINGS))
            return AccessDeniedView();

        var model = await _documentPortalModelFactory.PrepareConfigurationModelAsync();
        return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Settings(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(DocumentPortalDefaults.Permissions.MANAGE_DOCUMENT_SETTINGS))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Configure.cshtml", model);

        var settings = await _settingService.LoadSettingAsync<DocumentPortalSettings>();
        settings.DefaultPageSize = model.DefaultPageSize;
        settings.SearchInContent = model.SearchInContent;
        settings.ShowRelatedDocuments = model.ShowRelatedDocuments;
        settings.ShowDownloadCount = model.ShowDownloadCount;
        settings.AutoGenerateSlug = model.AutoGenerateSlug;
        settings.RequireLoginForPrivateDocuments = model.RequireLoginForPrivateDocuments;
        await _settingService.SaveSettingAsync(settings);

_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataSaved"));
        return RedirectToAction(nameof(Settings));
    }
}
