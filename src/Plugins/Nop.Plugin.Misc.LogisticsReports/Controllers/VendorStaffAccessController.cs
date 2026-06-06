using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Plugin.Misc.LogisticsReports.Models.VendorStaff;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.LogisticsReports.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
public class VendorStaffAccessController : Nop.Web.Areas.Admin.Controllers.BaseAdminController
{
    private readonly ICustomerService _customerService;
    private readonly ICustomerMilitaryProfileService _customerMilitaryProfileService;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IWorkContext _workContext;

    public VendorStaffAccessController(
        ICustomerService customerService,
        ICustomerMilitaryProfileService customerMilitaryProfileService,
        IRepository<Vendor> vendorRepository,
        IWorkContext workContext)
    {
        _customerService = customerService;
        _customerMilitaryProfileService = customerMilitaryProfileService;
        _vendorRepository = vendorRepository;
        _workContext = workContext;
    }

    public async Task<IActionResult> Index()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        var model = await BuildModelAsync(currentVendor.Id, currentVendor.PmCustomerId);
        return View("~/Plugins/Misc.LogisticsReports/Views/VendorStaffAccess/Index.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> ExportExcel()
    {
        var vendor = await GetAuthorizedVendorAsync();
        if (vendor == null)
            return AccessDeniedView();

        var staff = await BuildModelAsync(vendor.Id, vendor.PmCustomerId);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Danh sách nhân viên");
        var headers = new[] { "Tên", "Cấp bậc", "Chức vụ", "Số điện thoại", "Gmail", "Quyền hạn" };
        for (var i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
        }

        for (var row = 0; row < staff.Count; row++)
        {
            var item = staff[row];
            worksheet.Cell(row + 2, 1).Value = item.FullName;
            worksheet.Cell(row + 2, 2).Value = item.Rank;
            worksheet.Cell(row + 2, 3).Value = item.PositionTitle;
            worksheet.Cell(row + 2, 4).Value = item.Phone;
            worksheet.Cell(row + 2, 5).Value = item.Email;
            worksheet.Cell(row + 2, 6).Value = item.RoleNames;
        }

        worksheet.Columns().AdjustToContents();
        await using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"nhan-vien-don-vi-{vendor.Id}.xlsx");
    }

    [HttpPost]
    public async Task<IActionResult> ExportPdf()
    {
        var vendor = await GetAuthorizedVendorAsync();
        if (vendor == null)
            return AccessDeniedView();

        var staff = await BuildModelAsync(vendor.Id, vendor.PmCustomerId);
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
            table.SetWidths(new float[] { 22, 14, 18, 16, 18, 22 });
            foreach (var header in new[] { "Tên", "Cấp bậc", "Chức vụ", "Số điện thoại", "Gmail", "Quyền hạn" })
                table.AddCell(new PdfPCell(new Phrase(header, headerFont)) { BackgroundColor = new BaseColor(211, 211, 211), Padding = 5 });

            foreach (var item in staff)
            {
                table.AddCell(new Phrase(item.FullName, bodyFont));
                table.AddCell(new Phrase(item.Rank, bodyFont));
                table.AddCell(new Phrase(item.PositionTitle, bodyFont));
                table.AddCell(new Phrase(item.Phone, bodyFont));
                table.AddCell(new Phrase(item.Email, bodyFont));
                table.AddCell(new Phrase(item.RoleNames, bodyFont));
            }

            document.Add(table);
            document.Close();
        }

        return File(stream.ToArray(), "application/pdf", $"nhan-vien-don-vi-{vendor.Id}.pdf");
    }

    [HttpPost]
    public async Task<IActionResult> GrantAccess(int customerId)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        var target = await _customerService.GetCustomerByIdAsync(customerId);
        if (target == null || target.VendorId != currentVendor.Id || target.Id == currentCustomer.Id)
            return RedirectToAction(nameof(Index));

        if (await IsVendorLeaderAsync(target.Id, currentVendor.Id))
            return RedirectToAction(nameof(Index));

        var vendorsRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole != null && !await _customerService.IsInCustomerRoleAsync(target, NopCustomerDefaults.VendorsRoleName))
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = target.Id, CustomerRoleId = vendorsRole.Id });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RevokeAccess(int customerId)
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return AccessDeniedView();

        var target = await _customerService.GetCustomerByIdAsync(customerId);
        if (target == null || target.VendorId != currentVendor.Id || target.Id == currentCustomer.Id)
            return RedirectToAction(nameof(Index));

        if (await IsVendorLeaderAsync(target.Id))
            return RedirectToAction(nameof(Index));

        var vendorsRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole != null)
            await _customerService.RemoveCustomerRoleMappingAsync(target, vendorsRole);

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<VendorStaffAccessModel>> BuildModelAsync(int vendorId, int? pmCustomerId)
    {
        var staff = await _customerService.GetAllCustomersAsync(vendorId: vendorId);
        var model = new List<VendorStaffAccessModel>();

        foreach (var customer in staff)
        {
            var militaryProfile = await _customerMilitaryProfileService.GetByCustomerIdAsync(customer.Id);
            var roles = await _customerService.GetCustomerRolesAsync(customer, showHidden: true);
            model.Add(new VendorStaffAccessModel
            {
                CustomerId = customer.Id,
                Email = customer.Email,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Phone = customer.Phone ?? string.Empty,
                Rank = militaryProfile?.Rank ?? string.Empty,
                PositionTitle = militaryProfile?.PositionTitle ?? string.Empty,
                RoleNames = string.Join(", ", roles.Where(role => role.Active).Select(GetRoleDisplayName)),
                HasAccess = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.VendorsRoleName),
                IsLeader = customer.Id == pmCustomerId
            });
        }

        return model;
    }

    private async Task<Vendor?> GetAuthorizedVendorAsync()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return null;

        return currentVendor;
    }

    private static string GetRoleDisplayName(CustomerRole role)
    {
        if (role.SystemName == NopCustomerDefaults.AdministratorsRoleName)
            return "Quản trị hệ thống";

        if (role.SystemName == NopCustomerDefaults.VendorsRoleName)
            return "Quản trị đơn vị";

        return role.Name;
    }

    private async Task<bool> IsVendorLeaderAsync(int customerId, int? exceptVendorId = null)
    {
        return await _vendorRepository.Table.AnyAsync(v => v.PmCustomerId == customerId && (!exceptVendorId.HasValue || v.Id != exceptVendorId.Value));
    }
}
