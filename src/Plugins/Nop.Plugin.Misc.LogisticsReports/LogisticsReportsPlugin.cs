using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.LogisticsReports;

public class LogisticsReportsPlugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;
    private readonly IPermissionService _permissionService;

    public LogisticsReportsPlugin(ILocalizationService localizationService, IWebHelper webHelper, IPermissionService permissionService)
    {
        _localizationService = localizationService;
        _webHelper = webHelper;
        _permissionService = permissionService;
    }

    public override string GetConfigurationPageUrl()
        => $"{_webHelper.GetStoreLocation()}Admin/UnitReportAdmin/Index";

    public override async Task InstallAsync()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.LogisticsReports.Menu.Root"] = "Báo cáo Logistics",
            ["Plugins.Misc.LogisticsReports.Menu.Dashboard"] = "Bảng điều khiển",
            ["Plugins.Misc.LogisticsReports.Menu.CustomerOrganizations"] = "Người dùng - Đơn vị",
            ["Plugins.Misc.LogisticsReports.Menu.Organizations"] = "Đơn vị",
            ["Plugins.Misc.LogisticsReports.Menu.Templates"] = "Mẫu báo cáo",
            ["Plugins.Misc.LogisticsReports.Menu.Periods"] = "Kỳ báo cáo",
            ["Plugins.Misc.LogisticsReports.Menu.UnitReports"] = "Báo cáo đơn vị",
            ["Plugins.Misc.LogisticsReports.Common.Back"] = "Quay lại",
            ["Plugins.Misc.LogisticsReports.Common.Save"] = "Lưu",
            ["Plugins.Misc.LogisticsReports.Common.Edit"] = "Sửa",
            ["Plugins.Misc.LogisticsReports.Common.Create"] = "Tạo mới",
            ["Plugins.Misc.LogisticsReports.Common.NoData"] = "Không có dữ liệu",
            ["Plugins.Misc.LogisticsReports.Validation.ReturnReasonRequired"] = "Vui lòng nhập lý do trả lại.",
            ["Plugins.Misc.LogisticsReports.Notifications.ImportCompleted"] = "Import thành công {0} dòng dữ liệu.",
            ["Plugins.Misc.LogisticsReports.Notifications.UnitReportsGenerated"] = "Đã tạo {0} báo cáo đơn vị.",
            ["Plugins.Misc.LogisticsReports.Dashboard.Title"] = "Bảng điều khiển báo cáo logistics",
            ["Plugins.Misc.LogisticsReports.Dashboard.TotalOrganizations"] = "Đơn vị",
            ["Plugins.Misc.LogisticsReports.Dashboard.TotalTemplates"] = "Mẫu báo cáo",
            ["Plugins.Misc.LogisticsReports.Dashboard.TotalPeriods"] = "Kỳ báo cáo",
            ["Plugins.Misc.LogisticsReports.Dashboard.TotalUnitReports"] = "Báo cáo đơn vị",
            ["Plugins.Misc.LogisticsReports.Dashboard.Submitted"] = "Đã nộp",
            ["Plugins.Misc.LogisticsReports.Dashboard.Returned"] = "Bị trả lại",
            ["Plugins.Misc.LogisticsReports.Dashboard.Locked"] = "Đã khóa"
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var systems = new[]
        {
            Infrastructure.LogisticsReportsPermissionConfigManager.ManageOrganizationUnits,
            Infrastructure.LogisticsReportsPermissionConfigManager.ManageReportTemplates,
            Infrastructure.LogisticsReportsPermissionConfigManager.ManageReportPeriods,
            Infrastructure.LogisticsReportsPermissionConfigManager.InputOwnUnitReports,
            Infrastructure.LogisticsReportsPermissionConfigManager.ViewOwnUnitReports,
            Infrastructure.LogisticsReportsPermissionConfigManager.ViewChildUnitReports,
            Infrastructure.LogisticsReportsPermissionConfigManager.SubmitOwnUnitReports,
            Infrastructure.LogisticsReportsPermissionConfigManager.ReturnChildUnitReports,
            Infrastructure.LogisticsReportsPermissionConfigManager.LockReportPeriods,
            Infrastructure.LogisticsReportsPermissionConfigManager.ViewSystemWideReports
        };

        var all = await _permissionService.GetAllPermissionRecordsAsync();
        foreach (var permission in all.Where(x => systems.Contains(x.SystemName)))
            await _permissionService.DeletePermissionRecordAsync(permission);

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.LogisticsReports");

        await base.UninstallAsync();
    }
}
