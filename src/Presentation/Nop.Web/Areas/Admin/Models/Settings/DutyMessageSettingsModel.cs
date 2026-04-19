using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Settings;

public record DutyMessageSettingsModel
{
    [Display(Name = "Tiêu đề trang đăng nhập")]
    public string LoginTitle { get; set; }

    [Display(Name = "Tiêu đề phụ (tên đơn vị)")]
    public string LoginSubtitle { get; set; }

    [Display(Name = "Mô tả hệ thống")]
    public string LoginDescription { get; set; }

    [Display(Name = "Nội dung thông điệp")]
    public string Message { get; set; }

    [Display(Name = "Nguồn / Tác giả")]
    public string Author { get; set; }

    [Display(Name = "Bộ phận hỗ trợ")]
    public string SupportDepartment { get; set; }

    [Display(Name = "Đơn vị")]
    public string SupportUnit { get; set; }

    [Display(Name = "Người liên hệ")]
    public string SupportContactName { get; set; }

    [Display(Name = "Chức vụ")]
    public string SupportContactPosition { get; set; }

    [Display(Name = "Số điện thoại")]
    public string SupportPhone { get; set; }
}
