# Nop.Plugin.Misc.LogisticsReports

Plugin MVP cho bài toán tổng hợp báo cáo hậu cần nhiều cấp trong nopCommerce 4.80.

## Có trong bản này
- Quản lý cây đơn vị
- Gắn user với đơn vị
- Mẫu báo cáo + chỉ tiêu + map ô Excel
- Kỳ báo cáo
- Báo cáo đơn vị
- Import Excel bằng ClosedXML
- Tổng hợp cấp trên từ các đơn vị con trực tiếp
- Workflow cơ bản: nháp / đã gửi / trả lại / khóa
- Menu admin + permission + migration

## Giả định kỹ thuật
- Nền tảng: nopCommerce 4.80 / .NET 9
- Lưu file mẫu/import ở `App_Data/LogisticsReports`
- Màn hình admin ở mức MVP, thiên về CRUD và luồng nghiệp vụ cốt lõi
- Chưa có dashboard nâng cao, chữ ký số, scheduler tự sinh kỳ, hay liên kết Vendor

## Cần rà soát khi tích hợp vào project thật
- Namespace/using theo source nopCommerce cụ thể của dự án anh
- Base class/controller/model factory nếu dự án đang có convention riêng
- Giao diện Razor theo theme admin đang dùng
- Quy tắc import Excel thực tế của từng biểu mẫu
- Permission map với role thực tế

## Cấu trúc chính
- `Domain`: entity
- `Data`: migration
- `Services`: business logic
- `Controllers`: admin controllers
- `Views`: Razor views
- `Infrastructure`: menu, startup, permissions

## Luồng MVP
1. Tạo cây đơn vị
2. Gắn user vào đơn vị
3. Tạo mẫu báo cáo + chỉ tiêu + map ô
4. Tạo kỳ báo cáo
5. Sinh UnitReport cho các đơn vị
6. Đơn vị tải mẫu, import Excel, sửa tay nếu cần
7. Gửi báo cáo lên trên
8. Cấp trên tổng hợp và gửi tiếp
9. BTL khóa kỳ


## Added in v3 scaffold
- CustomerOrganization mapping module (admin CRUD)
- Dashboard module
- CSV export endpoint for unit reports
- Service registrations for the added modules
