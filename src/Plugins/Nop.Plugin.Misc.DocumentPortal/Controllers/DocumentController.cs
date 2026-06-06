using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Plugin.Misc.DocumentPortal.Factories;
using Nop.Plugin.Misc.DocumentPortal.Models.Public;
using Nop.Plugin.Misc.DocumentPortal.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Web.Controllers;

namespace Nop.Plugin.Misc.DocumentPortal.Controllers;

[AutoValidateAntiforgeryToken]
public class DocumentController : BasePublicController
{
    private const long MaxUploadFileSizeBytes = 30 * 1024 * 1024;
    private const int MaxWordPreviewFileSizeBytes = 5 * 1024 * 1024;
    private const int MaxPreviewCharacters = 200_000;

    private readonly IDocumentPortalService _documentPortalService;
    private readonly IDocumentPortalModelFactory _documentPortalModelFactory;
    private readonly IWorkContext _workContext;
    private readonly IDownloadService _downloadService;
    private readonly ICustomerService _customerService;
    private readonly ICustomerActivityService _customerActivityService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger _logger;

    public DocumentController(
        IDocumentPortalService documentPortalService,
        IDocumentPortalModelFactory documentPortalModelFactory,
        IWorkContext workContext,
        IDownloadService downloadService,
        ICustomerService customerService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILogger logger)
    {
        _documentPortalService = documentPortalService;
        _documentPortalModelFactory = documentPortalModelFactory;
        _workContext = workContext;
        _downloadService = downloadService;
        _customerService = customerService;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _logger = logger;
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
        if (!await _documentPortalService.CanViewDocumentAsync(document, customer))
        {
            if (await _customerService.IsGuestAsync(customer))
                return Challenge();

            return Forbid();
        }

        await _documentPortalService.IncrementViewCountAsync(document);
        var model = await _documentPortalModelFactory.PreparePublicDetailModelAsync(document, customer);
        return View("~/Plugins/Misc.DocumentPortal/Views/Public/Detail.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> CommentAdd(int id, DocumentDetailModel model)
    {
        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || !document.Published || document.Deleted)
            return NotFound();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _documentPortalService.CanViewDocumentAsync(document, customer))
        {
            if (await _customerService.IsGuestAsync(customer))
                return Challenge();

            return Forbid();
        }

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var commentText = model.AddNewComment.CommentText?.Trim();
        if (string.IsNullOrWhiteSpace(commentText))
        {
            TempData["DocumentCommentError"] = "Nội dung bình luận không được để trống.";
            return LocalRedirect($"/tai-lieu/{document.Slug}#document-comments");
        }

        var replyToCommentId = 0;
        if (model.AddNewComment.ReplyToCommentId > 0)
        {
            var repliedComment = await _documentPortalService.GetDocumentCommentByIdAsync(model.AddNewComment.ReplyToCommentId);
            if (repliedComment != null && repliedComment.DocumentId == document.Id && repliedComment.IsApproved)
                replyToCommentId = repliedComment.Id;
        }

        if (replyToCommentId > 0)
            commentText = $"[replyto:{replyToCommentId}]\n{commentText}";

        var comment = new DocumentComment
        {
            DocumentId = document.Id,
            CustomerId = customer.Id,
            CommentText = commentText,
            IsApproved = true,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _documentPortalService.InsertDocumentCommentAsync(comment);

        TempData["DocumentCommentSuccess"] = "Đăng bình luận thành công.";
        return LocalRedirect($"/tai-lieu/{document.Slug}#document-comment{comment.Id}");
    }

    public async Task<IActionResult> Download(int id)
    {
        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || !document.Published || document.Deleted || !document.DownloadId.HasValue)
            return NotFound();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _documentPortalService.CanViewDocumentAsync(document, customer))
        {
            if (await _customerService.IsGuestAsync(customer))
                return Challenge();

            return Forbid();
        }

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

    public async Task<IActionResult> Preview(int id)
    {
        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || !document.Published || document.Deleted || !document.DownloadId.HasValue)
            return NotFound();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _documentPortalService.CanViewDocumentAsync(document, customer))
        {
            if (await _customerService.IsGuestAsync(customer))
                return Challenge();

            return Forbid();
        }

        var download = await _downloadService.GetDownloadByIdAsync(document.DownloadId.Value);
        if (download == null || download.DownloadBinary == null)
            return NotFound();

        var fileName = string.IsNullOrWhiteSpace(download.Filename)
            ? document.Title
            : download.Filename;
        var extension = (download.Extension ?? string.Empty).ToLowerInvariant();
        var contentType = GetPreviewContentType(download.ContentType, extension);
        var displayFileName = $"{fileName}{download.Extension}";

        if (extension is ".docx" or ".doc" || IsWordContentType(download.ContentType))
        {
            if (download.DownloadBinary.Length > MaxWordPreviewFileSizeBytes)
                return Content(BuildDocumentPreviewHtml(document.Title, "File Word quá lớn để xem trước an toàn. Vui lòng tải file để xem bản đầy đủ."), "text/html; charset=utf-8");

            var html = extension == ".docx" || IsDocxContentType(download.ContentType)
                ? BuildDocumentPreviewHtml(document.Title, ExtractDocxText(download.DownloadBinary))
                : BuildDocumentPreviewHtml(document.Title, ExtractLegacyDocText(download.DownloadBinary));

            return Content(html, "text/html; charset=utf-8");
        }

        if (extension == ".pdf" || string.Equals(contentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{SanitizeHeaderValue(displayFileName)}\"";

        return File(download.DownloadBinary, contentType);
    }

    [HttpGet]
    public async Task<IActionResult> Upload()
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
                return Challenge();

            var model = await _documentPortalModelFactory.PrepareUploadModelAsync();
            return View("~/Plugins/Misc.DocumentPortal/Views/Public/Upload.cshtml", model);
        }
        catch (Exception ex)
        {
            _ = ex;
            var errorModel = await _documentPortalModelFactory.PrepareUploadModelAsync();
            return View("~/Plugins/Misc.DocumentPortal/Views/Public/Upload.cshtml", errorModel);
        }
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

        if (model.File != null && model.File.Length > MaxUploadFileSizeBytes)
            ModelState.AddModelError(nameof(model.File), "Kích thước file tối đa là 30 MB.");

        if (!Enum.IsDefined(typeof(DocumentAccessScope), model.AccessScopeId) || model.AccessScopeId == 0)
            ModelState.AddModelError(nameof(model.AccessScopeId), "Vui lòng chọn phạm vi tài liệu.");

        if (!ModelState.IsValid)
        {
            model = await _documentPortalModelFactory.PrepareUploadModelAsync(model);
            return View("~/Plugins/Misc.DocumentPortal/Views/Public/Upload.cshtml", model);
        }

        Download? download = null;

        try
        {
            var file = model.File!;
            var fileBytes = await _downloadService.GetDownloadBitsAsync(file);
            var extension = Path.GetExtension(file.FileName);
            var filenameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);

            download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBytes,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Filename = filenameWithoutExt,
                Extension = extension,
                IsNew = true
            };
            await _downloadService.InsertDownloadAsync(download);

            var slug = GenerateSlug(model.Title);
            var existingSlug = await _documentPortalService.GetBySlugAsync(slug);
            if (existingSlug != null)
                slug = $"{slug}-{DateTime.UtcNow.Ticks}";

            var document = new Document
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
                OwnerVendorId = customer.VendorId > 0 ? customer.VendorId : null,
                AccessScopeId = model.AccessScopeId,
                Published = false,
                AllowDownload = true,
                UploadedByCustomerId = customer.Id
            };

            await _documentPortalService.InsertAsync(document);

            await _customerActivityService.InsertActivityAsync(customer, NopActivityLogDefaults.AddNewPortalDocument,
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewPortalDocument"), document.Title), document);

            TempData["UploadSuccess"] = "Tài liệu đã được gửi lên và đang chờ duyệt.";
            return RedirectToRoute(DocumentPortalDefaults.Routes.LIST);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Document upload failed.", ex, customer);

            if (download != null)
            {
                try
                {
                    await _downloadService.DeleteDownloadAsync(download);
                }
                catch (Exception cleanupEx)
                {
                    await _logger.WarningAsync("Failed to cleanup uploaded file after upload error.", cleanupEx, customer);
                }
            }

            ModelState.AddModelError(string.Empty, "Tải tài liệu thất bại. Vui lòng thử lại.");
            model = await _documentPortalModelFactory.PrepareUploadModelAsync(model);
            return View("~/Plugins/Misc.DocumentPortal/Views/Public/Upload.cshtml", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditOwn(int id)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || document.Deleted)
            return NotFound();

        if (!await _documentPortalService.CanManageDocumentAsync(document, customer))
            return Forbid();

        var model = await PrepareEditModelAsync(document);
        return View("~/Plugins/Misc.DocumentPortal/Views/Public/EditOwn.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> EditOwn(int id, DocumentUploadModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || document.Deleted)
            return NotFound();

        if (!await _documentPortalService.CanManageDocumentAsync(document, customer))
            return Forbid();

        model.Id = id;
        model.IsEdit = true;

        if (string.IsNullOrWhiteSpace(model.Title))
            ModelState.AddModelError(nameof(model.Title), "Tiêu đề là bắt buộc.");

        if (model.File != null && model.File.Length > MaxUploadFileSizeBytes)
            ModelState.AddModelError(nameof(model.File), "Kích thước file tối đa là 30 MB.");

        if (!Enum.IsDefined(typeof(DocumentAccessScope), model.AccessScopeId) || model.AccessScopeId == 0)
            ModelState.AddModelError(nameof(model.AccessScopeId), "Vui lòng chọn phạm vi tài liệu.");

        if (!ModelState.IsValid)
        {
            model.HasExistingFile = document.DownloadId.HasValue;
            model.CurrentFileName = await GetDownloadFileNameAsync(document.DownloadId);
            model = await _documentPortalModelFactory.PrepareUploadModelAsync(model);
            return View("~/Plugins/Misc.DocumentPortal/Views/Public/EditOwn.cshtml", model);
        }

        Download? newDownload = null;
        var oldDownloadId = document.DownloadId;
        try
        {
            if (model.File != null && model.File.Length > 0)
            {
                var extension = Path.GetExtension(model.File.FileName);
                newDownload = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    UseDownloadUrl = false,
                    DownloadUrl = string.Empty,
                    DownloadBinary = await _downloadService.GetDownloadBitsAsync(model.File),
                    ContentType = string.IsNullOrWhiteSpace(model.File.ContentType) ? "application/octet-stream" : model.File.ContentType,
                    Filename = Path.GetFileNameWithoutExtension(model.File.FileName),
                    Extension = extension,
                    IsNew = true
                };
                await _downloadService.InsertDownloadAsync(newDownload);
            }

            var regularOwnerEdit = document.UploadedByCustomerId == customer.Id &&
                !await _documentPortalService.IsSystemAdminAsync(customer) &&
                !await _documentPortalService.HasUnitDocumentManageAccessAsync(customer);

            document.Title = model.Title.Trim();
            document.Slug = await PrepareUniqueSlugAsync(document.Title, document.Id);
            document.Code = model.Code?.Trim();
            document.Summary = model.Summary?.Trim();
            document.DocumentCategoryId = model.DocumentCategoryId;
            document.DocumentTypeId = model.DocumentTypeId;
            document.IssuerId = model.IssuerId;
            document.IssuedDate = model.IssuedDate;
            document.AccessScopeId = model.AccessScopeId;
            if (newDownload != null)
                document.DownloadId = newDownload.Id;

            if (regularOwnerEdit)
                document.Published = false;

            await _documentPortalService.UpdateAsync(document);

            if (newDownload != null && oldDownloadId.HasValue && oldDownloadId.Value != newDownload.Id && !await _documentPortalService.IsDownloadInUseAsync(oldDownloadId.Value))
            {
                try
                {
                    var oldDownload = await _downloadService.GetDownloadByIdAsync(oldDownloadId.Value);
                    if (oldDownload != null)
                        await _downloadService.DeleteDownloadAsync(oldDownload);
                }
                catch (Exception cleanupEx)
                {
                    await _logger.WarningAsync("Failed to cleanup replaced document file after edit.", cleanupEx, customer);
                }
            }

            TempData["ProfileDocumentsMessage"] = regularOwnerEdit
                ? "Tài liệu đã được cập nhật và chuyển về trạng thái chờ duyệt."
                : "Tài liệu đã được cập nhật.";

            return Redirect("/customer/profile#profile-tab-documents");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Document edit failed.", ex, customer);

            if (newDownload != null)
            {
                try
                {
                    await _downloadService.DeleteDownloadAsync(newDownload);
                }
                catch (Exception cleanupEx)
                {
                    await _logger.WarningAsync("Failed to cleanup uploaded file after edit error.", cleanupEx, customer);
                }
            }

            ModelState.AddModelError(string.Empty, "Cập nhật tài liệu thất bại. Vui lòng thử lại.");
            model.HasExistingFile = document.DownloadId.HasValue;
            model.CurrentFileName = await GetDownloadFileNameAsync(document.DownloadId);
            model = await _documentPortalModelFactory.PrepareUploadModelAsync(model);
            return View("~/Plugins/Misc.DocumentPortal/Views/Public/EditOwn.cshtml", model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteOwn(int id)
    {
        var document = await _documentPortalService.GetByIdAsync(id);
        if (document == null || document.Deleted)
            return NotFound();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        if (!await _documentPortalService.CanDeleteAsync(document, customer))
            return Forbid();

        await _documentPortalService.DeleteAsync(document);
        TempData["ProfileDocumentsMessage"] = "Tài liệu đã được xóa.";
        return Redirect("/customer/profile#profile-tab-documents");
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

    private async Task<DocumentUploadModel> PrepareEditModelAsync(Document document)
    {
        var model = new DocumentUploadModel
        {
            Id = document.Id,
            IsEdit = true,
            Title = document.Title,
            Code = document.Code,
            Summary = document.Summary,
            DocumentCategoryId = document.DocumentCategoryId,
            DocumentTypeId = document.DocumentTypeId,
            IssuerId = document.IssuerId,
            IssuedDate = document.IssuedDate,
            AccessScopeId = document.AccessScopeId,
            HasExistingFile = document.DownloadId.HasValue,
            CurrentFileName = await GetDownloadFileNameAsync(document.DownloadId)
        };

        return await _documentPortalModelFactory.PrepareUploadModelAsync(model);
    }

    private async Task<string?> GetDownloadFileNameAsync(int? downloadId)
    {
        if (!downloadId.HasValue)
            return null;

        var download = await _downloadService.GetDownloadByIdAsync(downloadId.Value);
        if (download == null)
            return null;

        return $"{download.Filename}{download.Extension}";
    }

    private async Task<string> PrepareUniqueSlugAsync(string title, int currentDocumentId)
    {
        var preparedSlug = GenerateSlug(title);
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

    private static string GetPreviewContentType(string? contentType, string extension)
    {
        if (extension == ".pdf")
            return "application/pdf";

        if (!string.IsNullOrWhiteSpace(contentType) && contentType != "application/octet-stream")
            return contentType;

        return "application/octet-stream";
    }

    private static bool IsWordContentType(string? contentType)
    {
        return IsDocxContentType(contentType)
            || string.Equals(contentType, "application/msword", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDocxContentType(string? contentType)
    {
        return string.Equals(contentType, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.OrdinalIgnoreCase);
    }

    private static string ExtractDocxText(byte[] fileBytes)
    {
        try
        {
            using var stream = new MemoryStream(fileBytes);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
            var documentEntry = archive.GetEntry("word/document.xml");
            if (documentEntry == null)
                return string.Empty;

            using var documentStream = documentEntry.Open();
            var document = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            var paragraphs = document
                .Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)).Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text));

            return LimitPreviewText(string.Join("\n\n", paragraphs));
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string ExtractLegacyDocText(byte[] fileBytes)
    {
        try
        {
            var unicodeText = Encoding.Unicode.GetString(fileBytes);
            var asciiText = Encoding.UTF8.GetString(fileBytes);
            var combined = unicodeText.Length > asciiText.Length ? unicodeText : asciiText;

            combined = Regex.Replace(combined, @"[^\u0009\u000A\u000D\u0020-\u007E\u00A0-\u1EF9]+", " ");
            combined = Regex.Replace(combined, @"[ \t]{2,}", " ");
            combined = Regex.Replace(combined, @"(\r?\n\s*){3,}", "\n\n");

            return LimitPreviewText(combined.Trim());
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string LimitPreviewText(string text)
    {
        if (text.Length <= MaxPreviewCharacters)
            return text;

        return text.Substring(0, MaxPreviewCharacters) + "\n\n... Nội dung xem trước đã được rút gọn để tránh quá tải bộ nhớ.";
    }

    private static string BuildDocumentPreviewHtml(string title, string text)
    {
        var encodedTitle = WebUtility.HtmlEncode(title);
        var encodedText = WebUtility.HtmlEncode(text);

        if (string.IsNullOrWhiteSpace(encodedText))
            encodedText = "Không trích xuất được nội dung xem trước từ file Word này. Vui lòng tải file để xem bản đầy đủ.";

        return $$"""
<!doctype html>
<html lang="vi">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <style>
        body { margin: 0; background: #f8fafc; color: #0f172a; font-family: Arial, sans-serif; }
        .document-preview { max-width: 900px; margin: 0 auto; padding: 32px 28px; background: #fff; min-height: 100vh; box-shadow: 0 20px 50px -40px rgba(15, 23, 42, .6); }
        h1 { margin: 0 0 20px; font-size: 18px; line-height: 1.35; color: #111827; }
        pre { margin: 0; white-space: pre-wrap; word-break: break-word; font: 14px/1.7 Arial, sans-serif; color: #334155; }
    </style>
</head>
<body>
    <main class="document-preview">
        <h1>{{encodedTitle}}</h1>
        <pre>{{encodedText}}</pre>
    </main>
</body>
</html>
""";
    }

    private static string SanitizeHeaderValue(string value)
    {
        return value.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\"", "'");
    }
}
