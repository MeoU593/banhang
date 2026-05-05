using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Plugin.Misc.DocumentPortal.Factories;
using Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Core;
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
    private readonly IWorkContext _workContext;

    public DocumentPortalAdminController(
        IDocumentPortalService documentPortalService,
        IDocumentPortalModelFactory documentPortalModelFactory,
        IPermissionService permissionService,
        ISettingService settingService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IWorkContext workContext)
    {
        _documentPortalService = documentPortalService;
        _documentPortalModelFactory = documentPortalModelFactory;
        _permissionService = permissionService;
        _settingService = settingService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _workContext = workContext;
    }

    protected virtual async Task<bool> CheckPermissionAsync()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        return await _documentPortalService.CanAccessDocumentManagementAsync(currentCustomer);
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

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        if (currentCustomer.VendorId > 0)
            model.OwnerVendorId = currentCustomer.VendorId;

        ValidateAccessSettings(model);

        if (!ModelState.IsValid)
        {
            model = await _documentPortalModelFactory.PrepareDocumentModelAsync(model, null, true);
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Create.cshtml", model);
        }

        var settings = await _settingService.LoadSettingAsync<DocumentPortalSettings>();
        var slug = await PrepareUniqueSlugAsync(model.Title, model.Slug, settings.AutoGenerateSlug);

        var entity = new Document
        {
            Title = model.Title,
            Slug = slug,
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
            OwnerVendorId = model.OwnerVendorId,
            AccessScopeId = model.AccessScopeId,
            Published = model.Published,
            AllowDownload = true,
            ShowOnHomepage = model.ShowOnHomepage,
            DisplayOrder = model.DisplayOrder
        };

        if (currentCustomer.VendorId > 0)
            entity.UploadedByCustomerId = currentCustomer.Id;

        await _documentPortalService.InsertAsync(entity);

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

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        if (!await _documentPortalService.CanManageDocumentAsync(entity, currentCustomer))
            return AccessDeniedView();

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

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        if (!await _documentPortalService.CanManageDocumentAsync(entity, currentCustomer))
            return AccessDeniedView();

        if (currentCustomer.VendorId > 0)
            model.OwnerVendorId = currentCustomer.VendorId;

        ValidateAccessSettings(model);

        if (!ModelState.IsValid)
        {
            model = await _documentPortalModelFactory.PrepareDocumentModelAsync(model, entity, true);
            return View("~/Plugins/Misc.DocumentPortal/Views/Admin/Edit.cshtml", model);
        }

        var settings = await _settingService.LoadSettingAsync<DocumentPortalSettings>();
        entity.Title = model.Title;
        entity.Slug = await PrepareUniqueSlugAsync(model.Title, model.Slug, settings.AutoGenerateSlug, entity.Id);
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
        entity.OwnerVendorId = model.OwnerVendorId;
        entity.AccessScopeId = model.AccessScopeId;
        entity.Published = model.Published;
        entity.AllowDownload = true;
        entity.ShowOnHomepage = model.ShowOnHomepage;
        entity.DisplayOrder = model.DisplayOrder;

        await _documentPortalService.UpdateAsync(entity);

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
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        if (entity != null && !entity.Deleted)
        {
            if (!await _documentPortalService.CanManageDocumentAsync(entity, currentCustomer))
                return AccessDeniedView();

            await _documentPortalService.DeleteAsync(entity);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataDeleted"));
        }

        return RedirectToAction(nameof(List));
    }

    private void ValidateAccessSettings(DocumentModel model)
    {
        if (!Enum.IsDefined(typeof(DocumentAccessScope), model.AccessScopeId) || model.AccessScopeId == 0)
            ModelState.AddModelError(nameof(model.AccessScopeId), "Vui lòng chọn phạm vi tài liệu.");
    }

    private async Task<string> PrepareUniqueSlugAsync(string title, string? slug, bool autoGenerateSlug, int currentDocumentId = 0)
    {
        var preparedSlug = autoGenerateSlug || string.IsNullOrWhiteSpace(slug)
            ? GenerateSlug(title)
            : GenerateSlug(slug);

        if (string.IsNullOrWhiteSpace(preparedSlug))
            preparedSlug = $"tai-lieu-{DateTime.UtcNow.Ticks}";

        var uniqueSlug = preparedSlug;
        var suffix = 1;
        while (true)
        {
            var existing = await _documentPortalService.GetBySlugAsync(uniqueSlug);
            if (existing == null || existing.Id == currentDocumentId)
                return uniqueSlug;

            uniqueSlug = $"{preparedSlug}-{suffix++}";
        }
    }

    private static string GenerateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                builder.Append(c == 'đ' ? 'd' : c);
        }

        var slug = System.Text.RegularExpressions.Regex.Replace(builder.ToString().Normalize(NormalizationForm.FormC), @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');

        return slug.Length > 200 ? slug[..200].TrimEnd('-') : slug;
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
        await _settingService.SaveSettingAsync(settings);

_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.DataSaved"));
        return RedirectToAction(nameof(Settings));
    }
}
