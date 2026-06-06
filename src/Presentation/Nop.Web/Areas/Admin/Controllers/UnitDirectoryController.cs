using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
public partial class UnitDirectoryController : BaseAdminController
{
    private static readonly int[] _allowedPageSizes = [10, 20, 50, 100];

    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IRepository<UnitDirectoryEntry> _directoryRepository;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;

    public UnitDirectoryController(
        INotificationService notificationService,
        IPermissionService permissionService,
        IRepository<UnitDirectoryEntry> directoryRepository,
        IVendorService vendorService,
        IWorkContext workContext)
    {
        _notificationService = notificationService;
        _permissionService = permissionService;
        _directoryRepository = directoryRepository;
        _vendorService = vendorService;
        _workContext = workContext;
    }

    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> List(int vendorId = 0, string q = null, string rank = null, string positionTitle = null, int publishedId = 0, int page = 1, int pageSize = 20)
    {
        var access = await GetDirectoryAccessAsync(vendorId);
        if (!access.CanManage)
            return AccessDeniedView();

        pageSize = NormalizePageSize(pageSize);
        page = Math.Max(page, 1);

        var query = _directoryRepository.Table;
        if (access.SelectedVendorId > 0)
            query = query.Where(entry => entry.VendorId == access.SelectedVendorId);

        q = q?.Trim();
        rank = rank?.Trim();
        positionTitle = positionTitle?.Trim();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(entry => entry.FullName.Contains(q) || entry.Phone.Contains(q) || entry.MilitaryCode.Contains(q) || entry.UnitName.Contains(q));

        if (!string.IsNullOrWhiteSpace(rank))
            query = query.Where(entry => entry.Rank.Contains(rank));

        if (!string.IsNullOrWhiteSpace(positionTitle))
            query = query.Where(entry => entry.PositionTitle.Contains(positionTitle));

        if (publishedId == 1)
            query = query.Where(entry => entry.Published);
        else if (publishedId == 2)
            query = query.Where(entry => !entry.Published);

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        page = Math.Min(page, totalPages);
        var skip = (page - 1) * pageSize;

        var entries = await query
            .OrderBy(entry => entry.VendorId)
            .ThenBy(entry => entry.DisplayOrder)
            .ThenBy(entry => entry.FullName)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
        var vendorNames = vendors.ToDictionary(vendor => vendor.Id, vendor => vendor.Name);
        var model = new UnitDirectoryAdminModel
        {
            VendorId = access.SelectedVendorId,
            SelectedVendorId = access.SelectedVendorId,
            VendorName = access.SelectedVendorId > 0 && vendorNames.TryGetValue(access.SelectedVendorId, out var selectedVendorName) ? selectedVendorName : "Toàn hệ thống",
            IsSystemAdmin = access.IsSystemAdmin,
            CanSelectVendor = access.IsSystemAdmin,
            SearchKeyword = q ?? string.Empty,
            SearchRank = rank ?? string.Empty,
            SearchPositionTitle = positionTitle ?? string.Empty,
            SearchPublishedId = publishedId,
            AvailableVendors = PrepareVendorSelectList(vendors, access.SelectedVendorId, includeAllOption: true),
            Entry = new UnitDirectoryEntryModel { VendorId = access.SelectedVendorId, FilterVendorId = access.SelectedVendorId, Published = true, Page = page, PageSize = pageSize },
            Entries = entries.Select(entry => ToModel(entry, vendorNames, access.SelectedVendorId, page, pageSize)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            FirstItemIndex = totalItems == 0 ? 0 : skip + 1,
            LastItemIndex = totalItems == 0 ? 0 : skip + entries.Count,
            AvailablePageSizes = PreparePageSizeSelectList(pageSize)
        };

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> Add([Bind(Prefix = nameof(UnitDirectoryAdminModel.Entry))] UnitDirectoryEntryModel model)
    {
        var vendor = await GetManageableVendorAsync(model.VendorId);
        if (vendor == null)
        {
            _notificationService.ErrorNotification("Vui lòng chọn đơn vị để thêm nhân sự.");
            return RedirectToAction(nameof(List), new { vendorId = model.FilterVendorId });
        }

        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            _notificationService.ErrorNotification("Vui lòng nhập họ tên.");
            return RedirectToAction(nameof(List), new { vendorId = model.FilterVendorId > 0 ? model.FilterVendorId : vendor.Id });
        }

        var now = DateTime.UtcNow;
        await _directoryRepository.InsertAsync(new UnitDirectoryEntry
        {
            VendorId = vendor.Id,
            FullName = model.FullName.Trim(),
            Rank = model.Rank?.Trim(),
            PositionTitle = model.PositionTitle?.Trim(),
            Phone = model.Phone?.Trim(),
            MilitaryCode = model.MilitaryCode?.Trim(),
            UnitName = string.IsNullOrWhiteSpace(model.UnitName) ? vendor.Name : model.UnitName.Trim(),
            Biography = model.Biography?.Trim(),
            DisplayOrder = 0,
            Published = model.Published,
            CreatedOnUtc = now,
            UpdatedOnUtc = now
        });

        _notificationService.SuccessNotification("Đã thêm nhân sự.");
        return RedirectToAction(nameof(List), new { vendorId = model.FilterVendorId > 0 ? model.FilterVendorId : vendor.Id });
    }

    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> Details(int id, int vendorId = 0, int page = 1, int pageSize = 20)
    {
        var entry = await _directoryRepository.GetByIdAsync(id);
        if (entry == null)
            return RedirectToAction(nameof(List), new { vendorId, page, pageSize });

        if (!await CanManageEntryAsync(entry))
            return AccessDeniedView();

        var access = await GetDirectoryAccessAsync(vendorId);
        if (!access.CanManage)
            return AccessDeniedView();

        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
        var vendorNames = vendors.ToDictionary(vendor => vendor.Id, vendor => vendor.Name);
        var filterVendorId = access.IsSystemAdmin ? vendorId : access.SelectedVendorId;

        var model = new UnitDirectoryAdminModel
        {
            VendorId = entry.VendorId,
            SelectedVendorId = filterVendorId,
            VendorName = vendorNames.TryGetValue(entry.VendorId, out var vendorName) ? vendorName : "Toàn hệ thống",
            IsSystemAdmin = access.IsSystemAdmin,
            CanSelectVendor = access.IsSystemAdmin,
            AvailableVendors = PrepareVendorSelectList(vendors, entry.VendorId, includeAllOption: false),
            Page = page,
            PageSize = NormalizePageSize(pageSize),
            AvailablePageSizes = PreparePageSizeSelectList(NormalizePageSize(pageSize)),
            Entry = ToModel(entry, vendorNames, filterVendorId, page, NormalizePageSize(pageSize))
        };

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> Edit(UnitDirectoryEntryModel model)
    {
        var entry = await _directoryRepository.GetByIdAsync(model.Id);
        if (entry == null)
            return RedirectToAction(nameof(List), new { vendorId = model.FilterVendorId });

        if (!await CanManageEntryAsync(entry))
            return AccessDeniedView();

        var targetVendor = await GetManageableVendorAsync(model.VendorId);
        if (targetVendor == null)
            return AccessDeniedView();

        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            _notificationService.ErrorNotification("Vui lòng nhập họ tên.");
            return RedirectToAction(nameof(Details), new { id = model.Id, vendorId = model.FilterVendorId, page = model.Page, pageSize = model.PageSize });
        }

        entry.VendorId = targetVendor.Id;
        entry.FullName = model.FullName.Trim();
        entry.Rank = model.Rank?.Trim();
        entry.PositionTitle = model.PositionTitle?.Trim();
        entry.Phone = model.Phone?.Trim();
        entry.MilitaryCode = model.MilitaryCode?.Trim();
        entry.UnitName = string.IsNullOrWhiteSpace(model.UnitName) ? targetVendor.Name : model.UnitName.Trim();
        entry.Biography = model.Biography?.Trim();
        entry.Published = model.Published;
        entry.UpdatedOnUtc = DateTime.UtcNow;
        await _directoryRepository.UpdateAsync(entry);

        _notificationService.SuccessNotification("Đã cập nhật nhân sự.");
        return RedirectToAction(nameof(Details), new { id = entry.Id, vendorId = model.FilterVendorId, page = model.Page, pageSize = model.PageSize });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> SetPublished(int id, bool published, int vendorId = 0, string q = null, string rank = null, string positionTitle = null, int publishedId = 0, int page = 1, int pageSize = 20)
    {
        var entry = await _directoryRepository.GetByIdAsync(id);
        if (entry == null)
            return RedirectToAction(nameof(List), new { vendorId, q, rank, positionTitle, publishedId, page, pageSize });

        if (!await CanManageEntryAsync(entry))
            return AccessDeniedView();

        entry.Published = published;
        entry.UpdatedOnUtc = DateTime.UtcNow;
        await _directoryRepository.UpdateAsync(entry);

        _notificationService.SuccessNotification("Đã cập nhật trạng thái hiển thị.");
        return RedirectToAction(nameof(List), new { vendorId, q, rank, positionTitle, publishedId, page, pageSize });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> Delete(int id, int vendorId = 0, string q = null, string rank = null, string positionTitle = null, int publishedId = 0, int page = 1, int pageSize = 20)
    {
        var entry = await _directoryRepository.GetByIdAsync(id);
        if (entry != null)
        {
            if (!await CanManageEntryAsync(entry))
                return AccessDeniedView();

            await _directoryRepository.DeleteAsync(entry);
            _notificationService.SuccessNotification("Đã xóa nhân sự.");
        }

        return RedirectToAction(nameof(List), new { vendorId, q, rank, positionTitle, publishedId, page, pageSize });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> ImportExcel(int vendorId, IFormFile importFile)
    {
        var vendor = await GetManageableVendorAsync(vendorId);
        if (vendor == null)
        {
            _notificationService.ErrorNotification("Vui lòng chọn đơn vị để import nhân sự.");
            return RedirectToAction(nameof(List), new { vendorId });
        }

        if (importFile == null || importFile.Length == 0)
        {
            _notificationService.ErrorNotification("Vui lòng chọn file Excel.");
            return RedirectToAction(nameof(List), new { vendorId = vendor.Id });
        }

        var now = DateTime.UtcNow;
        var imported = 0;
        await using var stream = importFile.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var fullName = row.Cell(1).GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(fullName))
                continue;

            await _directoryRepository.InsertAsync(new UnitDirectoryEntry
            {
                VendorId = vendor.Id,
                FullName = fullName,
                Rank = row.Cell(2).GetString()?.Trim(),
                PositionTitle = row.Cell(3).GetString()?.Trim(),
                Phone = row.Cell(4).GetString()?.Trim(),
                MilitaryCode = row.Cell(5).GetString()?.Trim(),
                UnitName = string.IsNullOrWhiteSpace(row.Cell(6).GetString()) ? vendor.Name : row.Cell(6).GetString().Trim(),
                DisplayOrder = 0,
                Biography = row.Cell(7).GetString()?.Trim(),
                Published = true,
                CreatedOnUtc = now,
                UpdatedOnUtc = now
            });
            imported++;
        }

        _notificationService.SuccessNotification($"Đã import {imported} nhân sự.");
        return RedirectToAction(nameof(List), new { vendorId = vendor.Id });
    }

    [CheckPermission(StandardPermission.Security.ACCESS_ADMIN_PANEL)]
    public virtual async Task<IActionResult> ExportExcel(int vendorId = 0, string q = null, string rank = null, string positionTitle = null, int publishedId = 0)
    {
        var access = await GetDirectoryAccessAsync(vendorId);
        if (!access.CanManage)
            return AccessDeniedView();

        var query = _directoryRepository.Table;
        if (access.SelectedVendorId > 0)
            query = query.Where(entry => entry.VendorId == access.SelectedVendorId);

        q = q?.Trim();
        rank = rank?.Trim();
        positionTitle = positionTitle?.Trim();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(entry => entry.FullName.Contains(q) || entry.Phone.Contains(q) || entry.MilitaryCode.Contains(q) || entry.UnitName.Contains(q));

        if (!string.IsNullOrWhiteSpace(rank))
            query = query.Where(entry => entry.Rank.Contains(rank));

        if (!string.IsNullOrWhiteSpace(positionTitle))
            query = query.Where(entry => entry.PositionTitle.Contains(positionTitle));

        if (publishedId == 1)
            query = query.Where(entry => entry.Published);
        else if (publishedId == 2)
            query = query.Where(entry => !entry.Published);

        var entries = await query
            .OrderBy(entry => entry.VendorId)
            .ThenBy(entry => entry.DisplayOrder)
            .ThenBy(entry => entry.FullName)
            .ToListAsync();

        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
        var vendorNames = vendors.ToDictionary(vendor => vendor.Id, vendor => vendor.Name);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Nhan su");
        var headers = new[]
        {
            "Đơn vị quản lý",
            "Họ tên",
            "Quân hàm",
            "Chức vụ",
            "Số điện thoại",
            "Số hiệu quân nhân",
            "Lý lịch"
        };

        for (var i = 0; i < headers.Length; i++)
            worksheet.Cell(1, i + 1).Value = headers[i];

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = vendorNames.TryGetValue(entry.VendorId, out var vendorName) ? vendorName : string.Empty;
            worksheet.Cell(row, 2).Value = entry.FullName;
            worksheet.Cell(row, 3).Value = entry.Rank;
            worksheet.Cell(row, 4).Value = entry.PositionTitle;
            worksheet.Cell(row, 5).Value = entry.Phone;
            worksheet.Cell(row, 6).Value = entry.MilitaryCode;
            worksheet.Cell(row, 7).Value = entry.Biography;
        }

        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Columns().AdjustToContents();

        await using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"nhan-su-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    protected virtual async Task<DirectoryAccess> GetDirectoryAccessAsync(int selectedVendorId)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var isSystemAdmin = currentVendor == null && await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, currentCustomer);
        if (isSystemAdmin)
            return new DirectoryAccess(true, true, selectedVendorId);

        if (currentVendor != null && currentVendor.PmCustomerId == currentCustomer.Id)
            return new DirectoryAccess(true, false, currentVendor.Id);

        return new DirectoryAccess(false, false, 0);
    }

    protected virtual async Task<Vendor> GetManageableVendorAsync(int vendorId)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            return currentVendor.PmCustomerId == currentCustomer.Id ? currentVendor : null;

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, currentCustomer))
            return null;

        if (vendorId <= 0)
            return null;

        var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
        return vendor == null || vendor.Deleted ? null : vendor;
    }

    protected virtual async Task<bool> CanManageEntryAsync(UnitDirectoryEntry entry)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            return currentVendor.PmCustomerId == currentCustomer.Id && entry.VendorId == currentVendor.Id;

        return await _permissionService.AuthorizeAsync(StandardPermission.Customers.VENDORS_CREATE_EDIT_DELETE, currentCustomer);
    }

    protected virtual IList<SelectListItem> PrepareVendorSelectList(IList<Vendor> vendors, int selectedVendorId, bool includeAllOption)
    {
        var items = new List<SelectListItem>();
        if (includeAllOption)
            items.Add(new SelectListItem { Text = "Tất cả đơn vị", Value = "0", Selected = selectedVendorId <= 0 });

        items.AddRange(vendors
            .Where(vendor => !vendor.Deleted)
            .OrderBy(vendor => vendor.Path ?? vendor.Name)
            .ThenBy(vendor => vendor.DisplayOrder)
            .ThenBy(vendor => vendor.Name)
            .Select(vendor => new SelectListItem
            {
                Text = vendor.Name,
                Value = vendor.Id.ToString(),
                Selected = vendor.Id == selectedVendorId
            }));

        return items;
    }

    protected virtual IList<SelectListItem> PreparePageSizeSelectList(int selectedPageSize)
    {
        return _allowedPageSizes
            .Select(size => new SelectListItem
            {
                Text = size.ToString(),
                Value = size.ToString(),
                Selected = size == selectedPageSize
            })
            .ToList();
    }

    protected virtual int NormalizePageSize(int pageSize)
    {
        return _allowedPageSizes.Contains(pageSize) ? pageSize : 20;
    }

    protected virtual UnitDirectoryEntryModel ToModel(UnitDirectoryEntry entry, IDictionary<int, string> vendorNames, int filterVendorId, int page = 1, int pageSize = 20)
    {
        return new UnitDirectoryEntryModel
        {
            Id = entry.Id,
            VendorId = entry.VendorId,
            FilterVendorId = filterVendorId,
            VendorName = vendorNames.TryGetValue(entry.VendorId, out var vendorName) ? vendorName : string.Empty,
            FullName = entry.FullName,
            Rank = entry.Rank,
            PositionTitle = entry.PositionTitle,
            Phone = entry.Phone,
            MilitaryCode = entry.MilitaryCode,
            UnitName = entry.UnitName,
            Biography = entry.Biography,
            DisplayOrder = entry.DisplayOrder,
            Published = entry.Published,
            Page = page,
            PageSize = pageSize
        };
    }

    protected readonly record struct DirectoryAccess(bool CanManage, bool IsSystemAdmin, int SelectedVendorId);
}
