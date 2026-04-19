using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common;

public partial class DutyMessageSettings : ISettings
{
    public string LoginTitle { get; set; } = "Cổng thông tin Hậu cần - Quân nhu";
    public string LoginSubtitle { get; set; } = "Bộ Tư lệnh Tăng Thiết Giáp";
    public string LoginDescription { get; set; } = "Quản lý kho bãi, danh mục vật tư và luồng văn bản nghiệp vụ trên một giao diện thống nhất, ưu tiên tốc độ xử lý và tính sẵn sàng.";
    public string Message { get; set; } = "Kỷ luật dữ liệu là nền của chỉ huy chính xác; hậu cần thông suốt là điều kiện bảo đảm chiến thắng.";
    public string Author { get; set; } = "Trung tâm kỹ thuật số hóa quân nhu - cụm điều hành số 02";
    public string SupportDepartment { get; set; } = "Phòng Kỹ thuật";
    public string SupportUnit { get; set; } = "BTL TTG";
    public string SupportContactName { get; set; } = string.Empty;
    public string SupportContactPosition { get; set; } = string.Empty;
    public string SupportPhone { get; set; } = string.Empty;
}
