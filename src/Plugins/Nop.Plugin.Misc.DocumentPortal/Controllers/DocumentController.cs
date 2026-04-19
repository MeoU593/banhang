using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.DocumentPortal.Factories;
using Nop.Plugin.Misc.DocumentPortal.Models.Public;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Web.Controllers;

namespace Nop.Plugin.Misc.DocumentPortal.Controllers;

[AutoValidateAntiforgeryToken]
public class DocumentController : BasePublicController
{
    private readonly IDocumentPortalService _documentPortalService;
    private readonly IDocumentPortalModelFactory _documentPortalModelFactory;
    private readonly IWorkContext _workContext;
    private readonly IDownloadService _downloadService;
    private readonly ICustomerService _customerService;

    public DocumentController(
        IDocumentPortalService documentPortalService,
        IDocumentPortalModelFactory documentPortalModelFactory,
        IWorkContext workContext,
        IDownloadService downloadService,
        ICustomerService customerService)
    {
        _documentPortalService = documentPortalService;
        _documentPortalModelFactory = documentPortalModelFactory;
        _workContext = workContext;
        _downloadService = downloadService;
        _customerService = customerService;
    }

    public async Task<IActionResult> List(DocumentSearchModel search)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var model = await _documentPortalModelFactory.PrepareDocumentListPageModelAsync(search, customer);
        return View("~/Plugins/Misc.DocumentPortal/Views/Public/List.cshtml", model);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var document = await _documentPortalService.GetBySlugAsync(slug);
        if (document == null || !document.Published || document.Deleted)
            return NotFound();

        var customer = await _workContext.GetCurrentCustomerAsync();
        await _documentPortalService.IncrementViewCountAsync(document);
        var model = await _documentPortalModelFactory.PreparePublicDetailModelAsync(document, customer);
        return View("~/Plugins/Misc.DocumentPortal/Views/Public/Detail.cshtml", model);
    }

    public async Task<IActionResult> Download(int id)
    {
        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || !document.Published || document.Deleted || !document.AllowDownload || !document.DownloadId.HasValue)
            return NotFound();

        var download = await _downloadService.GetDownloadByIdAsync(document.DownloadId.Value);
        if (download == null || download.DownloadBinary == null)
            return NotFound();

        await _documentPortalService.IncrementDownloadCountAsync(document);

        var fileName = string.IsNullOrWhiteSpace(download.Filename)
            ? document.Title
            : download.Filename;

        return File(download.DownloadBinary, download.ContentType ?? "application/octet-stream",
            $"{fileName}{download.Extension}");
    }

    [HttpGet]
    public async Task<IActionResult> Upload()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var model = await _documentPortalModelFactory.PrepareUploadModelAsync();
        return View("~/Plugins/Misc.DocumentPortal/Views/Public/Upload.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(DocumentUploadModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        if (string.IsNullOrWhiteSpace(model.Title))
            ModelState.AddModelError(nameof(model.Title), "Tiêu đề là bắt buộc.");

        if (model.File == null || model.File.Length == 0)
            ModelState.AddModelError(nameof(model.File), "Vui lòng chọn file để tải lên.");

        if (!ModelState.IsValid)
        {
            model = await _documentPortalModelFactory.PrepareUploadModelAsync(model);
            return View("~/Plugins/Misc.DocumentPortal/Views/Public/Upload.cshtml", model);
        }

        var file = model.File!;
        byte[] fileBytes;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            fileBytes = ms.ToArray();
        }

        var extension = Path.GetExtension(file.FileName);
        var filenameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);

        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = false,
            DownloadUrl = string.Empty,
            DownloadBinary = fileBytes,
            ContentType = file.ContentType,
            Filename = filenameWithoutExt,
            Extension = extension,
            IsNew = true
        };
        await _downloadService.InsertDownloadAsync(download);

        var slug = GenerateSlug(model.Title);
        var existingSlug = await _documentPortalService.GetBySlugAsync(slug);
        if (existingSlug != null)
            slug = $"{slug}-{DateTime.UtcNow.Ticks}";

        var document = new Domain.Document
        {
            Title = model.Title.Trim(),
            Slug = slug,
            Code = model.Code?.Trim(),
            Summary = model.Summary?.Trim(),
            DocumentCategoryId = model.DocumentCategoryId,
            DocumentTypeId = model.DocumentTypeId,
            IssuerId = model.IssuerId,
            IssuedDate = model.IssuedDate,
            DownloadId = download.Id,
            Published = false,
            AllowDownload = true,
            UploadedByCustomerId = customer.Id,
            UploadedByVendorId = customer.VendorId > 0 ? customer.VendorId : null
        };

        await _documentPortalService.InsertAsync(document);

        TempData["UploadSuccess"] = "Tài liệu đã được gửi lên và đang chờ duyệt.";
        return RedirectToRoute(DocumentPortalDefaults.Routes.LIST);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || document.Deleted)
            return NotFound();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _documentPortalService.CanDeleteAsync(document, customer))
            return Forbid();

        await _documentPortalService.DeleteAsync(document);
        return RedirectToRoute(DocumentPortalDefaults.Routes.LIST);
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace("à", "a").Replace("á", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
            .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
            .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
            .Replace("đ", "d")
            .Replace("è", "e").Replace("é", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
            .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
            .Replace("ì", "i").Replace("í", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
            .Replace("ò", "o").Replace("ó", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
            .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
            .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
            .Replace("ù", "u").Replace("ú", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
            .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
            .Replace("ỳ", "y").Replace("ý", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y");

        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');

        if (slug.Length > 200) slug = slug[..200].TrimEnd('-');

        return slug;
    }
}
