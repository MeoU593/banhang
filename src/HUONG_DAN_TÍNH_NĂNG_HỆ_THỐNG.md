
### Kiến trúc Người Dùng
Hệ thống hỗ trợ 3 loại người dùng chính:
1. **Trưởng Đơn Vị (Unit Leader)** - Quản lý đơn vị con, báo cáo, tài liệu
2. **Người Dùng Thuộc Đơn Vị Được Cấp Phép (Licensed Unit Member)** - Nhân sự của đơn vị
3. **Người Dùng Thường (Regular User)** - Người dùng hệ thống cơ bản

---

# PHẦN 1: CHỨC NĂNG NGƯỜI DÙNG

## I. CHỨC NĂNG CHUNG CHO TẤT CẢ NGƯỜI DÙNG

### 1. **Trang Chủ**
- Xem danh sách sản phẩm/dịch vụ 
- Xem các slide quảng cáo trên banner
- Xem nội dung trang chủ (trang chủ được trang trí qua hệ thống quản lý)
- Truy cập các danh mục sản phẩm chính

### 2. **Quản Lý Hồ Sơ Cá Nhân**
- **Cập nhật thông tin cá nhân:**
  - Tên, email, số điện thoại
  - Ảnh đại diện
- **Quản lý mật khẩu:**
  - Đổi mật khẩu
  - Đặt lại mật khẩu quên
- **Xác thực đa yếu tố (MFA):**
  - Bật/tắt 2FA nếu được kích hoạt
  - Quản lý các thiết bị tin cậy
- **Xem hồ sơ quân sự (nếu có):**
  - Thông tin quân hàm, đơn vị quân sự
  - Thông tin liên lạc quân sự

### 3. **Duyệt Sản Phẩm/Dịch Vụ**
- **Tìm kiếm sản phẩm:**
  - Tìm kiếm theo tên, từ khóa
  - Lọc theo danh mục
  - Lọc theo nhà cung cấp
- **Xem chi tiết sản phẩm:**
  - Mô tả chi tiết
  - Hình ảnh sản phẩm
  - Mã tài liệu tham khảo
- **Đánh giá sản phẩm:**
  - Xem đánh giá từ người dùng khác
  - Gửi đánh giá 
  - Xem nhận xét bình luận

### 4. **Quản Lý Wishlist (Danh Sách Yêu Thích)**
- Thêm/xóa sản phẩm khỏi wishlist

### 5. **Tin Tức & Thông Báo**
- **Xem tin tức:**
  - Tin tức chung (Tin tức)
  - Tin tức pháp lý (Công văn)
  - Lọc tin theo loại
  - Xem chi tiết tin tức

### 6. **Blog & Diễn Đàn**
- Xem bài blog
- Bình luận bài blog
- Tham gia diễn đàn (Forum)
- Xem các chủ đề diễn đàn
- Trả lời bài viết trên diễn đàn

### 7. **Quản Lý Tin Nhắn Cá Nhân**
- Gửi tin nhắn cho người dùng khác
- Nhận tin nhắn
- Xem lịch sử trò chuyện
- Xóa tin nhắn

---

## II. CHỨC NĂNG NGƯỜI DÙNG THƯỜNG (REGULAR USER)

Người dùng thường là những người đăng ký cơ bản.

### Quyền hạn:
- ✅ Xem sản phẩm/dịch vụ công khai
- ✅ Bình luận và đánh giá sản phẩm
- ✅ Quản lý hồ sơ cá nhân
- ✅ Xem tin tức công khai
- ❌ Không thể xem báo cáo 
- ❌ Không thể xem tài liệu hạn chế
- ❌ Không thể tạo báo cáo

---

## III. CHỨC NĂNG NGƯỜI DÙNG THUỘC ĐƠN VỊ ĐƯỢC CẤP PHÉP (LICENSED UNIT MEMBER)

Người dùng này là nhân sự của một đơn vị được phép truy cập hệ thống.

### Quyền hạn cơ bản:
- ✅ Tất cả quyền của người dùng thường
- ✅ Xem tài liệu được phân chia
- ✅ Tham gia báo cáo đơn vị

### A. **Xem Tài liệu (Kho Tài liệu)**
- **Duyệt tài liệu:**
  - Tìm kiếm tài liệu theo tiêu đề, mã
  - Lọc theo danh mục tài liệu
  - Lọc theo loại tài liệu
  - Lọc theo cơ quan ban hành
- **Xem tài liệu:**
  - Xem nội dung tài liệu
  - Xem hình ảnh đại diện
  - Xem thông tin:
    - Ngày ban hành
    - Ngày có hiệu lực
    - Cơ quan ban hành
    - Mã tài liệu
- **Tải tài liệu:**
  - Tải file PDF/Word
- **Xem tài liệu liên quan:**
  - Danh sách tài liệu liên quan

### B. **Nhập Báo Cáo Đơn Vị (Reports)**
- **Xem kỳ báo cáo:**
  - Xem các kỳ báo cáo hiện tại
  - Xem trạng thái từng kỳ (Mở, Khóa, Trả lại)
  - Xem hạn chót nộp báo cáo
- **Nhập dữ liệu báo cáo:**
  - Truy cập mẫu báo cáo của đơn vị
  - Nhập giá trị cho các chỉ báo
  - Lưu báo cáo nháp
  - Nộp báo cáo chính thức
- **Quản lý báo cáo:**
  - Xem danh sách báo cáo đã nộp
  - Sửa báo cáo (nếu cho phép)
  - Xem nhận xét từ người quản lý
  - Cập nhật báo cáo bị trả lại

### C. **Xem Báo Cáo Của Đơn Vị**
- Xem báo cáo đã nộp của đơn vị mình
- Xem thống kê báo cáo
- Xem lịch sử thay đổi báo cáo

---

## IV. CHỨC NĂNG TRƯỞNG ĐƠN VỊ (UNIT LEADER)

Trưởng đơn vị là người quản lý đơn vị, có quyền quản lý toàn bộ hoạt động báo cáo và tài liệu của đơn vị và các đơn vị cấp dưới.

### Quyền hạn cơ bản:
- ✅ Tất cả quyền của người dùng thuộc đơn vị
- ✅ Quản lý báo cáo đơn vị cấp dưới
- ✅ Phê duyệt/trả lại báo cáo
- ✅ Xem báo cáo toàn hệ thống (nếu được phép)

### A. **Quản Lý Báo Cáo Đơn Vị (Admin Portal)**
- **Xem danh sách báo cáo:**
  - Lọc báo cáo theo trạng thái (Nháp, Nộp, Trả lại, Khóa)
  - Lọc báo cáo theo đơn vị
  - Lọc báo cáo theo kỳ báo cáo
  - Tìm kiếm báo cáo
- **Phê duyệt/Trả lại báo cáo:**
  - Xem chi tiết báo cáo
  - Thêm nhận xét
  - Phê duyệt báo cáo
  - Trả lại báo cáo với yêu cầu chỉnh sửa
- **Khóa kỳ báo cáo:**
  - Khóa kỳ báo cáo để ngăn chỉnh sửa tiếp theo

### B. **Xem Báo Cáo Phân Cấp**
- **Báo cáo của đơn vị:**
  - Xem tất cả báo cáo của đơn vị mình
- **Báo cáo của đơn vị con:**
  - Xem báo cáo của tất cả đơn vị cấp dưới
  - Xem tóm tắt báo cáo từ các đơn vị con
  - So sánh báo cáo giữa các kỳ

### C. **Quản Lý Nhân Sự Đơn Vị (Vendor Staff Access)**
- **Xem danh sách nhân sự:**
  - Danh sách tất cả nhân sự của đơn vị
  - Thông tin cá nhân từng nhân sự
  - Trạng thái hoạt động
- **Phân quyền nhân sự:**
  - Gán quyền quản lý các đơn vị con

### D. **Quản Lý Tài Liệu**
- **Tải lên tài liệu:**
  - Tạo tài liệu mới
  - Chỉ định loại tài liệu
  - Chỉ định danh mục
  - Tải file lên
- **Quản lý tài liệu:**
  - Sửa thông tin tài liệu
  - Đặt ngày hiệu lực
  - Xuất bản/ẩn tài liệu
  - Xóa tài liệu
- **Phân chia quyền truy cập tài liệu:**
  - Đặt phạm vi truy cập (công khai, hạn chế)

---

# PHẦN 2: CHỨC NĂNG QUẢN TRỊ VIÊN (ADMIN)

Quản trị viên có quyền toàn bộ hệ thống và có thể quản lý tất cả các khía cạnh của nền tảng.

---

## I. QUẢN LÝ CATALOG (DANH MỤC SẢN PHẨM)

### A. **Quản Lý Sản Phẩm**
- **Xem danh sách sản phẩm:**
  - Tìm kiếm sản phẩm theo tên, SKU
  - Lọc theo danh mục, nhà cung cấp
  - Lọc theo trạng thái (xuất bản, ẩn)
- **Tạo/sửa sản phẩm:**
  - Điền thông tin cơ bản (tên, mô tả, giá)
  - Thêm hình ảnh sản phẩm
  - Quản lý thuộc tính sản phẩm
- **Quản lý giá :**
  - Cập nhật giá sản phẩm
- **Quản lý danh mục sản phẩm:**
  - Chỉ định sản phẩm vào danh mục
  - Quản lý thứ tự hiển thị
  - Thiết lập hình ảnh danh mục
- **Xóa sản phẩm**

### B. **Quản Lý Danh Mục**
- **Xem danh sách danh mục:**
  - Danh sách phân cấp tất cả danh mục
  - Tìm kiếm danh mục
- **Tạo/sửa danh mục:**
  - Điền tên, mô tả danh mục
  - Chỉ định danh mục cha
  - Thêm hình ảnh danh mục
  - Quản lý thứ tự hiển thị
- **Xóa danh mục**

### C. **Quản Lý Đánh Giá Sản Phẩm**
- **Xem danh sách đánh giá:**
  - Lọc đánh giá theo sản phẩm
  - Lọc đánh giá theo trạng thái (chờ duyệt, được phê duyệt, từ chối)
  - Tìm kiếm đánh giá
- **Phê duyệt/Từ chối đánh giá**
- **Xóa đánh giá**
- **Xem tính hữu ích của đánh giá**

---

## II. QUẢN LÝ KHÁCH HÀNG

### A. **Quản Lý Khách Hàng**
- **Xem danh sách khách hàng:**
  - Tìm kiếm khách hàng theo email, tên
  - Lọc theo nhóm khách hàng (vai trò)
  - Lọc theo ngày đăng ký
  - Xem thông tin hoạt động (lần đăng nhập cuối, số đơn hàng)
- **Xem chi tiết khách hàng:**
  - Thông tin cá nhân (tên, email, số điện thoại)
  - Danh sách địa chỉ
  - Hồ sơ quân sự (nếu có)
  - Lịch sử hoạt động
- **Tạo khách hàng mới**
- **Sửa thông tin khách hàng:**
  - Cập nhật thông tin cá nhân
  - Thêm/xóa địa chỉ
  - Thay đổi mật khẩu
- **Xóa khách hàng**
- **Xuất danh sách khách hàng**

### C. **Quản Lý Đơn Vị **
- **Xem danh sách đơn vị:**
  - Tìm kiếm đơn vị theo tên
  - Lọc theo trạng thái (hoạt động, ẩn)
- **Tạo/sửa đơn vị:**
  - Thông tin cơ bản (tên, mô tả)
  - Ảnh đại diện
  - Thông tin liên lạc
  - Cấp quản lý (phân cấp)
  - Tài khoản quản lý chính (PmCustomerId)
  - Địa chỉ
- **Xóa đơn vị**
- **Quản lý nhân sự đơn vị:**
  - Xem danh sách nhân sự
  - Thêm/xóa nhân sự

### D. **Khách Hàng Trực Tuyến**
- Xem danh sách khách hàng đang trực tuyến
- Xem thông tin phiên làm việc
- Xem hoạt động cuối cùng

### E. **Khách Hàng Chờ Duyệt**
- Xem danh sách tài khoản chờ phê duyệt
- Phê duyệt tài khoản
- Từ chối tài khoản

### F. **Nhật Ký Hoạt Động**
- **Xem nhật ký hoạt động:**
  - Lịch sử tất cả hoạt động của khách hàng
  - Lọc theo loại hoạt động
  - Lọc theo khách hàng
  - Lọc theo ngày
- **Quản lý loại hoạt động:**
  - Danh sách loại hoạt động
  - Bật/tắt ghi log loại hoạt động

---

## III. QUẢN LÝ NỘI DUNG

### A. **Quản Lý Chủ Đề (Topics)**
- **Xem danh sách chủ đề:**
  - Tìm kiếm chủ đề
  - Lọc theo trạng thái
- **Tạo/sửa chủ đề:**
  - Tiêu đề chủ đề
- **Xóa chủ đề**

### B. **Quản Lý Blog**
- **Xem bài blog:**
  - Danh sách bài blog
  - Lọc theo danh mục, tác giả
  - Lọc theo trạng thái (xuất bản, nháp)
- **Tạo/sửa bài blog:**
  - Tiêu đề, nội dung
  - Hình ảnh bài blog
  - Tag, danh mục
  - Ngày xuất bản
  - Cho phép bình luận
- **Quản lý bình luận blog:**
  - Xem bình luận blog
  - Phê duyệt/từ chối bình luận
  - Xóa bình luận
- **Xóa bài blog**

### D. **Quản Lý Menu**
- **Xem danh sách menu:**
  - Menu cho các trang khác nhau
- **Tạo/sửa menu:**
  - Thêm mục menu
  - Chỉ định URL
  - Thiết lập thứ tự
  - Thiết lập quyền truy cập
- **Xóa menu**

### E. **Nội Dung Trang Đăng Nhập (Duty Message)**
- **Chỉnh sửa nội dung:**
  - Tiêu đề thông điệp
  - Nội dung (hỗ trợ HTML)
  - Hình ảnh nền
  - Tắt/bật hiển thị

---

## IV. QUẢN LÝ TÀI LIỆU (DOCUMENT PORTAL)

### A. **Quản Lý Tài Liệu**
- **Xem danh sách tài liệu:**
  - Tìm kiếm theo tiêu đề, mã
  - Lọc theo danh mục, loại, cơ quan ban hành
  - Lọc theo trạng thái (xuất bản, nháp)
  - Lọc theo phạm vi truy cập
- **Tạo/sửa tài liệu:**
  - Tiêu đề tài liệu
  - Mã tài liệu
  - Tóm tắt, nội dung
  - Danh mục, loại, cơ quan ban hành
  - Ngày ban hành, ngày có hiệu lực
  - Tải file (PDF, Word, v.v.)
  - Ảnh đại diện
  - Từ khóa SEO
  **Đối tượng xem:**
    - Tất cả người dùng
    - Chỉ người dùng có tài khoản
  - Cho phép tải
  - Hiện trên trang chủ
- **Xóa tài liệu**

### C. **Quản Lý Loại Tài Liệu**
- **Xem danh sách loại:**
  - Tất cả loại tài liệu
- **Tạo/sửa loại:**
  - Tên loại tài liệu
  - Mô tả
  - Tự động tạo slug
- **Xóa loại**

### D. **Quản Lý Cơ Quan Ban Hành**
- **Xem danh sách cơ quan:**
  - Tất cả cơ quan ban hành
- **Tạo/sửa cơ quan:**
  - Tên cơ quan
  - Mô tả
  - Liên hệ
- **Xóa cơ quan**

### E. **Cài Đặt Kho Tài Liệu**
- Số tài liệu mỗi trang
- Tìm kiếm trong nội dung (bật/tắt)
- Hiển thị tài liệu liên quan
- Hiển thị số lượt tải
- Yêu cầu đăng nhập cho tài liệu hạn chế

---

### H. **Quản Lý Quyền Truy Cập Nhân Sự (Vendor Staff Access)**
- **Xem danh sách nhân sự:**
  - Tất cả nhân sự của các đơn vị

---

## VI. QUẢN LÝ HỆ THỐNG & CẤU HÌNH

### A. **Quản Lý Cài Đặt (Settings)**
- **Cài đặt chung:**
  - Tên cửa hàng, URL, logo
  - Múi giờ, ngôn ngữ mặc định
  - Cho phép vãng khách khám phá
  - Yêu cầu xác nhân tài khoản
- **Cài đặt khách hàng:**
  - Cho phép đăng ký
  - Cho phép khách hàng xem hồ sơ
  - Xác thực đa yếu tố
  - Quản lý quyền
- **Cài đặt Catalog:**
  - Cho phép bình luận sản phẩm
  - Cho phép đánh giá sản phẩm
  - Hiển thị số lượng sản phẩm
  - Cho phép wishlist
- **Cài đặt Blog:**
  - Bật/tắt blog
  - Cho phép bình luận
- **Cài đặt Diễn Đàn:**
  - Bật/tắt diễn đàn
  - Cho phép bài viết mới
- **Cài đặt Phương Tiện:**
  - Kích thước hình ảnh mặc định
  - Chất lượng hình ảnh
  - Thư mục lưu trữ

### B. **Quản Lý Tài Khoản Email**
- **Xem danh sách tài khoản:**
  - Tất cả tài khoản email cấu hình
- **Tạo/sửa tài khoản:**
  - Tên hiển thị
  - Email
- **Xóa tài khoản**
- **Kiểm tra kết nối**

---


## X. GHI CHÚ QUAN TRỌNG

### Cấu Trúc Người Dùng
- **VendorEmployee** - Quản lý nhân sự đơn vị (phân cấp)
- **PmCustomerId** - Quản lý chính của đơn vị (toàn quyền)

