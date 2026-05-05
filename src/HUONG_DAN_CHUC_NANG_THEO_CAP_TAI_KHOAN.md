# HƯỚNG DẪN TÍNH NĂNG - BẢNG SO SÁNH THEO CẤP TÀI KHOẢN

**Hệ Thống:** Nền Tảng Logistics Quân Sự (nopCommerce 5.00)  
**Phiên bản:** 2026-05-03

---

## 📊 BẢNG TỔNG QUÁT - SO SÁNH CÁC CẤP TÀI KHOẢN

### Bảng 1: Quyền Hạn Chính

| **Tính Năng / Quyền Hạn** | **👤 Người Dùng Thường** | **👥 Người Dùng Đơn Vị** | **👨‍💼 Trưởng Đơn Vị** | **🔐 Quản Trị Viên** |
|---|:-:|:-:|:-:|:-:|
| **CHỨC NĂNG CHUNG** | | | | |
| Xem sản phẩm công khai | ✅ | ✅ | ✅ | ✅ |
| Đánh giá & bình luận | ✅ | ✅ | ✅ | ✅ |
| Quản lý hồ sơ cá nhân | ✅ | ✅ | ✅ | ✅ |
| Xem tin tức | ✅ | ✅ | ✅ | ✅ |
| Blog & Diễn đàn | ✅ | ✅ | ✅ | ✅ |
| Tin nhắn riêng tư | ✅ | ✅ | ✅ | ✅ |
| **TÀI LIỆU & BÁO CÁO** | | | | |
| Xem tài liệu công khai | ❌ | ✅ | ✅ | ✅ |
| Nhập báo cáo đơn vị | ❌ | ✅ | ✅ | ✅ |
| Xem báo cáo riêng | ❌ | ✅ | ✅ | ✅ |
| Quản lý BC cấp dưới | ❌ | ❌ | ✅ | ✅ |
| Phê duyệt/trả lại BC | ❌ | ❌ | ✅ | ✅ |
| Xem BC toàn hệ thống | ❌ | ❌ | ❌/✅ | ✅ |
| **QUẢN LÝ NHÂN SỰ & TÀI LIỆU** | | | | |
| Quản lý nhân sự đơn vị | ❌ | ❌ | ✅ | ✅ |
| Tải tài liệu lên | ❌ | ❌ | ✅ | ✅ |
| Phân quyền tài liệu | ❌ | ❌ | ✅ | ✅ |
| **QUẢN LÝ HỆ THỐNG** | | | | |
| Quản lý sản phẩm | ❌ | ❌ | ❌ | ✅ |
| Quản lý khách hàng | ❌ | ❌ | ❌ | ✅ |
| Quản lý đơn vị | ❌ | ❌ | ❌ | ✅ |
| Quản lý nội dung | ❌ | ❌ | ❌ | ✅ |
| Quản lý cài đặt | ❌ | ❌ | ❌ | ✅ |
| Quản lý plugin | ❌ | ❌ | ❌ | ✅ |

---

## 📋 MÔ TẢ CHI TIẾT - 4 LOẠI TÀI KHOẢN

---

## 1️⃣ NGƯỜI DÙNG THƯỜNG (Regular User)

### Mô Tả
- Khách hàng công khai, không thuộc bất kỳ đơn vị logistics nào
- Chỉ xem các nội dung công khai
- Không được truy cập báo cáo hoặc tài liệu hạn chế

### Quyền Hạn Cấp Cao
- ✅ Xem sản phẩm/dịch vụ
- ✅ Quản lý hồ sơ cá nhân
- ✅ Bình luận & đánh giá
- ❌ KHÔNG thể xem báo cáo
- ❌ KHÔNG thể xem tài liệu hạn chế

### Tính Năng Chi Tiết

| **Mục** | **Chi Tiết** |
|---|---|
| **🏠 Trang Chủ** | • Xem sản phẩm công khai<br>• Xem slider quảng cáo<br>• Duyệt danh mục sản phẩm |
| **📦 Sản Phẩm** | • Tìm kiếm sản phẩm<br>• Lọc theo danh mục<br>• Xem chi tiết sản phẩm<br>• Xem hình ảnh, video<br>• Đọc đánh giá |
| **⭐ Đánh Giá** | • Xem đánh giá sản phẩm<br>• Gửi đánh giá mới<br>• Bình luận |
| **❤️ Wishlist** | • Thêm sản phẩm yêu thích<br>• Xem danh sách yêu thích<br>• Xóa khỏi wishlist |
| **📰 Tin Tức** | • Xem tin tức công khai<br>• Lọc theo loại tin<br>• Xem chi tiết tin tức |
| **📝 Blog** | • Xem bài blog<br>• Bình luận bài blog<br>• Xem các bài liên quan |
| **💬 Diễn Đàn** | • Xem các chủ đề<br>• Đọc bài viết<br>• Trả lời bài viết |
| **👤 Hồ Sơ** | • Cập nhật tên, email<br>• Đổi mật khẩu<br>• Thêm địa chỉ<br>• Bật 2FA |
| **✉️ Tin Nhắn** | • Gửi tin nhắn<br>• Nhận tin nhắn<br>• Xem lịch sử chat |

---

## 2️⃣ NGƯỜI DÙNG THUỘC ĐƠN VỊ ĐƯỢC CẤP PHÉP (Licensed Unit Member)

### Mô Tả
- Nhân sự của một đơn vị logistics
- Được cấp phép truy cập hệ thống báo cáo
- Có thể xem tài liệu của đơn vị
- Nhập dữ liệu báo cáo cho đơn vị

### Quyền Hạn Cấp Cao
- ✅ Tất cả quyền của người dùng thường
- ✅ Xem tài liệu được phân chia
- ✅ Nhập & xem báo cáo đơn vị
- ❌ KHÔNG thể phê duyệt báo cáo
- ❌ KHÔNG thể quản lý nhân sự

### Tính Năng Chi Tiết

| **Mục** | **Chi Tiết** |
|---|---|
| **📚 Kho Tài Liệu** | **Duyệt tài liệu:**<br>• Tìm kiếm theo tiêu đề, mã<br>• Lọc theo danh mục<br>• Lọc theo loại tài liệu<br>• Lọc theo cơ quan ban hành<br><br>**Xem tài liệu:**<br>• Xem nội dung chi tiết<br>• Xem ngày ban hành<br>• Xem ngày hiệu lực<br>• Xem cơ quan ban hành<br>• Xem mã tài liệu<br><br>**Tải tài liệu:**<br>• Tải file PDF/Word<br>• Xem lịch sử tải |
| **📊 Báo Cáo Đơn Vị** | **Xem kỳ báo cáo:**<br>• Danh sách kỳ hiện tại<br>• Xem trạng thái (Mở/Khóa/Trả lại)<br>• Xem hạn chót nộp<br><br>**Nhập dữ liệu:**<br>• Truy cập mẫu báo cáo<br>• Nhập giá trị chỉ báo<br>• Lưu báo cáo nháp<br>• Nộp báo cáo chính thức<br><br>**Quản lý báo cáo:**<br>• Xem danh sách đã nộp<br>• Sửa báo cáo (nếu cho phép)<br>• Xem nhận xét<br>• Cập nhật báo cáo trả lại |
| **📈 Thống Kê** | • Xem báo cáo của đơn vị<br>• Xem thống kê báo cáo<br>• Xem lịch sử thay đổi |
| **👤 Hồ Sơ** | • Tất cả chức năng của người dùng thường<br>• Xem thông tin quân sự (nếu có) |

---

## 3️⃣ TRƯỞNG ĐƠN VỊ (Unit Leader)

### Mô Tả
- Quản lý của một đơn vị logistics
- Toàn quyền quản lý báo cáo của đơn vị & các đơn vị con
- Phê duyệt/trả lại báo cáo
- Quản lý nhân sự và tài liệu
- Xem báo cáo phân cấp từ các đơn vị con

### Quyền Hạn Cấp Cao
- ✅ Tất cả quyền của người dùng đơn vị
- ✅ Quản lý báo cáo cấp dưới
- ✅ Phê duyệt/trả lại báo cáo
- ✅ Quản lý nhân sự đơn vị
- ✅ Tải & phân chia tài liệu
- ✅ Xem báo cáo toàn hệ thống (nếu được phép)

### Tính Năng Chi Tiết

| **Mục** | **Chi Tiết** |
|---|---|
| **📊 QUẢN LÝ BÁO CÁO** | **Danh sách báo cáo:**<br>• Lọc theo trạng thái (Nháp/Nộp/Trả lại/Khóa)<br>• Lọc theo đơn vị<br>• Lọc theo kỳ báo cáo<br>• Tìm kiếm báo cáo<br><br>**Xử lý báo cáo:**<br>• Xem chi tiết báo cáo<br>• Thêm nhận xét<br>• **Phê duyệt báo cáo**<br>• **Trả lại báo cáo** (yêu cầu chỉnh sửa)<br>• **Khóa kỳ báo cáo** |
| **📈 BÁO CÁO PHÂN CẤP** | **Báo cáo đơn vị:**<br>• Xem tất cả báo cáo của đơn vị mình<br><br>**Báo cáo đơn vị con:**<br>• Xem báo cáo từ các đơn vị cấp dưới<br>• Xem tóm tắt báo cáo<br>• So sánh giữa các kỳ<br><br>**Báo cáo toàn hệ thống (nếu được phép):**<br>• Xem báo cáo từ tất cả đơn vị<br>• Xem biểu đồ thống kê<br>• Xuất báo cáo |
| **👥 QUẢN LÝ NHÂN SỰ** | **Danh sách nhân sự:**<br>• Xem tất cả nhân sự của đơn vị<br>• Xem thông tin cá nhân<br>• Xem trạng thái hoạt động<br><br>**Phân quyền:**<br>• Gán quyền nhập báo cáo<br>• Gán quyền xem báo cáo<br>• Gán quyền quản lý đơn vị con<br><br>**Quản lý quyền truy cập:**<br>• Bật/tắt tài khoản<br>• Đặt lại mật khẩu<br>• Xem lịch sử hoạt động |
| **📚 QUẢN LÝ TÀI LIỆU** | **Tải lên tài liệu:**<br>• Tạo tài liệu mới<br>• Chỉ định loại tài liệu<br>• Chỉ định danh mục<br>• Tải file lên<br><br>**Chỉnh sửa tài liệu:**<br>• Sửa thông tin<br>• Đặt ngày hiệu lực<br>• Xuất bản/ẩn tài liệu<br>• Xóa tài liệu<br><br>**Phân chia quyền:**<br>• Đặt phạm vi truy cập (công khai/hạn chế/riêng tư)<br>• Chỉ định đối tượng xem<br>• Cho phép/không cho phép tải |
| **👤 Hồ Sơ** | • Tất cả chức năng của người dùng đơn vị |

---

## 4️⃣ QUẢN TRỊ VIÊN (Administrator)

### Mô Tả
- Toàn quyền hệ thống
- Quản lý tất cả các khía cạnh của nền tảng
- Quản lý catalog, khách hàng, nội dung, tài liệu, báo cáo
- Cấu hình hệ thống và plugin

### Quyền Hạn Cấp Cao
- ✅ TOÀN QUYỀN hệ thống
- ✅ Quản lý sản phẩm và danh mục
- ✅ Quản lý khách hàng và đơn vị
- ✅ Quản lý nội dung (blog, diễn đàn, menu)
- ✅ Quản lý tài liệu và báo cáo
- ✅ Cài đặt hệ thống
- ✅ Quản lý plugin

### Tính Năng Chi Tiết

| **Mục** | **Chi Tiết** |
|---|---|
| **📦 QUẢN LÝ CATALOG** | **Sản phẩm:**<br>• Tìm kiếm, lọc sản phẩm<br>• Tạo/sửa sản phẩm mới<br>• Quản lý giá, kho tồn<br>• Thêm hình ảnh, video<br>• Quản lý thuộc tính<br>• Thiết lập SEO<br>• Xóa sản phẩm<br><br>**Danh mục:**<br>• Tạo/sửa danh mục phân cấp<br>• Thêm hình ảnh danh mục<br>• Quản lý thứ tự<br>• Thiết lập SEO<br><br>**Đánh giá:**<br>• Xem danh sách đánh giá<br>• Phê duyệt/từ chối<br>• Xóa đánh giá<br><br>**Cấp độ lọc:**<br>• Quản lý cấp độ lọc tùy chỉnh<br>• Tạo giá trị lọc |
| **👥 QUẢN LÝ KHÁCH HÀNG** | **Khách hàng:**<br>• Tìm kiếm, lọc khách hàng<br>• Xem chi tiết khách hàng<br>• Tạo khách hàng mới<br>• Sửa thông tin cá nhân<br>• Quản lý địa chỉ<br>• Đặt lại mật khẩu<br>• Xóa khách hàng<br>• Xuất danh sách<br><br>**Nhóm khách hàng:**<br>• Tạo/sửa/xóa nhóm<br>• Gán quyền cho nhóm<br><br>**Đơn vị (Vendors):**<br>• Danh sách đơn vị<br>• Tạo/sửa đơn vị<br>• Quản lý cấp bậc phân cấp<br>• Quản lý nhân sự<br>• Xóa đơn vị<br><br>**Khác:**<br>• Xem khách hàng trực tuyến<br>• Xem khách hàng chờ duyệt<br>• Phê duyệt/từ chối tài khoản<br>• Xem nhật ký hoạt động |
| **📰 QUẢN LÝ NỘI DUNG** | **Chủ đề (Topics):**<br>• Tạo/sửa/xóa chủ đề<br>• Thiết lập SEO<br><br>**Blog:**<br>• Quản lý bài blog<br>• Quản lý bình luận<br>• Phê duyệt/từ chối bình luận<br><br>**Diễn đàn:**<br>• Quản lý nhóm diễn đàn<br>• Quản lý chủ đề<br>• Xóa bài không hợp lệ<br><br>**Menu:**<br>• Tạo/sửa menu<br>• Quản lý quyền truy cập<br><br>**Nội dung đặc biệt:**<br>• Quản lý nội dung trang đăng nhập |
| **📚 QUẢN LÝ TÀI LIỆU** | **Tài liệu:**<br>• Tìm kiếm, lọc tài liệu<br>• Tạo/sửa tài liệu<br>• Quản lý phạm vi truy cập<br>• Quản lý quyền tải<br>• Xóa tài liệu<br><br>**Danh mục, loại, cơ quan:**<br>• Quản lý danh mục tài liệu<br>• Quản lý loại tài liệu<br>• Quản lý cơ quan ban hành<br><br>**Cài đặt:**<br>• Số tài liệu/trang<br>• Tìm kiếm trong nội dung<br>• Hiệu ứng hiển thị |
| **📊 QUẢN LÝ BÁO CÁO** | **Báo cáo tổng hợp:**<br>• Quản lý mẫu báo cáo<br>• Quản lý chỉ báo<br>• Quản lý kỳ báo cáo<br><br>**Xử lý báo cáo:**<br>• Xem tất cả báo cáo<br>• Phê duyệt/trả lại<br>• Khóa kỳ báo cáo<br><br>**Tổng hợp & xuất:**<br>• Xem báo cáo tổng hợp<br>• Xuất Excel/PDF<br>• Nhập dữ liệu từ Excel<br><br>**Quản lý nhân sự:**<br>• Phân quyền nhân sự báo cáo |
| **⚙️ CẤU HÌNH HỆ THỐNG** | **Cài đặt chung:**<br>• Tên cửa hàng, URL, logo<br>• Múi giờ, ngôn ngữ<br>• Tùy chỉnh giao diện<br><br>**Cài đặt từng loại:**<br>• Catalog settings<br>• Customer settings<br>• Blog, Forum settings<br>• Media settings<br><br>**Email & Kết nối:**<br>• Quản lý tài khoản email<br>• Cấu hình SMTP<br><br>**Lưu trữ & Cửa hàng:**<br>• Quản lý cửa hàng<br>• Quản lý kho lưu trữ<br><br>**Bảo mật:**<br>• Quản lý danh sách kiểm soát truy cập (ACL)<br>• Gán quyền cho nhóm |
| **🔧 QUẢN LÝ HỆ THỐNG** | **Monitoring:**<br>• Thông tin hệ thống<br>• Xem nhật ký lỗi (Log)<br>• Xem cảnh báo<br><br>**Bảo trì:**<br>• Xóa dữ liệu tạm<br>• Tái xây dựng bảng<br>• Kiểm tra tính toàn vẹn dữ liệu<br><br>**Email & Công việc:**<br>• Quản lý email chờ gửi<br>• Chạy công việc định thời<br>• Xem lịch sử chạy<br><br>**SEO & Công cụ:**<br>• Quản lý tên SEO thân thiện<br>• Danh sách tên được tạo<br><br>**Plugin:**<br>• Xem danh sách plugin<br>• Bật/tắt plugin<br>• Cấu hình plugin |

---

## 📑 BẢNG TÍNH NĂNG THEO CHỨC NĂNG

### I. Quản Lý Sản Phẩm & Catalog

| **Chức Năng** | **Người Dùng** | **Người Dùng Đơn Vị** | **Trưởng Đơn Vị** | **Quản Trị Viên** |
|---|:-:|:-:|:-:|:-:|
| Xem sản phẩm | ✅ | ✅ | ✅ | ✅ |
| Tìm kiếm sản phẩm | ✅ | ✅ | ✅ | ✅ |
| Lọc theo danh mục | ✅ | ✅ | ✅ | ✅ |
| Xem chi tiết sản phẩm | ✅ | ✅ | ✅ | ✅ |
| Tạo sản phẩm mới | ❌ | ❌ | ❌ | ✅ |
| Sửa thông tin sản phẩm | ❌ | ❌ | ❌ | ✅ |
| Quản lý giá | ❌ | ❌ | ❌ | ✅ |
| Quản lý kho tồn | ❌ | ❌ | ❌ | ✅ |
| Thêm hình ảnh | ❌ | ❌ | ❌ | ✅ |
| Xóa sản phẩm | ❌ | ❌ | ❌ | ✅ |
| Quản lý danh mục | ❌ | ❌ | ❌ | ✅ |

---

### II. Quản Lý Tài Liệu

| **Chức Năng** | **Người Dùng** | **Người Dùng Đơn Vị** | **Trưởng Đơn Vị** | **Quản Trị Viên** |
|---|:-:|:-:|:-:|:-:|
| Xem tài liệu công khai | ❌ | ✅ | ✅ | ✅ |
| Tìm kiếm tài liệu | ❌ | ✅ | ✅ | ✅ |
| Lọc theo danh mục | ❌ | ✅ | ✅ | ✅ |
| Tải tài liệu | ❌ | ✅ | ✅ | ✅ |
| Xem tài liệu liên quan | ❌ | ✅ | ✅ | ✅ |
| Tạo tài liệu mới | ❌ | ❌ | ✅ | ✅ |
| Sửa tài liệu | ❌ | ❌ | ✅ | ✅ |
| Quản lý phạm vi truy cập | ❌ | ❌ | ✅ | ✅ |
| Phân quyền tài liệu | ❌ | ❌ | ✅ | ✅ |
| Xóa tài liệu | ❌ | ❌ | ✅ | ✅ |
| Quản lý danh mục tài liệu | ❌ | ❌ | ❌ | ✅ |
| Quản lý loại tài liệu | ❌ | ❌ | ❌ | ✅ |
| Quản lý cơ quan ban hành | ❌ | ❌ | ❌ | ✅ |

---

### III. Quản Lý Báo Cáo Logistics

| **Chức Năng** | **Người Dùng** | **Người Dùng Đơn Vị** | **Trưởng Đơn Vị** | **Quản Trị Viên** |
|---|:-:|:-:|:-:|:-:|
| Xem kỳ báo cáo | ❌ | ✅ | ✅ | ✅ |
| Nhập dữ liệu báo cáo | ❌ | ✅ | ✅ | ✅ |
| Lưu báo cáo nháp | ❌ | ✅ | ✅ | ✅ |
| Nộp báo cáo | ❌ | ✅ | ✅ | ✅ |
| Xem báo cáo đơn vị | ❌ | ✅ | ✅ | ✅ |
| Xem báo cáo cấp dưới | ❌ | ❌ | ✅ | ✅ |
| Phê duyệt báo cáo | ❌ | ❌ | ✅ | ✅ |
| Trả lại báo cáo | ❌ | ❌ | ✅ | ✅ |
| Khóa kỳ báo cáo | ❌ | ❌ | ✅ | ✅ |
| Xem báo cáo toàn hệ thống | ❌ | ❌ | ❌/✅ | ✅ |
| Quản lý mẫu báo cáo | ❌ | ❌ | ❌ | ✅ |
| Quản lý chỉ báo | ❌ | ❌ | ❌ | ✅ |
| Quản lý kỳ báo cáo | ❌ | ❌ | ❌ | ✅ |
| Xuất báo cáo Excel/PDF | ❌ | ❌ | ✅ | ✅ |
| Nhập dữ liệu từ Excel | ❌ | ❌ | ❌ | ✅ |

---

### IV. Quản Lý Khách Hàng

| **Chức Năng** | **Người Dùng** | **Người Dùng Đơn Vị** | **Trưởng Đơn Vị** | **Quản Trị Viên** |
|---|:-:|:-:|:-:|:-:|
| Xem hồ sơ cá nhân | ✅ | ✅ | ✅ | ✅ |
| Sửa hồ sơ cá nhân | ✅ | ✅ | ✅ | ✅ |
| Đổi mật khẩu | ✅ | ✅ | ✅ | ✅ |
| Xem danh sách khách hàng | ❌ | ❌ | ❌ | ✅ |
| Tạo khách hàng mới | ❌ | ❌ | ❌ | ✅ |
| Sửa thông tin khách hàng | ❌ | ❌ | ❌ | ✅ |
| Xóa khách hàng | ❌ | ❌ | ❌ | ✅ |
| Quản lý nhóm khách hàng | ❌ | ❌ | ❌ | ✅ |
| Quản lý đơn vị (Vendors) | ❌ | ❌ | ❌ | ✅ |
| Quản lý nhân sự nhân sự | ❌ | ❌ | ✅ | ✅ |

---

## 🎯 TÓMMÉT: BẢNG TÓM TẮT NHANH

```
┌─────────────────────────┬──────────┬──────────┬──────────┬──────────┐
│ CHỨC NĂNG CHÍNH         │ Người DK │ Người ĐV │ Trưởng ĐV│  Admin   │
├─────────────────────────┼──────────┼──────────┼──────────┼──────────┤
│ Xem Sản Phẩm            │    ✅    │    ✅    │    ✅    │    ✅    │
│ Đánh Giá & Bình Luận    │    ✅    │    ✅    │    ✅    │    ✅    │
│ Quản Lý Hồ Sơ          │    ✅    │    ✅    │    ✅    │    ✅    │
│ Xem Tài Liệu           │    ❌    │    ✅    │    ✅    │    ✅    │
│ Nhập Báo Cáo           │    ❌    │    ✅    │    ✅    │    ✅    │
│ Phê Duyệt Báo Cáo      │    ❌    │    ❌    │    ✅    │    ✅    │
│ Quản Lý Nhân Sự        │    ❌    │    ❌    │    ✅    │    ✅    │
│ Tải Tài Liệu Lên       │    ❌    │    ❌    │    ✅    │    ✅    │
│ Quản Lý Sản Phẩm       │    ❌    │    ❌    │    ❌    │    ✅    │
│ Quản Lý Nội Dung       │    ❌    │    ❌    │    ❌    │    ✅    │
│ Cài Đặt Hệ Thống       │    ❌    │    ❌    │    ❌    │    ✅    │
└─────────────────────────┴──────────┴──────────┴──────────┴──────────┘
```

---

## 📞 HỖ TRỢ

**Liên hệ Admin:** luckyboizclone1@gmail.com  
**Phiên bản:** nopCommerce 5.00  
**Cập nhật:** 2026-05-03

---

*Tài liệu này giúp hiểu rõ quyền hạn và tính năng của từng loại tài khoản trong hệ thống.*
