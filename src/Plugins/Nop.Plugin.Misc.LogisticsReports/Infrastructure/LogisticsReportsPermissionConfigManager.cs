using Nop.Core.Domain.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.LogisticsReports.Infrastructure;

public class LogisticsReportsPermissionConfigManager : IPermissionConfigManager
{
    public const string ManageOrganizationUnits = nameof(ManageOrganizationUnits);
    public const string ManageReportTemplates = nameof(ManageReportTemplates);
    public const string ManageReportPeriods = nameof(ManageReportPeriods);
    public const string InputOwnUnitReports = nameof(InputOwnUnitReports);
    public const string ViewOwnUnitReports = nameof(ViewOwnUnitReports);
    public const string ViewChildUnitReports = nameof(ViewChildUnitReports);
    public const string SubmitOwnUnitReports = nameof(SubmitOwnUnitReports);
    public const string ReturnChildUnitReports = nameof(ReturnChildUnitReports);
    public const string LockReportPeriods = nameof(LockReportPeriods);
    public const string ViewSystemWideReports = nameof(ViewSystemWideReports);

    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        new("Admin area. Logistics reports. Manage organization units", ManageOrganizationUnits, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. Manage report templates", ManageReportTemplates, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. Manage report periods", ManageReportPeriods, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. Input own unit reports", InputOwnUnitReports, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. View own unit reports", ViewOwnUnitReports, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. View child unit reports", ViewChildUnitReports, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. Submit own unit reports", SubmitOwnUnitReports, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. Return child unit reports", ReturnChildUnitReports, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. Lock report periods", LockReportPeriods, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new("Admin area. Logistics reports. View system wide reports", ViewSystemWideReports, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName)
    };
}
