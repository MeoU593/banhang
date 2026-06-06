using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
public partial class UnitActivityLogController : BaseAdminController
{
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IWorkContext _workContext;

    public UnitActivityLogController(
        IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IRepository<Customer> customerRepository,
        IWorkContext workContext)
    {
        _baseAdminModelFactory = baseAdminModelFactory;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _customerRepository = customerRepository;
        _workContext = workContext;
    }

    public virtual async Task<IActionResult> List()
    {
        var vendor = await GetLeaderVendorAsync();
        if (vendor == null)
            return AccessDeniedView();

        var model = new UnitActivityLogSearchModel
        {
            VendorId = vendor.Id,
            VendorName = vendor.Name
        };
        await PrepareAllowedSystemActivityLogTypesAsync(model.ActivityLogType);
        model.SetGridPageSize();

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> ListLogs(UnitActivityLogSearchModel searchModel)
    {
        var vendor = await GetLeaderVendorAsync();
        if (vendor == null)
            return AccessDeniedView();

        var staffCustomerIds = await _customerRepository.Table
            .Where(customer => !customer.Deleted && customer.VendorId == vendor.Id)
            .Select(customer => customer.Id)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(searchModel.SearchCustomerEmail))
        {
            var matchedCustomers = await _customerService.GetAllCustomersAsync(email: searchModel.SearchCustomerEmail, pageSize: int.MaxValue);
            var matchedIds = matchedCustomers.Select(customer => customer.Id).ToHashSet();
            staffCustomerIds = staffCustomerIds.Where(matchedIds.Contains).ToList();
        }

        var customerIds = staffCustomerIds.Any() ? staffCustomerIds.ToArray() : new[] { -1 };
        var startDateValue = searchModel.CreatedOnFrom == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = searchModel.CreatedOnTo == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var allowedActivityLogTypeIds = (await GetAllowedSystemActivityLogTypeIdsAsync()).DefaultIfEmpty(-1).ToArray();
        var selectedActivityLogTypeId = allowedActivityLogTypeIds.Contains(searchModel.ActivityLogTypeId)
            ? searchModel.ActivityLogTypeId
            : 0;

        var activityLog = await _customerActivityService.GetAllActivitiesAsync(
            createdOnFrom: startDateValue,
            createdOnTo: endDateValue,
            activityLogTypeId: selectedActivityLogTypeId,
            customerIds: customerIds,
            activityLogTypeIds: allowedActivityLogTypeIds,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        var activityLogCustomers = await _customerService.GetCustomersByIdsAsync(activityLog.Select(log => log.CustomerId).Distinct().ToArray());
        var model = await new ActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
        {
            return activityLog.SelectAwait(async logItem =>
            {
                var logItemModel = logItem.ToModel<ActivityLogModel>();
                logItemModel.ActivityLogTypeName = (await _customerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;
                logItemModel.CustomerEmail = activityLogCustomers?.FirstOrDefault(customer => customer.Id == logItem.CustomerId)?.Email;
                logItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);
                return logItemModel;
            });
        });

        return Json(model);
    }

    protected virtual async Task<Nop.Core.Domain.Vendors.Vendor> GetLeaderVendorAsync()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor == null || currentVendor.PmCustomerId != currentCustomer.Id)
            return null;

        return currentVendor;
    }

    protected virtual async Task<IList<int>> GetAllowedSystemActivityLogTypeIdsAsync()
    {
        var allowedKeywords = NopActivityLogDefaults.SystemActivityLogKeywords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var availableActivityTypes = await _customerActivityService.GetAllActivityTypesAsync();

        return availableActivityTypes
            .Where(activityType => activityType.Enabled && allowedKeywords.Contains(activityType.SystemKeyword))
            .Select(activityType => activityType.Id)
            .ToList();
    }

    protected virtual async Task PrepareAllowedSystemActivityLogTypesAsync(IList<SelectListItem> items)
    {
        items.Add(new SelectListItem { Value = "0", Text = "Tất cả" });

        var allowedKeywords = NopActivityLogDefaults.SystemActivityLogKeywords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var availableActivityTypes = await _customerActivityService.GetAllActivityTypesAsync();

        foreach (var activityType in availableActivityTypes
            .Where(activityType => activityType.Enabled && allowedKeywords.Contains(activityType.SystemKeyword))
            .OrderBy(activityType => Array.IndexOf(NopActivityLogDefaults.SystemActivityLogKeywords, activityType.SystemKeyword)))
        {
            items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
        }
    }
}
