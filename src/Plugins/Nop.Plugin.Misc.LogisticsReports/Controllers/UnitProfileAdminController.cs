using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Models.UnitProfile;
using Nop.Services.Html;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
public class UnitProfileAdminController : Nop.Web.Areas.Admin.Controllers.BaseAdminController
{
    private readonly IDownloadService _downloadService;
    private readonly IHtmlFormatter _htmlFormatter;
    private readonly IPictureService _pictureService;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IVendorService _vendorService;
    private readonly IWorkContext _workContext;

    public UnitProfileAdminController(
        IDownloadService downloadService,
        IHtmlFormatter htmlFormatter,
        IPictureService pictureService,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IRepository<Vendor> vendorRepository,
        IVendorService vendorService,
        IWorkContext workContext)
    {
        _downloadService = downloadService;
        _htmlFormatter = htmlFormatter;
        _pictureService = pictureService;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _vendorRepository = vendorRepository;
        _vendorService = vendorService;
        _workContext = workContext;
    }

    public async Task<IActionResult> Index()
    {
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null)
            return AccessDeniedView();

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var model = await PrepareModelAsync(currentVendor, currentVendor.PmCustomerId == currentCustomer.Id);

        return View("~/Plugins/Misc.LogisticsReports/Views/UnitProfileAdmin/Index.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Save(UnitProfileModel model, IFormFile uploadedFile)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        Picture? picture = null;
        if (uploadedFile != null && !string.IsNullOrWhiteSpace(uploadedFile.FileName))
        {
            var contentType = uploadedFile.ContentType?.ToLowerInvariant() ?? string.Empty;
            if (!contentType.StartsWith("image/") || contentType.StartsWith("image/svg"))
                ModelState.AddModelError(string.Empty, "Ảnh đại diện không hợp lệ.");
            else
            {
                var pictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                picture = await _pictureService.InsertPictureAsync(pictureBinary, contentType, null);
            }
        }

        if (!ModelState.IsValid)
            return View("~/Plugins/Misc.LogisticsReports/Views/UnitProfileAdmin/Index.cshtml", await PrepareModelAsync(currentVendor, true, model));

        var previousPicture = await _pictureService.GetPictureByIdAsync(currentVendor.PictureId);
        currentVendor.Name = model.Name?.Trim() ?? string.Empty;
        currentVendor.Email = model.Email?.Trim() ?? string.Empty;
        var description = _htmlFormatter.FormatText(model.Description, false, false, true, false, false, false);
        currentVendor.Description = WebUtility.HtmlEncode(description) ?? string.Empty;

        if (picture != null)
        {
            currentVendor.PictureId = picture.Id;
            if (previousPicture != null)
                await _pictureService.DeletePictureAsync(previousPicture);
        }

        await _vendorService.UpdateVendorAsync(currentVendor);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemovePicture()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        var picture = await _pictureService.GetPictureByIdAsync(currentVendor.PictureId);
        if (picture != null)
            await _pictureService.DeletePictureAsync(picture);

        currentVendor.PictureId = 0;
        await _vendorService.UpdateVendorAsync(currentVendor);

        return RedirectToAction(nameof(Index));
    }

    private async Task<UnitProfileModel> PrepareModelAsync(Vendor vendor, bool canEdit, UnitProfileModel? postedModel = null)
    {
        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
        var pictureUrl = picture != null ? (await _pictureService.GetPictureUrlAsync(picture, 160)).Url : string.Empty;
        var parent = vendor.ParentId.HasValue ? await _vendorRepository.Table.FirstOrDefaultAsync(x => x.Id == vendor.ParentId.Value) : null;

        return new UnitProfileModel
        {
            Id = vendor.Id,
            Name = postedModel?.Name ?? vendor.Name ?? string.Empty,
            Email = postedModel?.Email ?? vendor.Email ?? string.Empty,
            Description = postedModel?.Description ?? WebUtility.HtmlDecode(vendor.Description ?? string.Empty),
            PictureUrl = pictureUrl,
            Code = vendor.Code ?? string.Empty,
            Level = vendor.Level,
            ParentName = parent?.Name ?? string.Empty,
            Active = vendor.Active,
            CanEdit = canEdit,
            ChildUnitCount = await _vendorRepository.Table.CountAsync(x => x.ParentId == vendor.Id && !x.Deleted),
            ProductCount = await _productRepository.Table.CountAsync(x => x.VendorId == vendor.Id && !x.Deleted),
            StaffCount = await _customerRepository.Table.CountAsync(x => x.VendorId == vendor.Id && !x.Deleted)
        };
    }
}
