using Nop.Core;
using Nop.Plugin.Misc.DocumentPortal.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.DocumentPortal;

public class DocumentPortalPlugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;

    public DocumentPortalPlugin(
        ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/DocumentPortalAdmin/Settings";
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new DocumentPortalSettings());

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.DocumentPortal.Menu.Root"] = "Kho Tài liệu",
            ["Plugins.Misc.DocumentPortal.Menu.Documents"] = "Danh sách tài liệu",
            ["Plugins.Misc.DocumentPortal.Menu.Settings"] = "Cài đặt",
            ["Plugins.Misc.DocumentPortal.Menu.VendorDocuments"] = "Tài liệu đơn vị",

            ["Plugins.Misc.DocumentPortal.Document.Fields.Title"] = "Tiêu đề",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Title.Required"] = "Tiêu đề là bắt buộc.",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Slug"] = "Slug (URL)",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Code"] = "Mã tài liệu",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Summary"] = "Tóm tắt",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Content"] = "Nội dung",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Keywords"] = "Từ khóa",
            ["Plugins.Misc.DocumentPortal.Document.Fields.DocumentCategory"] = "Danh mục",
            ["Plugins.Misc.DocumentPortal.Document.Fields.DocumentType"] = "Loại tài liệu",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Issuer"] = "Cơ quan ban hành",
            ["Plugins.Misc.DocumentPortal.Document.Fields.IssuedDate"] = "Ngày ban hành",
            ["Plugins.Misc.DocumentPortal.Document.Fields.EffectiveDate"] = "Ngày có hiệu lực",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Download"] = "ID file tải",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Thumbnail"] = "ID ảnh đại diện",
            ["Plugins.Misc.DocumentPortal.Document.Fields.Published"] = "Xuất bản",
            ["Plugins.Misc.DocumentPortal.Document.Fields.AllowDownload"] = "Cho phép tải",
            ["Plugins.Misc.DocumentPortal.Document.Fields.ShowOnHomepage"] = "Hiện trên trang chủ",
            ["Plugins.Misc.DocumentPortal.Document.Fields.DisplayOrder"] = "Thứ tự hiển thị",

            ["Plugins.Misc.DocumentPortal.Document.List.SearchTitle"] = "Tiêu đề",
            ["Plugins.Misc.DocumentPortal.Document.List.SearchCode"] = "Mã tài liệu",
            ["Plugins.Misc.DocumentPortal.Document.List.SearchCategory"] = "Danh mục",
            ["Plugins.Misc.DocumentPortal.Document.List.SearchType"] = "Loại",
            ["Plugins.Misc.DocumentPortal.Document.List.SearchIssuer"] = "Cơ quan ban hành",
            ["Plugins.Misc.DocumentPortal.Document.List.SearchPublished"] = "Trạng thái",

            ["Plugins.Misc.DocumentPortal.Settings.Fields.DefaultPageSize"] = "Số tài liệu mỗi trang",
            ["Plugins.Misc.DocumentPortal.Settings.Fields.SearchInContent"] = "Tìm kiếm trong nội dung",
            ["Plugins.Misc.DocumentPortal.Settings.Fields.ShowRelatedDocuments"] = "Hiện tài liệu liên quan",
            ["Plugins.Misc.DocumentPortal.Settings.Fields.ShowDownloadCount"] = "Hiện số lượt tải",
            ["Plugins.Misc.DocumentPortal.Settings.Fields.AutoGenerateSlug"] = "Tự tạo slug",
            ["Plugins.Misc.DocumentPortal.Settings.Fields.RequireLoginForPrivateDocuments"] = "Yêu cầu đăng nhập để xem tài liệu hạn chế",
        });

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<DocumentPortalSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.DocumentPortal");
        await base.UninstallAsync();
    }
}
