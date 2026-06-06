using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class VendorController : BaseAdminController
{
    private const string VietnamCountryIsoCode = "VN";
    private const int NewsPostTypeId = 0;
    private const int BlogDocumentPostTypeId = 1;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    #region Fields

    protected readonly ForumSettings _forumSettings;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly IAttributeParser<VendorAttribute, VendorAttributeValue> _vendorAttributeParser;
    protected readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerMilitaryProfileService _customerMilitaryProfileService;
    protected readonly ICustomerService _customerService;
    protected readonly ICountryService _countryService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IRepository<BlogPost> _blogPostRepository;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<UnitDirectoryEntry> _unitDirectoryRepository;
    protected readonly IRepository<Vendor> _vendorRepository;
    protected readonly IVendorModelFactory _vendorModelFactory;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public VendorController(ForumSettings forumSettings,
        IAddressService addressService,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeParser<VendorAttribute, VendorAttributeValue> vendorAttributeParser,
        IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService,
        ICustomerActivityService customerActivityService,
        ICustomerMilitaryProfileService customerMilitaryProfileService,
        ICustomerService customerService,
        ICountryService countryService,
        INopDataProvider dataProvider,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IUrlRecordService urlRecordService,
        IRepository<BlogPost> blogPostRepository,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IRepository<UnitDirectoryEntry> unitDirectoryRepository,
        IRepository<Vendor> vendorRepository,
        IVendorModelFactory vendorModelFactory,
        IVendorService vendorService,
        IWorkContext workContext)
    {
        _forumSettings = forumSettings;
        _addressService = addressService;
        _addressAttributeParser = addressAttributeParser;
        _vendorAttributeParser = vendorAttributeParser;
        _vendorAttributeService = vendorAttributeService;
        _customerActivityService = customerActivityService;
        _customerMilitaryProfileService = customerMilitaryProfileService;
        _customerService = customerService;
        _countryService = countryService;
        _dataProvider = dataProvider;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _urlRecordService = urlRecordService;
        _blogPostRepository = blogPostRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitDirectoryRepository = unitDirectoryRepository;
        _vendorRepository = vendorRepository;
        _vendorModelFactory = vendorModelFactory;
        _vendorService = vendorService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdatePictureSeoNamesAsync(Vendor vendor)
    {
        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
        if (picture != null)
            await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(vendor.Name));
    }

    protected virtual async Task UpdateLocalesAsync(Vendor vendor, VendorModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.Name,
                localized.Name,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.Description,
                localized.Description,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);

            await _localizedEntityService.SaveLocalizedValueAsync(vendor,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(vendor, localized.SeName, localized.Name, false);
            await _urlRecordService.SaveSlugAsync(vendor, seName, localized.LanguageId);
        }
    }

    protected virtual async Task<string> ParseVendorAttributesAsync(IFormCollection form)
    {
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = string.Empty;
        var vendorAttributes = await _vendorAttributeService.GetAllAttributesAsync();
        foreach (var attribute in vendorAttributes)
        {
            var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
            StringValues ctrlAttributes;
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                    ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var selectedAttributeId = int.Parse(ctrlAttributes);
                        if (selectedAttributeId > 0)
                        {
                            attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }
                    }

                    break;
                case AttributeControlType.Checkboxes:
                    var cblAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cblAttributes))
                    {
                        foreach (var item in cblAttributes.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var selectedAttributeId = int.Parse(item);
                            if (selectedAttributeId > 0)
                            {
                                attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                    }

                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                    //load read-only (already server-side selected) values
                    var attributeValues = await _vendorAttributeService.GetAttributeValuesAsync(attribute.Id);
                    foreach (var selectedAttributeId in attributeValues
                                 .Where(v => v.IsPreSelected)
                                 .Select(v => v.Id)
                                 .ToList())
                    {
                        attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                    }

                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                    ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var enteredText = ctrlAttributes.ToString().Trim();
                        attributesXml = _vendorAttributeParser.AddAttribute(attributesXml,
                            attribute, enteredText);
                    }

                    break;
                case AttributeControlType.Datepicker:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.FileUpload:
                //not supported vendor attributes
                default:
                    break;
            }
        }

        return attributesXml;
    }

    protected virtual async Task NormalizeVendorAddressAsync(AddressModel model)
    {
        if (model == null)
            return;

        var vietnamCountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(VietnamCountryIsoCode))?.Id;
        if (vietnamCountryId.HasValue)
            model.CountryId = vietnamCountryId.Value;

        model.FirstName = null;
        model.LastName = null;
        model.County = null;
        model.Address2 = null;
        model.FaxNumber = null;
    }

    protected virtual async Task ValidateParentVendorAsync(int vendorId, int? parentId)
    {
        if (!parentId.HasValue || parentId <= 0)
            return;

        var allVendors = await _vendorService.GetAllVendorsAsync(showHidden: true, pageSize: int.MaxValue);
        var byId = allVendors.ToDictionary(v => v.Id, v => v);

        if (!byId.ContainsKey(parentId.Value))
        {
            ModelState.AddModelError(nameof(VendorModel.ParentId), "Đơn vị cấp trên không tồn tại.");
            return;
        }

        if (vendorId <= 0)
            return;

        if (parentId.Value == vendorId || WouldCreateHierarchyCycle(vendorId, parentId, byId))
            ModelState.AddModelError(nameof(VendorModel.ParentId), "Không thể chọn đơn vị cấp trên là chính đơn vị hoặc đơn vị cấp dưới của nó.");
    }

    protected virtual async Task ValidateVendorManagerAssignmentAsync(int? pmCustomerId, int? vendorId)
    {
        if (!pmCustomerId.HasValue || pmCustomerId <= 0)
            return;

        var manager = await _customerService.GetCustomerByIdAsync(pmCustomerId.Value);
        if (manager == null)
        {
            ModelState.AddModelError(nameof(VendorModel.PmCustomerId), "Tài khoản trưởng đơn vị không tồn tại.");
            return;
        }

        var assignedVendorId = vendorId.GetValueOrDefault();
        if (assignedVendorId > 0)
        {
            if (manager.VendorId > 0 && manager.VendorId != assignedVendorId)
                ModelState.AddModelError(nameof(VendorModel.PmCustomerId), "Tài khoản này đang thuộc đơn vị khác.");
        }
        else if (manager.VendorId > 0)
        {
            ModelState.AddModelError(nameof(VendorModel.PmCustomerId), "Tài khoản này đang thuộc đơn vị khác.");
        }

        if (await _vendorRepository.Table.AnyAsync(v => v.PmCustomerId == pmCustomerId.Value && (!vendorId.HasValue || v.Id != vendorId.Value)))
            ModelState.AddModelError(nameof(VendorModel.PmCustomerId), "Mỗi tài khoản chỉ có thể làm trưởng đơn vị của một đơn vị duy nhất.");
    }

    private static bool WouldCreateHierarchyCycle(int vendorId, int? parentId, IDictionary<int, Vendor> byId)
    {
        var currentParentId = parentId;
        var visited = new HashSet<int> { vendorId };

        while (currentParentId.HasValue && currentParentId.Value > 0)
        {
            if (!visited.Add(currentParentId.Value))
                return true;

            if (!byId.TryGetValue(currentParentId.Value, out var parent))
                return false;

            currentParentId = parent.ParentId;
        }

        return false;
    }

    #endregion

    #region Vendors

    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE])]
    public virtual async Task<IActionResult> AddCustomerToVendorPopup()
    {
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorCustomerSearchModelAsync(new VendorCustomerSearchModel());

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE])]
    public virtual async Task<IActionResult> AddCustomerToVendorPopup([Bind(Prefix = nameof(AddCustomerToVendorModel))] AddCustomerToVendorModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (customer == null)
            return Content("Cannot load a customer");

        ViewBag.RefreshPage = true;
        ViewBag.customerId = customer.Id;
        ViewBag.customerInfo = customer.Email;

        return View(new VendorCustomerSearchModel());
    }

    [HttpPost]
    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE])]
    public virtual async Task<IActionResult> AddCustomerToVendorPopupList(VendorCustomerSearchModel searchModel)
    {
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorCustomerListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> ExportStaffExcel(int id)
    {
        var vendor = await GetAccessibleVendorAsync(id);
        if (vendor == null)
            return AccessDeniedView();

        var staff = await PrepareVendorStaffExportRowsAsync(vendor);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Danh sách nhân viên");
        var headers = new[] { "Tên", "Số điện thoại", "Gmail", "Cấp bậc", "Chức vụ", "Quyền hạn" };

        for (var i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
        }

        for (var row = 0; row < staff.Count; row++)
        {
            var item = staff[row];
            worksheet.Cell(row + 2, 1).Value = item.FullName;
            worksheet.Cell(row + 2, 2).Value = item.Phone;
            worksheet.Cell(row + 2, 3).Value = item.Email;
            worksheet.Cell(row + 2, 4).Value = item.Rank;
            worksheet.Cell(row + 2, 5).Value = item.PositionTitle;
            worksheet.Cell(row + 2, 6).Value = item.RoleNames;
        }

        worksheet.Columns().AdjustToContents();
        await using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), MimeTypes.TextXlsx, $"nhan-vien-don-vi-{vendor.Id}.xlsx");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> ExportStaffPdf(int id)
    {
        var vendor = await GetAccessibleVendorAsync(id);
        if (vendor == null)
            return AccessDeniedView();

        var staff = await PrepareVendorStaffExportRowsAsync(vendor);
        await using var stream = new MemoryStream();
        using (var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 24, 24, 24, 24))
        {
            PdfWriter.GetInstance(document, stream);
            document.Open();

            var fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var titleFont = new Font(baseFont, 14, Font.BOLD);
            var headerFont = new Font(baseFont, 10, Font.BOLD);
            var bodyFont = new Font(baseFont, 9, Font.NORMAL);

            document.Add(new Paragraph($"Danh sách nhân viên - {vendor.Name}", titleFont) { SpacingAfter = 12 });
            var table = new PdfPTable(6) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 22, 16, 20, 14, 18, 24 });

            foreach (var header in new[] { "Tên", "Số điện thoại", "Gmail", "Cấp bậc", "Chức vụ", "Quyền hạn" })
                table.AddCell(new PdfPCell(new Phrase(header, headerFont)) { BackgroundColor = new BaseColor(211, 211, 211), Padding = 5 });

            foreach (var item in staff)
            {
                table.AddCell(new Phrase(item.FullName, bodyFont));
                table.AddCell(new Phrase(item.Phone, bodyFont));
                table.AddCell(new Phrase(item.Email, bodyFont));
                table.AddCell(new Phrase(item.Rank, bodyFont));
                table.AddCell(new Phrase(item.PositionTitle, bodyFont));
                table.AddCell(new Phrase(item.RoleNames, bodyFont));
            }

            document.Add(table);
            document.Close();
        }

        return File(stream.ToArray(), MimeTypes.ApplicationPdf, $"nhan-vien-don-vi-{vendor.Id}.pdf");
    }

    protected virtual async Task<Vendor> GetAccessibleVendorAsync(int id)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id);
        if (vendor == null || vendor.Deleted)
            return null;

        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null)
            return await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_VIEW) ? vendor : null;

        var allowedVendorIds = await _vendorService.GetDescendantVendorIdsAsync(currentVendor.Id, includeSelf: true);
        return allowedVendorIds.Contains(vendor.Id) ? vendor : null;
    }

    protected virtual async Task<IList<VendorStaffExportRow>> PrepareVendorStaffExportRowsAsync(Vendor vendor)
    {
        var customers = await _customerService.GetAllCustomersAsync(vendorId: vendor.Id);
        var rows = new List<VendorStaffExportRow>();

        foreach (var customer in customers)
        {
            var militaryProfile = await _customerMilitaryProfileService.GetByCustomerIdAsync(customer.Id);
            var roles = await _customerService.GetCustomerRolesAsync(customer, showHidden: true);
            rows.Add(new VendorStaffExportRow
            {
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Phone = customer.Phone ?? string.Empty,
                Email = customer.Email ?? string.Empty,
                Rank = militaryProfile?.Rank ?? string.Empty,
                PositionTitle = militaryProfile?.PositionTitle ?? string.Empty,
                RoleNames = string.Join(", ", roles.Where(role => role.Active).Select(GetRoleDisplayName))
            });
        }

        return rows;
    }

    protected virtual string GetRoleDisplayName(CustomerRole role)
    {
        return role.Name;
    }

    protected virtual async Task<VendorDetailsModel> PrepareVendorDetailsModelAsync(Vendor vendor, Customer currentCustomer, bool includeAllVendors = false)
    {
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var scopeVendorIds = includeAllVendors
            ? await _vendorRepository.Table.Where(x => !x.Deleted && x.Active).Select(x => x.Id).ToListAsync()
            : (await _vendorService.GetDescendantVendorIdsAsync(vendor.Id, includeSelf: true)).ToList();
        if (!includeAllVendors && currentVendor != null)
        {
            var allowedVendorIds = (await _vendorService.GetDescendantVendorIdsAsync(currentVendor.Id, includeSelf: true)).ToHashSet();
            scopeVendorIds = scopeVendorIds.Where(allowedVendorIds.Contains).ToList();
        }

        if (!includeAllVendors && !scopeVendorIds.Any())
            scopeVendorIds.Add(vendor.Id);

        var vendorsById = await _vendorRepository.Table
            .Where(x => scopeVendorIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name);
        var parent = !includeAllVendors && vendor.ParentId.HasValue
            ? await _vendorRepository.Table.FirstOrDefaultAsync(x => x.Id == vendor.ParentId.Value && !x.Deleted)
            : null;

        var canEditVendor = !includeAllVendors &&
            (await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, currentCustomer) ||
                (currentVendor != null && currentVendor.PmCustomerId == currentCustomer.Id));

        var productQuery = _productRepository.Table.Where(x => !x.Deleted && (includeAllVendors || scopeVendorIds.Contains(x.VendorId)));
        var newsQuery = _blogPostRepository.Table.Where(x => x.PostTypeId == NewsPostTypeId && (includeAllVendors || scopeVendorIds.Contains(x.VendorId)));
        var blogDocumentQuery = _blogPostRepository.Table.Where(x => x.PostTypeId == BlogDocumentPostTypeId && (includeAllVendors || scopeVendorIds.Contains(x.VendorId)));
        var staffQuery = _customerRepository.Table.Where(x => !x.Deleted && x.VendorId > 0 && (includeAllVendors || scopeVendorIds.Contains(x.VendorId)));
        var directoryQuery = _unitDirectoryRepository.Table.Where(x => includeAllVendors || scopeVendorIds.Contains(x.VendorId));
        var portalDocuments = await GetPortalDocumentsAsync(scopeVendorIds, includeAllVendors);
        var reports = await GetUnitReportsAsync(scopeVendorIds, includeAllVendors);

        var labels = BuildMonthLabels();
        var productDates = await productQuery.Select(x => x.CreatedOnUtc).ToListAsync();
        var newsDates = await newsQuery.Select(x => x.StartDateUtc ?? x.CreatedOnUtc).ToListAsync();
        var blogDocumentDates = await blogDocumentQuery.Select(x => x.StartDateUtc ?? x.CreatedOnUtc).ToListAsync();
        var portalDocumentDates = portalDocuments.Select(x => x.CreatedOnUtc).ToList();
        var staffDates = await staffQuery.Select(x => x.CreatedOnUtc).ToListAsync();
        var reportDates = reports.Select(x => x.SubmittedOnUtc ?? x.LockedOnUtc ?? DateTime.MinValue).Where(x => x != DateTime.MinValue).ToList();

        var model = new VendorDetailsModel
        {
            Id = includeAllVendors ? 0 : vendor.Id,
            Name = includeAllVendors ? "Tổng hệ thống" : vendor.Name ?? string.Empty,
            Code = includeAllVendors ? string.Empty : vendor.Code ?? string.Empty,
            ParentName = parent?.Name ?? string.Empty,
            ScopeUnitCount = scopeVendorIds.Count,
            CanEditVendor = canEditVendor,
            IsSystemDashboard = includeAllVendors,
            ProductCount = await productQuery.CountAsync(),
            NewsCount = await newsQuery.CountAsync(),
            BlogDocumentCount = await blogDocumentQuery.CountAsync(),
            PortalDocumentCount = portalDocuments.Count,
            StaffCount = await staffQuery.CountAsync(),
            ReportCount = reports.Count,
            ChartLabelsJson = JsonSerializer.Serialize(labels.Select(x => x.Label), JsonOptions),
            ProductSeriesJson = JsonSerializer.Serialize(CountByMonth(productDates, labels), JsonOptions),
            NewsSeriesJson = JsonSerializer.Serialize(CountByMonth(newsDates, labels), JsonOptions),
            BlogDocumentSeriesJson = JsonSerializer.Serialize(CountByMonth(blogDocumentDates, labels), JsonOptions),
            PortalDocumentSeriesJson = JsonSerializer.Serialize(CountByMonth(portalDocumentDates, labels), JsonOptions),
            StaffSeriesJson = JsonSerializer.Serialize(CountByMonth(staffDates, labels), JsonOptions),
            ReportSeriesJson = JsonSerializer.Serialize(CountByMonth(reportDates, labels), JsonOptions)
        };

        model.Products = (await productQuery.OrderByDescending(x => x.CreatedOnUtc).Take(8).ToListAsync())
            .Select(x => new VendorDetailsListItemModel
            {
                Title = x.Name,
                Subtitle = GetVendorName(vendorsById, x.VendorId),
                CreatedOnUtc = x.CreatedOnUtc,
                Url = Url.Action("Edit", "Product", new { id = x.Id }) ?? string.Empty
            })
            .ToList();

        model.News = (await newsQuery.OrderByDescending(x => x.StartDateUtc ?? x.CreatedOnUtc).Take(8).ToListAsync())
            .Select(x => ToBlogDetailsListItem(x, vendorsById))
            .ToList();

        model.BlogDocuments = (await blogDocumentQuery.OrderByDescending(x => x.StartDateUtc ?? x.CreatedOnUtc).Take(8).ToListAsync())
            .Select(x => ToBlogDetailsListItem(x, vendorsById))
            .ToList();

        model.PortalDocuments = portalDocuments.OrderByDescending(x => x.CreatedOnUtc).Take(8)
            .Select(x => new VendorDetailsListItemModel
            {
                Title = x.Title,
                Subtitle = x.Code ?? GetVendorName(vendorsById, x.OwnerVendorId ?? 0),
                CreatedOnUtc = x.CreatedOnUtc,
                Url = Url.Action("Edit", "DocumentPortalAdmin", new { id = x.Id }) ?? string.Empty
            })
            .ToList();

        model.Reports = reports.OrderByDescending(x => x.SubmittedOnUtc ?? x.LockedOnUtc ?? DateTime.MinValue).Take(8)
            .Select(x => new VendorDetailsListItemModel
            {
                Title = $"Báo cáo #{x.Id}",
                Subtitle = $"Kỳ {x.ReportPeriodId} - {GetReportStatusName(x.StatusId)}",
                CreatedOnUtc = x.SubmittedOnUtc ?? x.LockedOnUtc,
                Url = Url.Action("Edit", "UnitReportAdmin", new { id = x.Id }) ?? string.Empty
            })
            .ToList();

        var staff = await staffQuery.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.Email).Take(20).ToListAsync();
        model.Staff = await PrepareVendorDetailsStaffAsync(staff, includeAllVendors ? null : vendor.PmCustomerId);

        model.DirectoryEntries = await directoryQuery
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.FullName)
            .Take(20)
            .Select(x => new VendorDetailsDirectoryItemModel
            {
                FullName = x.FullName,
                Rank = x.Rank,
                PositionTitle = x.PositionTitle,
                Phone = x.Phone,
                MilitaryCode = x.MilitaryCode,
                UnitName = x.UnitName,
                Published = x.Published
            })
            .ToListAsync();

        return model;
    }

    protected virtual async Task<IList<VendorDetailsStaffItemModel>> PrepareVendorDetailsStaffAsync(IList<Customer> staff, int? pmCustomerId)
    {
        var result = new List<VendorDetailsStaffItemModel>();

        foreach (var customer in staff)
        {
            var profile = await _customerMilitaryProfileService.GetByCustomerIdAsync(customer.Id);
            var roles = await _customerService.GetCustomerRolesAsync(customer, showHidden: true);

            result.Add(new VendorDetailsStaffItemModel
            {
                CustomerId = customer.Id,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Rank = profile?.Rank ?? string.Empty,
                PositionTitle = profile?.PositionTitle ?? string.Empty,
                Phone = customer.Phone ?? string.Empty,
                Email = customer.Email ?? string.Empty,
                RoleNames = string.Join(", ", roles.Where(role => role.Active).Select(GetRoleDisplayName)),
                IsLeader = pmCustomerId.HasValue && customer.Id == pmCustomerId.Value
            });
        }

        return result;
    }

    protected virtual async Task<IList<PortalDocumentRow>> GetPortalDocumentsAsync(IList<int> vendorIds, bool includeAllVendors = false)
    {
        if (!includeAllVendors && !vendorIds.Any())
            return new List<PortalDocumentRow>();

        var idList = string.Join(",", vendorIds.Select(id => Math.Max(0, id)));
        try
        {
            var whereClause = includeAllVendors ? "Deleted = 0" : $"Deleted = 0 AND OwnerVendorId IN ({idList})";
            return await _dataProvider.QueryAsync<PortalDocumentRow>($@"
                SELECT Id, Title, Code, CreatedOnUtc, OwnerVendorId
                FROM [Document]
                WHERE {whereClause}
                ORDER BY CreatedOnUtc DESC");
        }
        catch
        {
            return new List<PortalDocumentRow>();
        }
    }

    protected virtual async Task<IList<UnitReportRow>> GetUnitReportsAsync(IList<int> vendorIds, bool includeAllVendors = false)
    {
        if (!includeAllVendors && !vendorIds.Any())
            return new List<UnitReportRow>();

        var idList = string.Join(",", vendorIds.Select(id => Math.Max(0, id)));
        try
        {
            var whereClause = includeAllVendors ? "1 = 1" : $"VendorId IN ({idList})";
            return await _dataProvider.QueryAsync<UnitReportRow>($@"
                SELECT Id, ReportPeriodId, VendorId, StatusId, SubmittedOnUtc, LockedOnUtc
                FROM [LR_UnitReport]
                WHERE {whereClause}
                ORDER BY COALESCE(SubmittedOnUtc, LockedOnUtc) DESC");
        }
        catch
        {
            return new List<UnitReportRow>();
        }
    }

    protected virtual VendorDetailsListItemModel ToBlogDetailsListItem(BlogPost blogPost, IDictionary<int, string> vendorsById)
    {
        return new VendorDetailsListItemModel
        {
            Title = blogPost.Title,
            Subtitle = GetVendorName(vendorsById, blogPost.VendorId),
            CreatedOnUtc = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc,
            Url = Url.Action("Edit", "Blog", new { id = blogPost.Id }) ?? string.Empty
        };
    }

    protected virtual IList<(DateTime Month, string Label)> BuildMonthLabels()
    {
        var firstMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-5);
        return Enumerable.Range(0, 6)
            .Select(index => firstMonth.AddMonths(index))
            .Select(month => (month, month.ToString("MM/yyyy")))
            .ToList();
    }

    protected virtual IList<int> CountByMonth(IEnumerable<DateTime> dates, IList<(DateTime Month, string Label)> labels)
    {
        var dateList = dates.ToList();
        return labels
            .Select(label => dateList.Count(date => date.Year == label.Month.Year && date.Month == label.Month.Month))
            .ToList();
    }

    protected virtual string GetVendorName(IDictionary<int, string> vendorsById, int vendorId)
        => vendorId > 0 && vendorsById.TryGetValue(vendorId, out var name) ? name : "Hệ thống";

    protected virtual string GetReportStatusName(int statusId)
    {
        return statusId switch
        {
            10 => "Nháp",
            20 => "Đã nhập",
            30 => "Đã nộp",
            40 => "Trả lại",
            50 => "Đã khóa",
            _ => "Không rõ"
        };
    }

    protected class PortalDocumentRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int? OwnerVendorId { get; set; }
    }

    protected class UnitReportRow
    {
        public int Id { get; set; }
        public int ReportPeriodId { get; set; }
        public int VendorId { get; set; }
        public int StatusId { get; set; }
        public DateTime? SubmittedOnUtc { get; set; }
        public DateTime? LockedOnUtc { get; set; }
    }

    protected class VendorStaffExportRow
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Rank { get; set; }
        public string PositionTitle { get; set; }
        public string RoleNames { get; set; }
    }

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> Dashboard()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var model = await PrepareVendorDetailsModelAsync(null, currentCustomer, includeAllVendors: true);
        return View("Details", model);
    }

    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> Details(int id)
    {
        var vendor = await GetAccessibleVendorAsync(id);
        if (vendor == null)
            return AccessDeniedView();

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var model = await PrepareVendorDetailsModelAsync(vendor, currentCustomer);
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> List(int searchRootVendorId = 0)
    {
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorSearchModelAsync(new VendorSearchModel { SearchRootVendorId = searchRootVendorId });

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> List(VendorSearchModel searchModel)
    {
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _vendorModelFactory.PrepareVendorModelAsync(new VendorModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(VendorModel model, bool continueEditing, IFormCollection form)
    {
        //parse vendor attributes
        var vendorAttributesXml = await ParseVendorAttributesAsync(form);
        var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
        foreach (var warning in warnings)
            ModelState.AddModelError(string.Empty, warning);

        await ValidateParentVendorAsync(0, model.ParentId);
        await ValidateVendorManagerAssignmentAsync(model.PmCustomerId, null);

        if (ModelState.IsValid)
        {
            await NormalizeVendorAddressAsync(model.Address);

            var vendor = model.ToEntity<Vendor>();
            await _vendorService.InsertVendorAsync(vendor);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewVendor",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewVendor"), vendor.Id), vendor);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
            await _urlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

            //address
            var address = model.Address.ToEntity<Address>();
            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;
            await _addressService.InsertAddressAsync(address);
            vendor.AddressId = address.Id;
            await _vendorService.UpdateVendorAsync(vendor);
            await RecalculateVendorHierarchyAsync(vendor);

            //vendor attributes
            await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

            //locales
            await UpdateLocalesAsync(vendor, model);

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(vendor);

            //grant Vendors role to the unit manager
            await UpdateVendorManagerRoleAsync(null, vendor.PmCustomerId, vendor.Id);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = vendor.Id });
        }

        //prepare model
        model = await _vendorModelFactory.PrepareVendorModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(id);
        if (vendor == null || vendor.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _vendorModelFactory.PrepareVendorModelAsync(null, vendor);

        if (!_forumSettings.AllowPrivateMessages && model.PmCustomerId > 0)
            _notificationService.WarningNotification("Private messages are disabled. Do not forget to enable them.");

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(VendorModel model, bool continueEditing, IFormCollection form)
    {
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(model.Id);
        if (vendor == null || vendor.Deleted)
            return RedirectToAction("List");

        //parse vendor attributes
        var vendorAttributesXml = await ParseVendorAttributesAsync(form);
        var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
        foreach (var warning in warnings)
            ModelState.AddModelError(string.Empty, warning);

        //custom address attributes
        var customAttributes = await _addressAttributeParser.ParseCustomAttributesAsync(form, NopCommonDefaults.AddressAttributeControlName);
        var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
        foreach (var error in customAttributeWarnings)
            ModelState.AddModelError(string.Empty, error);

        await ValidateParentVendorAsync(model.Id, model.ParentId);
        await ValidateVendorManagerAssignmentAsync(model.PmCustomerId, model.Id);

        if (ModelState.IsValid)
        {
            await NormalizeVendorAddressAsync(model.Address);

            var prevPictureId = vendor.PictureId;
            var oldPmCustomerId = vendor.PmCustomerId;
            vendor = model.ToEntity(vendor);
            await _vendorService.UpdateVendorAsync(vendor);
            await RecalculateVendorHierarchyAsync(vendor);

            //vendor attributes
            await _genericAttributeService.SaveAttributeAsync(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditVendor",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditVendor"), vendor.Id), vendor);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(vendor, model.SeName, vendor.Name, true);
            await _urlRecordService.SaveSlugAsync(vendor, model.SeName, 0);

            //address
            var address = await _addressService.GetAddressByIdAsync(vendor.AddressId);
            if (address == null)
            {
                address = model.Address.ToEntity<Address>();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.InsertAddressAsync(address);
                vendor.AddressId = address.Id;
                await _vendorService.UpdateVendorAsync(vendor);
            }
            else
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.UpdateAddressAsync(address);
            }

            //locales
            await UpdateLocalesAsync(vendor, model);

            //delete an old picture (if deleted or updated)
            if (prevPictureId > 0 && prevPictureId != vendor.PictureId)
            {
                var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                if (prevPicture != null)
                    await _pictureService.DeletePictureAsync(prevPicture);
            }
            //update picture seo file name
            await UpdatePictureSeoNamesAsync(vendor);

            //sync Vendors role with unit manager assignment
            await UpdateVendorManagerRoleAsync(oldPmCustomerId, vendor.PmCustomerId, vendor.Id);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = vendor.Id });
        }

        //prepare model
        model = await _vendorModelFactory.PrepareVendorModelAsync(model, vendor, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(id);
        if (vendor == null)
            return RedirectToAction("List");

        var oldPmCustomerId = vendor.PmCustomerId;

        //clear associated customer references
        var associatedCustomers = await _customerService.GetAllCustomersAsync(vendorId: vendor.Id);
        foreach (var customer in associatedCustomers)
        {
            customer.VendorId = 0;
            await _customerService.UpdateCustomerAsync(customer);
        }

        //delete a vendor
        await _vendorService.DeleteVendorAsync(vendor);

        //revoke Vendors role from former manager if they no longer manage any vendor
        await UpdateVendorManagerRoleAsync(oldPmCustomerId, null, null);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteVendor",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteVendor"), vendor.Id), vendor);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Vendors.Deleted"));

        return RedirectToAction("List");
    }

    protected virtual async Task RecalculateVendorHierarchyAsync(Vendor vendor)
    {
        var allVendors = await _vendorService.GetAllVendorsAsync(showHidden: true, pageSize: int.MaxValue);
        var byId = allVendors.ToDictionary(v => v.Id, v => v);

        if (!byId.TryGetValue(vendor.Id, out var currentVendor))
            return;

        if (WouldCreateHierarchyCycle(currentVendor.Id, currentVendor.ParentId, byId))
            throw new InvalidOperationException("Cấu trúc cây đơn vị không hợp lệ.");

        var parent = currentVendor.ParentId.HasValue && currentVendor.ParentId.Value > 0 && byId.TryGetValue(currentVendor.ParentId.Value, out var parentVendor)
            ? parentVendor
            : null;

        var currentCode = string.IsNullOrWhiteSpace(currentVendor.Code) ? $"V-{currentVendor.Id}" : currentVendor.Code.Trim();
        var currentPath = parent == null ? $"/{currentCode}" : $"{parent.Path}/{currentCode}";
        var currentLevel = parent == null ? 0 : parent.Level + 1;

        if (currentVendor.Code != currentCode || currentVendor.Path != currentPath || currentVendor.Level != currentLevel)
        {
            currentVendor.Code = currentCode;
            currentVendor.Path = currentPath;
            currentVendor.Level = currentLevel;
            await _vendorService.UpdateVendorAsync(currentVendor);
        }

        var descendants = allVendors
            .Where(v => v.Id != currentVendor.Id)
            .ToDictionary(v => v.Id, v => v);

        var queue = new Queue<Vendor>();
        queue.Enqueue(currentVendor);

        while (queue.Count > 0)
        {
            var parentNode = queue.Dequeue();
            var children = descendants.Values.Where(v => v.ParentId == parentNode.Id).ToList();

            foreach (var child in children)
            {
                var childCode = string.IsNullOrWhiteSpace(child.Code) ? $"V-{child.Id}" : child.Code.Trim();
                var childPath = $"{parentNode.Path}/{childCode}";
                var childLevel = parentNode.Level + 1;

                if (child.Code != childCode || child.Path != childPath || child.Level != childLevel)
                {
                    child.Code = childCode;
                    child.Path = childPath;
                    child.Level = childLevel;
                    await _vendorService.UpdateVendorAsync(child);
                }

                queue.Enqueue(child);
            }
        }
    }

    protected virtual async Task UpdateVendorManagerRoleAsync(int? oldPmCustomerId, int? newPmCustomerId, int? vendorId)
    {
        var vendorsRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole == null)
            return;

        // Grant Vendors role to new manager
        if (newPmCustomerId > 0)
        {
            var newManager = await _customerService.GetCustomerByIdAsync(newPmCustomerId.Value);
            if (newManager != null)
            {
                if (vendorId.HasValue && vendorId > 0 && newManager.VendorId != vendorId.Value)
                {
                    newManager.VendorId = vendorId.Value;
                    await _customerService.UpdateCustomerAsync(newManager);
                }

                if (!await _customerService.IsInCustomerRoleAsync(newManager, NopCustomerDefaults.VendorsRoleName))
                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = newManager.Id, CustomerRoleId = vendorsRole.Id });
            }
        }

        // Revoke Vendors role from old manager if they no longer manage any vendor
        if (oldPmCustomerId > 0 && oldPmCustomerId != newPmCustomerId)
        {
            if (!await _vendorRepository.Table.AnyAsync(v => v.PmCustomerId == oldPmCustomerId))
            {
                var oldManager = await _customerService.GetCustomerByIdAsync(oldPmCustomerId.Value);
                if (oldManager != null)
                    await _customerService.RemoveCustomerRoleMappingAsync(oldManager, vendorsRole);
            }
        }
    }

    #endregion

    #region Vendor notes

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_VIEW)]
    public virtual async Task<IActionResult> VendorNotesSelect(VendorNoteSearchModel searchModel)
    {
        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(searchModel.VendorId)
            ?? throw new ArgumentException("No vendor found with the specified id");

        //prepare model
        var model = await _vendorModelFactory.PrepareVendorNoteListModelAsync(searchModel, vendor);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VendorNoteAdd(int vendorId, string message)
    {
        if (string.IsNullOrEmpty(message))
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.Vendors.VendorNotes.Fields.Note.Validation"));

        //try to get a vendor with the specified id
        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
        if (vendor == null)
            return ErrorJson("Vendor cannot be loaded");

        await _vendorService.InsertVendorNoteAsync(new VendorNote
        {
            Note = message,
            CreatedOnUtc = DateTime.UtcNow,
            VendorId = vendor.Id
        });

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> VendorNoteDelete(int id)
    {
        //try to get a vendor note with the specified id
        var vendorNote = await _vendorService.GetVendorNoteByIdAsync(id)
            ?? throw new ArgumentException("No vendor note found with the specified id", nameof(id));

        await _vendorService.DeleteVendorNoteAsync(vendorNote);

        return new NullJsonResult();
    }

    #endregion
}
