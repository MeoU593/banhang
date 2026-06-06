# Hướng dẫn sử dụng hệ thống theo cấp tài khoản

Tài liệu này được viết theo từng cấp tài khoản. Trong mỗi cấp tài khoản, từng chức năng được trình bày như một hướng dẫn thao tác thực tế: mục đích, điều kiện, URL thực hiện, URL để chụp ảnh, vị trí đặt ảnh, các bước nhập liệu cụ thể, kết quả và ví dụ minh họa.

Quy ước URL:

- Địa chỉ hệ thống khi chạy local: `https://localhost:64260`
- URL trong tài liệu dùng dạng rút gọn, ví dụ `/login/`. Khi chụp ảnh, mở đầy đủ: `https://localhost:64260/login/`.
- Các URL có `{id}`, `{vendorId}`, `{customerId}`, `{documentId}`, `{reportId}` cần thay bằng mã thực tế trên hệ thống.
- Ảnh minh họa đã được đặt ngay trong từng chức năng. Sau khi chụp ảnh, thay đường dẫn placeholder trong `![...](...)` bằng tên file ảnh thật.

## 1. Khách chưa đăng nhập

Khách chưa đăng nhập là người truy cập công khai vào hệ thống. Nhóm này chỉ xem được nội dung public và đăng ký tài khoản, không được vào khu vực quản trị và không được thao tác dữ liệu nội bộ.

### 1.1. Xem trang chủ

Mục đích:

- Xem tổng quan cổng thông tin, tin mới, văn bản mới, kho tài liệu công khai và các menu điều hướng chính.

Điều kiện thực hiện:

- Không cần đăng nhập.
- Trình duyệt truy cập được địa chỉ hệ thống.

URL thực hiện:

- `/`

URL chụp ảnh:

- `/`

Ảnh minh họa:

![Trang chủ hệ thống](images/khach-trang-chu.png)

Cần chụp những gì:

- Chụp phần đầu trang gồm logo, thanh tìm kiếm, menu trái và các khối nội dung nổi bật.

Hướng dẫn từng bước:

1. Mở trình duyệt Chrome, Edge hoặc Firefox.
2. Nhập `https://localhost:64260` vào thanh địa chỉ.
3. Nhấn `Enter` để truy cập.
4. Quan sát phần đầu trang để kiểm tra logo, thanh tìm kiếm và menu chính.
5. Quan sát menu bên trái để xem các mục như trang chủ, tin tức, văn bản, đơn vị, kho tài liệu.
6. Cuộn xuống để xem tin nổi bật, văn bản mới và các nội dung công khai.

Kết quả:

- Hệ thống hiển thị trang chủ.
- Khách xem được nội dung công khai mà không cần đăng nhập.

Ví dụ:

- Một cán bộ muốn xem nhanh tin mới của hệ thống. Cán bộ mở `https://localhost:64260`, xem khu vực tin nổi bật và bấm vào một tin để đọc chi tiết.

### 1.2. Tìm kiếm toàn hệ thống

Mục đích:

- Tìm kiếm nhanh sản phẩm, tin tức, văn bản, tài liệu và đơn vị bằng một từ khóa chung.

Điều kiện thực hiện:

- Không cần đăng nhập.
- Kết quả hiển thị phụ thuộc vào dữ liệu đã được công khai.

URL thực hiện:

- `/search/`

URL chụp ảnh:

- `/search/?q=202`

Ảnh minh họa:

![Tìm kiếm toàn hệ thống](images/khach-tim-kiem-toan-he-thong.png)

Cần chụp những gì:

- Chụp ô tìm kiếm, nút `Tìm kiếm`, các ô thống kê kết quả và danh sách kết quả bên dưới.

Hướng dẫn từng bước:

1. Từ trang chủ, bấm vào ô tìm kiếm trên thanh đầu trang.
2. Nhập từ khóa cần tìm, ví dụ `202`.
3. Bấm nút `Tìm kiếm` hoặc nhấn `Enter`.
4. Hệ thống mở trang kết quả tìm kiếm.
5. Xem các nhóm kết quả gồm `Sản phẩm`, `Tin tức`, `Văn bản`, `Đơn vị`.
6. Bấm vào ô `Tin tức` nếu chỉ muốn xem kết quả là tin tức.
7. Bấm vào ô `Văn bản` nếu chỉ muốn xem kết quả là văn bản.
8. Bấm vào tiêu đề một kết quả để mở trang chi tiết.

Kết quả:

- Hệ thống hiển thị các kết quả phù hợp với từ khóa đã nhập.
- Người dùng có thể lọc kết quả theo từng nhóm dữ liệu.

Ví dụ:

- Nhập từ khóa `202`. Hệ thống có thể hiển thị đơn vị liên quan đến `202`, tin tức có nội dung chứa `202` và văn bản có mã hoặc tiêu đề chứa `202`.

### 1.3. Xem danh sách tin tức

Mục đích:

- Xem các bài tin tức đã được xuất bản trên hệ thống.

Điều kiện thực hiện:

- Không cần đăng nhập.
- Tin tức phải ở trạng thái đã xuất bản.

URL thực hiện:

- `/blog/news`

URL chụp ảnh:

- `/blog/news`

Ảnh minh họa:

![Danh sách tin tức](images/khach-danh-sach-tin-tuc.png)

Cần chụp những gì:

- Chụp phần thống kê, tin nổi bật, danh sách tin và bộ lọc nếu có.

Hướng dẫn từng bước:

1. Từ menu hệ thống, chọn mục `Tin tức`.
2. Hoặc nhập trực tiếp `/blog/news` trên thanh địa chỉ.
3. Xem số liệu thống kê ở đầu trang, ví dụ số tin tức và số đơn vị đăng tin.
4. Xem tin nổi bật ở khu vực trên cùng của danh sách.
5. Cuộn xuống để xem các tin còn lại.
6. Nếu cần tìm theo từ khóa, nhập từ khóa vào ô tìm kiếm của trang tin tức.
7. Bấm vào tiêu đề tin để xem chi tiết.

Kết quả:

- Hệ thống hiển thị danh sách tin tức đã xuất bản.
- Mỗi tin có tiêu đề, ảnh đại diện, ngày đăng và lượt xem nếu có.

Ví dụ:

- Muốn xem tin về hoạt động hậu cần, nhập từ khóa `hậu cần`, sau đó bấm vào tin có tiêu đề phù hợp để đọc chi tiết.

### 1.4. Xem chi tiết tin tức

Mục đích:

- Đọc nội dung đầy đủ của một bài tin tức.

Điều kiện thực hiện:

- Bài tin đã được xuất bản.
- Người xem có đường dẫn hoặc tìm thấy bài tin trong danh sách.

URL thực hiện:

- `/blog/{slug}`

URL chụp ảnh:

- `/blog/{slug}`

Ảnh minh họa:

![Chi tiết tin tức](images/khach-chi-tiet-tin-tuc.png)

Cần chụp những gì:

- Chụp tiêu đề bài viết, ảnh đại diện, nội dung chính, ngày đăng và lượt xem.

Hướng dẫn từng bước:

1. Mở trang `/blog/news`.
2. Tìm bài tin cần đọc.
3. Bấm vào tiêu đề hoặc ảnh đại diện của bài tin.
4. Hệ thống mở trang chi tiết bài viết.
5. Đọc tiêu đề, tóm tắt và nội dung chi tiết.
6. Cuộn trang để xem đầy đủ hình ảnh và nội dung.
7. Bấm nút quay lại của trình duyệt nếu muốn trở về danh sách tin.

Kết quả:

- Hệ thống hiển thị nội dung chi tiết của bài tin.
- Lượt xem của bài tin có thể được ghi nhận theo cấu hình hệ thống.

Ví dụ:

- Từ danh sách tin tức, bấm vào tin `Lương thực dự trữ trong mùa mưa bão` để xem nội dung chi tiết và các ảnh kèm theo.

### 1.5. Xem danh sách văn bản

Mục đích:

- Xem các bài đăng dạng văn bản hoặc thông báo đã xuất bản trên hệ thống.

Điều kiện thực hiện:

- Không cần đăng nhập.
- Văn bản phải ở trạng thái đã xuất bản.

URL thực hiện:

- `/blog/documents`

URL chụp ảnh:

- `/blog/documents`

Ảnh minh họa:

![Danh sách văn bản](images/khach-danh-sach-van-ban.png)

Cần chụp những gì:

- Chụp phần thống kê, bộ lọc, bảng danh sách văn bản và cột lượt xem.

Hướng dẫn từng bước:

1. Từ menu hệ thống, chọn mục `Văn bản`.
2. Hoặc nhập trực tiếp `/blog/documents`.
3. Xem thống kê ở đầu trang, gồm tổng số văn bản và số đơn vị đăng văn bản.
4. Nếu cần tìm văn bản, nhập từ khóa vào ô tìm kiếm.
5. Nếu cần lọc theo ngày, chọn khoảng thời gian từ ngày đến ngày.
6. Bấm `Tìm kiếm` để áp dụng bộ lọc.
7. Bấm vào tiêu đề văn bản để xem chi tiết.

Kết quả:

- Hệ thống hiển thị danh sách văn bản đã xuất bản.
- Người xem thấy được tiêu đề, ngày đăng và lượt xem.

Ví dụ:

- Nhập từ khóa `quý II`, hệ thống hiển thị các văn bản có tiêu đề hoặc nội dung liên quan đến quý II.

### 1.6. Xem kho tài liệu công khai

Mục đích:

- Xem và tải các tài liệu công khai trong kho tài liệu.

Điều kiện thực hiện:

- Không cần đăng nhập đối với tài liệu công khai.
- Tài liệu phải được xuất bản và cho phép hiển thị.

URL thực hiện:

- `/tai-lieu`

URL chụp ảnh:

- `/tai-lieu`

Ảnh minh họa:

![Kho tài liệu công khai](images/khach-kho-tai-lieu-cong-khai.png)

Cần chụp những gì:

- Chụp bộ lọc tài liệu, danh sách tài liệu, thông tin lượt tải và nút xem/tải.

Hướng dẫn từng bước:

1. Chọn menu `Kho tài liệu`.
2. Hệ thống hiển thị danh sách tài liệu công khai.
3. Nếu cần tìm theo tiêu đề, nhập tiêu đề vào ô tìm kiếm.
4. Nếu cần lọc theo danh mục, chọn danh mục trong hộp chọn.
5. Nếu cần lọc theo loại tài liệu, chọn loại tài liệu từ danh sách.
6. Nếu cần lọc theo cơ quan ban hành, chọn cơ quan ban hành.
7. Bấm nút `Tìm kiếm`.
8. Bấm `Xem` để mở chi tiết tài liệu.
9. Bấm `Tải xuống` nếu muốn tải file về máy.

Kết quả:

- Khách xem được tài liệu công khai.
- Khách không xem được tài liệu nội bộ nếu chưa đăng nhập hoặc không có quyền.

Ví dụ:

- Chọn danh mục `Hướng dẫn`, nhập từ khóa `an toàn`, sau đó mở tài liệu `Hướng dẫn bảo đảm an toàn kho lương thực`.

### 1.7. Xem danh sách đơn vị

Mục đích:

- Xem các đơn vị được công khai trên hệ thống.

Điều kiện thực hiện:

- Không cần đăng nhập.
- Đơn vị đã được tạo và cho phép hiển thị.

URL thực hiện:

- `/don-vi`

URL chụp ảnh:

- `/don-vi`

Ảnh minh họa:

![Danh sách đơn vị](images/khach-danh-sach-don-vi.png)

Cần chụp những gì:

- Chụp ô tìm kiếm đơn vị và các thẻ đơn vị trong danh sách.

Hướng dẫn từng bước:

1. Chọn menu `Đơn vị`.
2. Hệ thống hiển thị danh sách các đơn vị.
3. Nếu cần tìm nhanh, nhập tên đơn vị vào ô tìm kiếm.
4. Bấm `Tìm kiếm` hoặc nhấn `Enter`.
5. Xem kết quả đơn vị phù hợp.
6. Bấm vào tên đơn vị hoặc nút chi tiết để xem trang chi tiết đơn vị.

Kết quả:

- Khách xem được danh sách đơn vị công khai.

Ví dụ:

- Nhập `Lữ đoàn 202` vào ô tìm kiếm, hệ thống hiển thị đơn vị có tên tương ứng để người dùng mở chi tiết.

### 1.8. Đăng ký tài khoản

Mục đích:

- Tạo tài khoản mới để sử dụng các chức năng yêu cầu đăng nhập.

Điều kiện thực hiện:

- Người dùng chưa có tài khoản.
- Email đăng ký chưa tồn tại trên hệ thống.

URL thực hiện:

- `/register/`

URL chụp ảnh:

- `/register/`

Ảnh minh họa:

![Đăng ký tài khoản](images/khach-dang-ky-tai-khoan.png)

Cần chụp những gì:

- Chụp form đăng ký với các trường email, mật khẩu, xác nhận mật khẩu và thông tin cá nhân nếu có.

Hướng dẫn từng bước:

1. Bấm nút `Đăng ký` trên giao diện.
2. Hoặc nhập trực tiếp URL `/register/`.
3. Tại ô `Email`, nhập email đăng ký.
4. Tại ô `Mật khẩu`, nhập mật khẩu mới.
5. Tại ô `Xác nhận mật khẩu`, nhập lại đúng mật khẩu vừa nhập.
6. Nếu form có `Họ tên`, nhập họ tên thật của người dùng.
7. Nếu form có `Số điện thoại`, nhập số điện thoại liên hệ.
8. Kiểm tra lại thông tin đã nhập.
9. Bấm nút `Đăng ký`.
10. Xem thông báo kết quả đăng ký.

Kết quả:

- Hệ thống tạo tài khoản mới ở trạng thái chờ duyệt.
- Tài khoản chưa sử dụng đầy đủ cho đến khi quản trị hệ thống kích hoạt.

Ví dụ:

- Email: `nguyenvana@donvi.mil.vn`
- Mật khẩu: `Abc@123456`
- Họ tên: `Nguyễn Văn A`
- Số điện thoại: `0912345678`

## 2. Tài khoản chờ duyệt

Tài khoản chờ duyệt là tài khoản đã đăng ký thành công nhưng chưa được quản trị hệ thống kích hoạt. Nhóm này chưa được sử dụng các chức năng nội bộ.

### 2.1. Xem kết quả đăng ký

Mục đích:

- Xác nhận việc đăng ký tài khoản đã được hệ thống ghi nhận.

Điều kiện thực hiện:

- Người dùng vừa hoàn thành form đăng ký.

URL thực hiện:

- `/registerresult/{resultId}`

URL chụp ảnh:

- `/registerresult/{resultId}`

Ảnh minh họa:

![Kết quả đăng ký](images/cho-duyet-ket-qua-dang-ky.png)

Cần chụp những gì:

- Chụp thông báo kết quả đăng ký và nội dung yêu cầu chờ quản trị viên phê duyệt nếu có.

Hướng dẫn từng bước:

1. Hoàn thành các bước tại chức năng `Đăng ký tài khoản`.
2. Sau khi bấm `Đăng ký`, đợi hệ thống chuyển sang trang kết quả.
3. Đọc nội dung thông báo trên màn hình.
4. Nếu thông báo yêu cầu chờ phê duyệt, không đăng ký lại nhiều lần.
5. Lưu lại email đã đăng ký để sử dụng sau khi được kích hoạt.

Kết quả:

- Người dùng biết tài khoản đã được tạo và đang chờ phê duyệt.

Ví dụ:

- Sau khi đăng ký email `nguyenvana@donvi.mil.vn`, hệ thống hiển thị thông báo tài khoản cần được quản trị viên phê duyệt trước khi sử dụng.

### 2.2. Đăng nhập kiểm tra trạng thái tài khoản

Mục đích:

- Kiểm tra tài khoản đã được phê duyệt hay chưa.

Điều kiện thực hiện:

- Tài khoản đã đăng ký.
- Tài khoản có thể chưa được kích hoạt.

URL thực hiện:

- `/login/`

URL chụp ảnh:

- `/login/`

Ảnh minh họa:

![Đăng nhập tài khoản chờ duyệt](images/cho-duyet-dang-nhap.png)

Cần chụp những gì:

- Chụp form đăng nhập và thông báo nếu tài khoản chưa được phê duyệt.

Hướng dẫn từng bước:

1. Mở URL `/login/`.
2. Tại ô `Email`, nhập email đã đăng ký.
3. Tại ô `Mật khẩu`, nhập mật khẩu đã tạo.
4. Bấm nút `Đăng nhập`.
5. Đọc thông báo trả về của hệ thống.
6. Nếu hệ thống báo tài khoản chưa được kích hoạt, liên hệ quản trị hệ thống.
7. Sau khi được phê duyệt, thực hiện đăng nhập lại.

Kết quả:

- Nếu tài khoản chưa được duyệt, người dùng không vào được khu vực nội bộ.
- Nếu tài khoản đã được duyệt, hệ thống cho phép đăng nhập.

Ví dụ:

- Email `nguyenvana@donvi.mil.vn` vừa đăng ký trong ngày. Khi đăng nhập, hệ thống thông báo tài khoản chưa được phê duyệt, nên người dùng cần chờ admin kích hoạt.

## 3. Người dùng đã duyệt

Người dùng đã duyệt là tài khoản đã được kích hoạt. Nhóm này có thể đăng nhập, xem hồ sơ cá nhân, sửa thông tin cá nhân, đăng tài liệu và theo dõi tài liệu mình đã gửi.

### 3.1. Đăng nhập hệ thống

Mục đích:

- Truy cập hệ thống bằng tài khoản đã được phê duyệt.

Điều kiện thực hiện:

- Tài khoản đã được kích hoạt.
- Tài khoản không bị khóa.
- Có email và mật khẩu đúng.

URL thực hiện:

- `/login/`

URL chụp ảnh:

- `/login/`

Ảnh minh họa:

![Đăng nhập hệ thống](images/nguoi-dung-dang-nhap.png)

Cần chụp những gì:

- Chụp form đăng nhập trước khi bấm nút đăng nhập và giao diện sau khi đăng nhập thành công.

Hướng dẫn từng bước:

1. Mở URL `/login/`.
2. Tại ô `Email`, nhập email tài khoản.
3. Tại ô `Mật khẩu`, nhập mật khẩu.
4. Nếu muốn trình duyệt ghi nhớ phiên, chọn `Ghi nhớ đăng nhập` nếu form có tùy chọn này.
5. Bấm nút `Đăng nhập`.
6. Nếu thông tin đúng, hệ thống chuyển về trang chủ hoặc trang trước đó.
7. Kiểm tra góc trên giao diện đã hiển thị tên tài khoản hoặc menu tài khoản.

Kết quả:

- Người dùng đăng nhập thành công và có thể sử dụng chức năng nội bộ theo quyền.

Ví dụ:

- Email: `nguyenvana@donvi.mil.vn`
- Mật khẩu: `Abc@123456`
- Sau khi đăng nhập, bấm vào tên tài khoản để vào hồ sơ cá nhân.

### 3.2. Xem hồ sơ cá nhân

Mục đích:

- Xem thông tin cá nhân, thông tin tài khoản, hoạt động gần đây và tài liệu đã đăng.

Điều kiện thực hiện:

- Đã đăng nhập.

URL thực hiện:

- `/customer/profile`

URL chụp ảnh:

- `/customer/profile`

Ảnh minh họa:

![Hồ sơ cá nhân](images/nguoi-dung-ho-so-ca-nhan.png)

Cần chụp những gì:

- Chụp thông tin cá nhân, khu thống kê và tab hoặc danh sách văn bản đã đăng.

Hướng dẫn từng bước:

1. Đăng nhập hệ thống.
2. Bấm vào tên tài khoản hoặc menu tài khoản ở góc trên.
3. Chọn `Hồ sơ cá nhân`.
4. Hệ thống mở trang `/customer/profile`.
5. Xem thông tin họ tên, email, số điện thoại và đơn vị nếu có.
6. Xem số liệu tài liệu đã đăng trong khu thống kê.
7. Cuộn xuống phần danh sách tài liệu đã đăng nếu cần kiểm tra từng tài liệu.

Kết quả:

- Người dùng xem được thông tin tài khoản của mình.
- Người dùng thấy được tài liệu mình đã gửi lên hệ thống.

Ví dụ:

- Người dùng vào hồ sơ cá nhân để kiểm tra tài liệu `Kế hoạch bảo đảm quân nhu quý II` đã được duyệt hay chưa.

### 3.3. Sửa thông tin cá nhân

Mục đích:

- Cập nhật thông tin cá nhân của người dùng.

Điều kiện thực hiện:

- Đã đăng nhập.
- Tài khoản được phép cập nhật thông tin cá nhân.

URL thực hiện:

- `/customer/edit`

URL chụp ảnh:

- `/customer/edit`

Ảnh minh họa:

![Sửa thông tin cá nhân](images/nguoi-dung-sua-ho-so.png)

Cần chụp những gì:

- Chụp form chỉnh sửa thông tin cá nhân và nút lưu.

Hướng dẫn từng bước:

1. Vào `/customer/profile`.
2. Bấm nút `Chỉnh sửa hồ sơ` hoặc `Sửa thông tin`.
3. Hệ thống mở trang `/customer/edit`.
4. Tại ô `Họ tên`, nhập họ tên mới nếu cần sửa.
5. Tại ô `Số điện thoại`, nhập số điện thoại mới nếu cần sửa.
6. Cập nhật các trường khác nếu form hiển thị.
7. Kiểm tra lại thông tin đã nhập.
8. Bấm nút `Lưu`.
9. Quay lại `/customer/profile` để kiểm tra thông tin mới.

Kết quả:

- Thông tin cá nhân được cập nhật theo nội dung vừa nhập.

Ví dụ:

- Sửa số điện thoại từ `0900000000` thành `0912345678`, sau đó bấm `Lưu`.

### 3.4. Đăng tài liệu mới

Mục đích:

- Gửi tài liệu lên kho tài liệu để người có quyền xem xét, phê duyệt và xuất bản.

Điều kiện thực hiện:

- Đã đăng nhập.
- Có file tài liệu hợp lệ.
- Biết thông tin tiêu đề, mã tài liệu, ngày ban hành, danh mục, loại tài liệu và phạm vi truy cập.

URL thực hiện:

- `/tai-lieu/upload`

URL chụp ảnh:

- `/tai-lieu/upload`

Ảnh minh họa:

![Đăng tài liệu mới](images/nguoi-dung-dang-tai-lieu.png)

Cần chụp những gì:

- Chụp form gửi tài liệu với các trường tiêu đề, mã tài liệu, danh mục, loại, phạm vi, chọn file và nút gửi.

Hướng dẫn từng bước:

1. Đăng nhập hệ thống.
2. Mở URL `/tai-lieu/upload`.
3. Tại ô `Tiêu đề`, nhập tên tài liệu.
4. Tại ô `Mã tài liệu`, nhập số ký hiệu hoặc mã quản lý nếu có.
5. Tại ô `Ngày ban hành`, chọn ngày ban hành trên lịch.
6. Tại ô `Tóm tắt`, nhập nội dung tóm tắt ngắn gọn.
7. Tại ô `Danh mục`, chọn nhóm tài liệu phù hợp.
8. Tại ô `Loại tài liệu`, chọn loại như kế hoạch, hướng dẫn, báo cáo hoặc quy định.
9. Tại ô `Cơ quan ban hành`, chọn đơn vị hoặc cơ quan ban hành.
10. Tại ô `Phạm vi truy cập`, chọn `Công khai`, `Nội bộ` hoặc phạm vi phù hợp.
11. Tại phần `Tệp đính kèm`, bấm `Chọn tệp`.
12. Chọn file trên máy tính, ví dụ file PDF hoặc DOCX.
13. Kiểm tra lại toàn bộ thông tin.
14. Bấm nút `Gửi tài liệu`.

Kết quả:

- Tài liệu được gửi lên hệ thống.
- Tài liệu có thể ở trạng thái chờ duyệt trước khi hiển thị công khai.

Ví dụ:

- Tiêu đề: `Kế hoạch bảo đảm quân nhu quý II`
- Mã tài liệu: `KH-QN-2026-02`
- Ngày ban hành: `15/05/2026`
- Danh mục: `Kế hoạch`
- Loại tài liệu: `Văn bản nội bộ`
- Phạm vi truy cập: `Nội bộ`
- Tệp đính kèm: `ke-hoach-quan-nhu-quy-ii.pdf`

### 3.5. Sửa tài liệu đã đăng

Mục đích:

- Cập nhật lại thông tin tài liệu do chính người dùng đã gửi.

Điều kiện thực hiện:

- Đã đăng nhập.
- Tài liệu thuộc người dùng hiện tại hoặc người dùng có quyền sửa.
- Tài liệu chưa bị khóa sửa theo quy trình phê duyệt.

URL thực hiện:

- `/customer/profile`
- `/tai-lieu/cua-toi/sua/{documentId}`

URL chụp ảnh:

- `/customer/profile`
- `/tai-lieu/cua-toi/sua/{documentId}`

Ảnh minh họa:

![Sửa tài liệu đã đăng](images/nguoi-dung-sua-tai-lieu-da-dang.png)

Cần chụp những gì:

- Chụp danh sách tài liệu trong hồ sơ có nút `Sửa`, sau đó chụp form sửa tài liệu.

Hướng dẫn từng bước:

1. Vào `/customer/profile`.
2. Cuộn đến phần `Văn bản đã đăng` hoặc `Tài liệu đã đăng`.
3. Tìm tài liệu cần sửa.
4. Bấm nút `Sửa` tại dòng tài liệu đó.
5. Hệ thống mở trang `/tai-lieu/cua-toi/sua/{documentId}`.
6. Sửa lại `Tiêu đề` nếu tiêu đề chưa đúng.
7. Sửa lại `Tóm tắt` nếu cần bổ sung nội dung.
8. Sửa `Danh mục`, `Loại tài liệu` hoặc `Phạm vi truy cập` nếu cần.
9. Nếu cần thay file, bấm `Chọn tệp` và chọn file mới.
10. Bấm nút `Lưu` hoặc `Cập nhật`.
11. Quay lại hồ sơ cá nhân để kiểm tra thông tin tài liệu.

Kết quả:

- Thông tin tài liệu được cập nhật.
- Nếu quy trình yêu cầu phê duyệt lại, tài liệu có thể quay về trạng thái chờ duyệt.

Ví dụ:

- Tài liệu `Kế hoạch bảo đảm quân nhu quý II` bị sai mã tài liệu. Người dùng bấm `Sửa`, đổi mã từ `KH-QN-2026-2` thành `KH-QN-2026-02`, sau đó bấm `Lưu`.

### 3.6. Xem diễn đàn trao đổi

Mục đích:

- Xem các chủ đề trao đổi trên diễn đàn nếu hệ thống bật chức năng này.

Điều kiện thực hiện:

- Đã đăng nhập nếu diễn đàn yêu cầu đăng nhập.
- Có quyền xem diễn đàn theo cấu hình hệ thống.

URL thực hiện:

- `/boards`

URL chụp ảnh:

- `/boards`

Ảnh minh họa:

![Diễn đàn trao đổi](images/nguoi-dung-dien-dan.png)

Cần chụp những gì:

- Chụp danh sách diễn đàn, chủ đề mới và thanh tìm kiếm nếu có.

Hướng dẫn từng bước:

1. Đăng nhập hệ thống.
2. Mở URL `/boards`.
3. Xem danh sách nhóm trao đổi.
4. Bấm vào một nhóm để xem các chủ đề bên trong.
5. Bấm vào một chủ đề để đọc nội dung trao đổi.
6. Nếu có quyền bình luận, nhập nội dung vào ô trả lời.
7. Bấm `Gửi` để đăng phản hồi.

Kết quả:

- Người dùng xem được nội dung trao đổi phù hợp với quyền truy cập.

Ví dụ:

- Vào `/boards`, mở chủ đề `Trao đổi về bảo đảm lương thực`, đọc các ý kiến và gửi phản hồi nếu được phép.

## 4. Người dùng đơn vị

Người dùng đơn vị là tài khoản đã được gán vào một đơn vị cụ thể. Nhóm này có thể xem thông tin liên quan đến đơn vị của mình và sử dụng các chức năng cá nhân trong bối cảnh đơn vị.

### 4.1. Xem thông tin đơn vị trong hồ sơ

Mục đích:

- Kiểm tra tài khoản hiện đang thuộc đơn vị nào.

Điều kiện thực hiện:

- Đã đăng nhập.
- Tài khoản đã được admin gán vào đơn vị.

URL thực hiện:

- `/customer/profile`

URL chụp ảnh:

- `/customer/profile`

Ảnh minh họa:

![Thông tin đơn vị trong hồ sơ](images/nguoi-dung-don-vi-ho-so.png)

Cần chụp những gì:

- Chụp khu thông tin cá nhân có hiển thị tên đơn vị hoặc thông tin đơn vị công tác.

Hướng dẫn từng bước:

1. Đăng nhập bằng tài khoản đã được gán đơn vị.
2. Mở `/customer/profile`.
3. Tìm khu thông tin cá nhân.
4. Kiểm tra trường `Đơn vị`, `Đơn vị công tác` hoặc thông tin tương đương.
5. Nếu không thấy đơn vị, liên hệ admin để được gán lại đơn vị.
6. Nếu đơn vị hiển thị sai, báo admin cập nhật.

Kết quả:

- Người dùng biết tài khoản của mình đang thuộc đơn vị nào.

Ví dụ:

- Hồ sơ hiển thị `Đơn vị: Lữ đoàn 202`, nghĩa là tài khoản đang thuộc Lữ đoàn 202.

### 4.2. Xem chi tiết đơn vị của mình

Mục đích:

- Xem trang thông tin công khai và nội dung liên quan đến đơn vị.

Điều kiện thực hiện:

- Đơn vị đã tồn tại trên hệ thống.
- Người dùng biết tên hoặc đường dẫn đơn vị.

URL thực hiện:

- `/don-vi/{vendorId}`

URL chụp ảnh:

- `/don-vi/{vendorId}`

Ảnh minh họa:

![Chi tiết đơn vị](images/nguoi-dung-don-vi-chi-tiet.png)

Cần chụp những gì:

- Chụp trang chi tiết đơn vị, thông tin giới thiệu, sản phẩm/năng lực và tin liên quan nếu có.

Hướng dẫn từng bước:

1. Mở `/don-vi`.
2. Tại ô tìm kiếm đơn vị, nhập tên đơn vị của mình.
3. Bấm `Tìm kiếm`.
4. Trong danh sách kết quả, bấm vào tên đơn vị.
5. Hệ thống mở trang `/don-vi/{vendorId}`.
6. Xem thông tin giới thiệu, địa chỉ, liên hệ nếu có.
7. Xem các sản phẩm/năng lực gắn với đơn vị.
8. Xem tin tức hoặc nội dung liên quan đến đơn vị nếu hệ thống hiển thị.

Kết quả:

- Người dùng xem được trang thông tin của đơn vị mình.

Ví dụ:

- Người dùng thuộc `Lữ đoàn 202`, vào `/don-vi`, tìm `Lữ đoàn 202`, bấm vào kết quả để xem năng lực hậu cần của đơn vị.

### 4.3. Tìm kiếm tài liệu và tin tức liên quan đến đơn vị

Mục đích:

- Tìm nhanh nội dung có liên quan đến đơn vị đang công tác.

Điều kiện thực hiện:

- Đã đăng nhập nếu cần xem nội dung nội bộ.
- Từ khóa tìm kiếm nên là tên đơn vị, mã đơn vị hoặc từ khóa nghiệp vụ.

URL thực hiện:

- `/search/`
- `/blog/news`
- `/blog/documents`
- `/tai-lieu`

URL chụp ảnh:

- `/search/?q=Lữ%20đoàn%20202`

Ảnh minh họa:

![Tìm nội dung theo đơn vị](images/nguoi-dung-don-vi-tim-noi-dung.png)

Cần chụp những gì:

- Chụp ô tìm kiếm có từ khóa đơn vị và danh sách kết quả trả về.

Hướng dẫn từng bước:

1. Mở trang `/search/`.
2. Tại ô tìm kiếm, nhập tên đơn vị, ví dụ `Lữ đoàn 202`.
3. Bấm `Tìm kiếm`.
4. Xem các kết quả thuộc nhóm `Tin tức`, `Văn bản`, `Tài liệu` và `Đơn vị`.
5. Nếu chỉ muốn xem tin, bấm nhóm `Tin tức`.
6. Nếu chỉ muốn xem văn bản, bấm nhóm `Văn bản`.
7. Bấm vào kết quả cần xem để mở chi tiết.

Kết quả:

- Hệ thống hiển thị các nội dung liên quan đến đơn vị.

Ví dụ:

- Nhập `Lữ đoàn 202`, hệ thống trả về tin hoạt động, văn bản và tài liệu có nội dung liên quan đến Lữ đoàn 202.

## 5. Nhân viên đơn vị

Nhân viên đơn vị là tài khoản thuộc một đơn vị và đã được cấp quyền nghiệp vụ. Nhóm này chỉ thao tác trong phạm vi quyền được giao, thường là tài liệu, báo cáo, tin tức, sản phẩm/năng lực hoặc dữ liệu thuộc đơn vị hiện tại.

### 5.1. Truy cập khu vực quản trị theo quyền

Mục đích:

- Vào khu vực quản trị để thực hiện các nhiệm vụ được giao.

Điều kiện thực hiện:

- Đã đăng nhập.
- Tài khoản được cấp quyền vào `/Admin`.
- Tài khoản thuộc đơn vị hiện tại.

URL thực hiện:

- `/Admin`

URL chụp ảnh:

- `/Admin`

Ảnh minh họa:

![Admin nhân viên đơn vị](images/nhan-vien-don-vi-admin.png)

Cần chụp những gì:

- Chụp dashboard admin sau khi nhân viên đơn vị đăng nhập, đặc biệt là các menu mà tài khoản được thấy.

Hướng dẫn từng bước:

1. Đăng nhập bằng tài khoản nhân viên đơn vị.
2. Nhập `/Admin` trên thanh địa chỉ.
3. Hệ thống kiểm tra quyền truy cập.
4. Nếu có quyền, dashboard quản trị được hiển thị.
5. Quan sát menu bên trái để xem các chức năng được cấp.
6. Nếu bị từ chối truy cập, liên hệ trưởng đơn vị hoặc admin để được cấp quyền.

Kết quả:

- Nhân viên đơn vị vào được khu vực quản trị theo quyền.
- Các menu không có quyền sẽ không hiển thị hoặc không truy cập được.

Ví dụ:

- Nhân viên phụ trách tài liệu vào `/Admin`, thấy menu `Kho tài liệu` và `Báo cáo đơn vị`, nhưng không thấy menu cấu hình hệ thống.

### 5.2. Quản lý tài liệu đơn vị

Mục đích:

- Tạo, tìm kiếm, cập nhật và theo dõi tài liệu trong phạm vi đơn vị được giao.

Điều kiện thực hiện:

- Đã đăng nhập vào admin.
- Có quyền quản lý tài liệu.
- Dữ liệu thao tác thuộc phạm vi đơn vị hiện tại hoặc phạm vi được cấp.

URL thực hiện:

- `/Admin/DocumentPortalAdmin/List`

URL chụp ảnh:

- `/Admin/DocumentPortalAdmin/List`

Ảnh minh họa:

![Quản lý tài liệu đơn vị](images/nhan-vien-don-vi-quan-ly-tai-lieu.png)

Cần chụp những gì:

- Chụp bộ lọc danh sách tài liệu, bảng tài liệu, cột trạng thái, cột lượt xem/tải và nút sửa.

Hướng dẫn từng bước:

1. Vào `/Admin`.
2. Chọn menu `Kho tài liệu` hoặc mở trực tiếp `/Admin/DocumentPortalAdmin/List`.
3. Tại ô `Tiêu đề`, nhập từ khóa cần tìm nếu có.
4. Tại ô `Mã tài liệu`, nhập mã tài liệu nếu cần lọc chính xác.
5. Chọn `Danh mục` nếu muốn lọc theo nhóm tài liệu.
6. Chọn `Loại tài liệu` nếu muốn lọc theo loại.
7. Chọn `Cơ quan ban hành` nếu cần.
8. Chọn `Trạng thái` để lọc tài liệu đã xuất bản hoặc chưa xuất bản.
9. Bấm `Tìm kiếm`.
10. Xem danh sách kết quả trong bảng.
11. Bấm `Sửa` tại tài liệu cần cập nhật.
12. Cập nhật thông tin cần sửa.
13. Bấm `Lưu`.

Kết quả:

- Nhân viên đơn vị quản lý được tài liệu thuộc phạm vi được giao.

Ví dụ:

- Nhập `TL-DEMO-001` vào ô `Mã tài liệu`, bấm `Tìm kiếm`, mở tài liệu tìm thấy và sửa `Phạm vi truy cập` thành `Nội bộ`.

### 5.3. Thêm tài liệu trong admin

Mục đích:

- Tạo tài liệu mới từ khu vực admin, phục vụ quản lý tập trung của đơn vị.

Điều kiện thực hiện:

- Có quyền tạo tài liệu trong admin.
- Có file tài liệu cần đăng.

URL thực hiện:

- `/Admin/DocumentPortalAdmin/Create`

URL chụp ảnh:

- `/Admin/DocumentPortalAdmin/Create`

Ảnh minh họa:

![Thêm tài liệu admin](images/nhan-vien-don-vi-them-tai-lieu-admin.png)

Cần chụp những gì:

- Chụp form tạo tài liệu trong admin với các trường nhập và nút lưu.

Hướng dẫn từng bước:

1. Vào `/Admin/DocumentPortalAdmin/List`.
2. Bấm nút `Thêm mới`.
3. Tại ô `Tiêu đề`, nhập tên tài liệu.
4. Tại ô `Mã tài liệu`, nhập mã hoặc số ký hiệu.
5. Chọn `Ngày ban hành`.
6. Nhập `Tóm tắt`.
7. Chọn `Danh mục`.
8. Chọn `Loại tài liệu`.
9. Chọn `Cơ quan ban hành`.
10. Chọn `Phạm vi truy cập`.
11. Chọn file tài liệu tại phần tệp đính kèm.
12. Nếu có tùy chọn `Published`, bật nếu muốn xuất bản ngay.
13. Bấm `Lưu`.

Kết quả:

- Tài liệu mới được tạo trong hệ thống.
- Tài liệu hiển thị theo trạng thái xuất bản và phạm vi truy cập đã chọn.

Ví dụ:

- Tiêu đề: `Quy trình cấp phát vật chất hậu cần`
- Mã tài liệu: `QT-HC-2026-01`
- Phạm vi truy cập: `Nội bộ`
- Trạng thái: `Published`

### 5.4. Nhập báo cáo đơn vị

Mục đích:

- Nhập số liệu báo cáo theo kỳ cho đơn vị hiện tại.

Điều kiện thực hiện:

- Tài khoản có quyền báo cáo đơn vị.
- Kỳ báo cáo đã được tạo.
- Báo cáo chưa bị khóa.

URL thực hiện:

- `/Admin/UnitReportAdmin/Index`

URL chụp ảnh:

- `/Admin/UnitReportAdmin/Index`
- `/Admin/UnitReportAdmin/Edit/{reportId}`

Ảnh minh họa:

![Nhập báo cáo đơn vị](images/nhan-vien-don-vi-nhap-bao-cao.png)

Cần chụp những gì:

- Chụp danh sách báo cáo theo kỳ và màn hình nhập chỉ tiêu báo cáo.

Hướng dẫn từng bước:

1. Vào `/Admin/UnitReportAdmin/Index`.
2. Tại bộ lọc `Kỳ báo cáo`, chọn kỳ cần nhập.
3. Bấm `Tìm kiếm` nếu danh sách không tự cập nhật.
4. Tìm dòng báo cáo của đơn vị mình.
5. Bấm nút `Mở`, `Sửa` hoặc `Chi tiết`.
6. Hệ thống mở trang `/Admin/UnitReportAdmin/Edit/{reportId}`.
7. Đọc tên từng chỉ tiêu trong bảng nhập liệu.
8. Tại cột giá trị, nhập số liệu cho từng chỉ tiêu.
9. Nếu có cột ghi chú, nhập giải thích nếu số liệu bất thường.
10. Bấm `Lưu` để lưu tạm.
11. Kiểm tra lại toàn bộ số liệu.
12. Bấm `Gửi báo cáo` nếu đã hoàn thành.

Kết quả:

- Số liệu báo cáo của đơn vị được lưu.
- Nếu bấm gửi, báo cáo chuyển sang trạng thái đã gửi theo quy trình.

Ví dụ:

- Kỳ báo cáo: `Tháng 05/2026`
- Chỉ tiêu: `Lương thực tồn kho`
- Giá trị nhập: `1250`
- Ghi chú: `Đã bao gồm lương thực dự trữ tại kho phụ`

### 5.5. Quản lý tin tức hoặc văn bản của đơn vị nếu được cấp quyền

Mục đích:

- Tạo và cập nhật tin tức, văn bản liên quan đến đơn vị.

Điều kiện thực hiện:

- Có quyền quản lý bài viết.
- Nội dung bài viết thuộc phạm vi đơn vị hoặc nhiệm vụ được giao.

URL thực hiện:

- `/Admin/Blog/List`
- `/Admin/Blog/Create`

URL chụp ảnh:

- `/Admin/Blog/List`
- `/Admin/Blog/Create`

Ảnh minh họa:

![Quản lý tin tức đơn vị](images/nhan-vien-don-vi-quan-ly-tin-tuc.png)

Cần chụp những gì:

- Chụp danh sách bài viết admin và form tạo bài viết mới.

Hướng dẫn từng bước:

1. Vào `/Admin/Blog/List`.
2. Bấm `Thêm mới` để tạo bài viết.
3. Tại ô `Tiêu đề`, nhập tiêu đề bài viết.
4. Tại ô `Tóm tắt`, nhập nội dung tóm tắt hiển thị ở danh sách.
5. Tại ô `Nội dung`, nhập nội dung chi tiết.
6. Chọn ảnh đại diện nếu form có trường ảnh.
7. Chọn loại bài viết là `Tin tức` hoặc `Văn bản` theo nhu cầu.
8. Gán đơn vị đăng tin nếu form có trường đơn vị.
9. Bật `Published` nếu được phép xuất bản.
10. Bấm `Lưu`.
11. Mở `/blog/news` hoặc `/blog/documents` để kiểm tra bài viết.

Kết quả:

- Bài viết được tạo hoặc cập nhật theo quyền của nhân viên đơn vị.

Ví dụ:

- Tiêu đề: `Lữ đoàn 202 kiểm tra công tác bảo đảm quân nhu tháng 5`
- Loại bài viết: `Tin tức`
- Tóm tắt: `Đơn vị tổ chức kiểm tra công tác bảo đảm quân nhu tại các bộ phận trực thuộc.`

### 5.6. Quản lý sản phẩm hoặc năng lực của đơn vị nếu được cấp quyền

Mục đích:

- Cập nhật các sản phẩm, năng lực, hạng mục hoặc thông tin giới thiệu liên quan đến đơn vị.

Điều kiện thực hiện:

- Có quyền quản lý sản phẩm.
- Sản phẩm thuộc đơn vị hiện tại hoặc phạm vi được giao.

URL thực hiện:

- `/Admin/Product/List`
- `/Admin/Product/Create`

URL chụp ảnh:

- `/Admin/Product/List`
- `/Admin/Product/Create`

Ảnh minh họa:

![Quản lý sản phẩm đơn vị](images/nhan-vien-don-vi-quan-ly-san-pham.png)

Cần chụp những gì:

- Chụp danh sách sản phẩm admin và form thêm/sửa sản phẩm.

Hướng dẫn từng bước:

1. Vào `/Admin/Product/List`.
2. Dùng bộ lọc để tìm sản phẩm của đơn vị nếu cần.
3. Bấm `Thêm mới` nếu cần tạo sản phẩm hoặc năng lực mới.
4. Tại ô `Tên sản phẩm`, nhập tên sản phẩm hoặc năng lực.
5. Chọn `Danh mục` phù hợp.
6. Chọn đơn vị quản lý nếu form có trường đơn vị.
7. Nhập `Mô tả ngắn`.
8. Nhập `Mô tả chi tiết`.
9. Tải ảnh đại diện hoặc ảnh minh họa nếu có.
10. Bật `Published` nếu muốn hiển thị công khai.
11. Bấm `Lưu`.

Kết quả:

- Sản phẩm hoặc năng lực được tạo hoặc cập nhật.
- Nội dung hiển thị ngoài public nếu được xuất bản.

Ví dụ:

- Tên sản phẩm/năng lực: `Suất ăn dã ngoại`
- Danh mục: `Quân nhu`
- Mô tả ngắn: `Phục vụ bảo đảm ăn uống cơ động cho đơn vị huấn luyện dài ngày.`

## 6. Trưởng đơn vị

Trưởng đơn vị là tài khoản phụ trách một đơn vị. Nhóm này theo dõi dữ liệu của đơn vị hiện tại, phân công quyền cho nhân viên đơn vị và kiểm tra tình hình báo cáo của đơn vị.

### 6.1. Xem dashboard quản trị của đơn vị

Mục đích:

- Xem tổng quan dữ liệu và tình hình công việc của đơn vị hiện tại.

Điều kiện thực hiện:

- Tài khoản là trưởng đơn vị.
- Có quyền truy cập admin.
- Dashboard chỉ tính dữ liệu của đơn vị hiện tại theo cấu hình đã chốt.

URL thực hiện:

- `/Admin`

URL chụp ảnh:

- `/Admin`

Ảnh minh họa:

![Dashboard trưởng đơn vị](images/truong-don-vi-dashboard.png)

Cần chụp những gì:

- Chụp các thẻ thống kê tổng quan và menu quản trị mà trưởng đơn vị được phép dùng.

Hướng dẫn từng bước:

1. Đăng nhập bằng tài khoản trưởng đơn vị.
2. Mở URL `/Admin`.
3. Xem các chỉ số thống kê trên dashboard.
4. Kiểm tra các số liệu tài liệu, tin tức, sản phẩm hoặc báo cáo nếu dashboard hiển thị.
5. Lưu ý số liệu chỉ tính trong đơn vị hiện tại, không cộng đơn vị trực thuộc nếu hệ thống được cấu hình như vậy.
6. Bấm vào một thẻ thống kê nếu thẻ đó có liên kết đến danh sách chi tiết.

Kết quả:

- Trưởng đơn vị nắm được tình hình tổng quan của đơn vị.

Ví dụ:

- Trưởng đơn vị vào `/Admin` và thấy đơn vị có `12 tài liệu`, `4 tin tức`, `2 báo cáo đang chờ hoàn thiện` trong phạm vi đơn vị hiện tại.

### 6.2. Cấp quyền nhân viên đơn vị

Mục đích:

- Gán quyền nghiệp vụ cho người dùng thuộc đơn vị để hỗ trợ quản lý dữ liệu.

Điều kiện thực hiện:

- Tài khoản là trưởng đơn vị.
- Người được cấp quyền đã thuộc đơn vị hiện tại.

URL thực hiện:

- `/Admin/VendorStaffAccess/Index`

URL chụp ảnh:

- `/Admin/VendorStaffAccess/Index`

Ảnh minh họa:

![Cấp quyền nhân viên đơn vị](images/truong-don-vi-cap-quyen-nhan-vien.png)

Cần chụp những gì:

- Chụp danh sách người dùng đơn vị và các tùy chọn bật/tắt quyền.

Hướng dẫn từng bước:

1. Đăng nhập bằng tài khoản trưởng đơn vị.
2. Vào `/Admin/VendorStaffAccess/Index`.
3. Xem danh sách người dùng thuộc đơn vị.
4. Tìm người dùng cần cấp quyền bằng tên hoặc email.
5. Chọn dòng người dùng đó.
6. Bật quyền phù hợp, ví dụ quyền quản lý tài liệu hoặc quyền nhập báo cáo.
7. Kiểm tra lại các quyền đã chọn.
8. Bấm `Lưu` hoặc nút cập nhật tương ứng.
9. Yêu cầu nhân viên đăng xuất và đăng nhập lại để nhận quyền mới nếu cần.

Kết quả:

- Nhân viên được cấp quyền nghiệp vụ trong phạm vi đơn vị.

Ví dụ:

- Trưởng đơn vị cấp quyền `Nhập báo cáo đơn vị` cho tài khoản `tranvanb@donvi.mil.vn` để nhân viên B nhập số liệu hằng tháng.

### 6.3. Theo dõi và kiểm tra báo cáo đơn vị

Mục đích:

- Kiểm tra trạng thái và số liệu báo cáo của đơn vị theo từng kỳ.

Điều kiện thực hiện:

- Tài khoản là trưởng đơn vị.
- Có báo cáo theo kỳ đã được tạo.

URL thực hiện:

- `/Admin/UnitReportAdmin/Index`

URL chụp ảnh:

- `/Admin/UnitReportAdmin/Index`

Ảnh minh họa:

![Theo dõi báo cáo đơn vị](images/truong-don-vi-theo-doi-bao-cao.png)

Cần chụp những gì:

- Chụp danh sách báo cáo, bộ lọc kỳ báo cáo và cột trạng thái.

Hướng dẫn từng bước:

1. Vào `/Admin/UnitReportAdmin/Index`.
2. Chọn kỳ báo cáo cần xem.
3. Bấm `Tìm kiếm`.
4. Xem dòng báo cáo của đơn vị hiện tại.
5. Kiểm tra cột `Trạng thái`, ví dụ nháp, đã gửi, trả lại hoặc đã khóa.
6. Bấm `Chi tiết` hoặc `Sửa` để mở nội dung báo cáo.
7. Kiểm tra từng chỉ tiêu và ghi chú.
8. Nếu dữ liệu thiếu, yêu cầu nhân viên bổ sung.
9. Nếu dữ liệu đúng, thực hiện gửi hoặc xác nhận theo nút có sẵn trên màn hình.

Kết quả:

- Trưởng đơn vị nắm được báo cáo đã hoàn thành hay chưa.

Ví dụ:

- Kỳ `Tháng 05/2026` đang ở trạng thái `Draft`. Trưởng đơn vị mở chi tiết, thấy chỉ tiêu `Nhiên liệu tồn kho` chưa nhập, yêu cầu nhân viên phụ trách cập nhật.

### 6.4. Kiểm tra tài liệu của đơn vị

Mục đích:

- Giám sát các tài liệu của đơn vị đã đăng, đã xuất bản hoặc đang chờ xử lý.

Điều kiện thực hiện:

- Tài khoản là trưởng đơn vị.
- Có quyền xem danh sách tài liệu admin.

URL thực hiện:

- `/Admin/DocumentPortalAdmin/List`

URL chụp ảnh:

- `/Admin/DocumentPortalAdmin/List`

Ảnh minh họa:

![Kiểm tra tài liệu đơn vị](images/truong-don-vi-kiem-tra-tai-lieu.png)

Cần chụp những gì:

- Chụp danh sách tài liệu, bộ lọc trạng thái và cột người tạo/đơn vị nếu có.

Hướng dẫn từng bước:

1. Vào `/Admin/DocumentPortalAdmin/List`.
2. Tại bộ lọc `Trạng thái`, chọn trạng thái cần kiểm tra.
3. Nếu cần tìm tài liệu cụ thể, nhập tiêu đề hoặc mã tài liệu.
4. Bấm `Tìm kiếm`.
5. Xem danh sách tài liệu trong bảng.
6. Bấm `Sửa` hoặc `Chi tiết` để xem nội dung.
7. Kiểm tra file, phạm vi truy cập và trạng thái xuất bản.
8. Nếu cần, yêu cầu nhân viên cập nhật thông tin.

Kết quả:

- Trưởng đơn vị nắm được tình trạng tài liệu của đơn vị.

Ví dụ:

- Lọc `Trạng thái: Chưa xuất bản`, thấy tài liệu `Kế hoạch tiếp nhận vật chất`, trưởng đơn vị yêu cầu nhân viên bổ sung file đính kèm trước khi xuất bản.

### 6.5. Kiểm tra thông tin public của đơn vị

Mục đích:

- Đảm bảo trang đơn vị hiển thị đúng thông tin ra bên ngoài.

Điều kiện thực hiện:

- Đơn vị đã được tạo.
- Trưởng đơn vị biết URL chi tiết đơn vị.

URL thực hiện:

- `/don-vi/{vendorId}`

URL chụp ảnh:

- `/don-vi/{vendorId}`

Ảnh minh họa:

![Kiểm tra public đơn vị](images/truong-don-vi-kiem-tra-public-don-vi.png)

Cần chụp những gì:

- Chụp phần giới thiệu đơn vị, thông tin liên hệ và danh sách sản phẩm/năng lực.

Hướng dẫn từng bước:

1. Mở `/don-vi`.
2. Tìm đơn vị của mình.
3. Bấm vào trang chi tiết đơn vị.
4. Kiểm tra tên đơn vị, mô tả, địa chỉ, liên hệ và ảnh đại diện nếu có.
5. Kiểm tra các sản phẩm/năng lực đang hiển thị.
6. Kiểm tra tin tức liên quan đến đơn vị nếu có.
7. Nếu thông tin sai, ghi lại nội dung cần sửa và cập nhật trong admin nếu có quyền, hoặc báo admin.

Kết quả:

- Trang public của đơn vị được kiểm tra và có danh sách nội dung cần cập nhật nếu sai.

Ví dụ:

- Trưởng đơn vị thấy số điện thoại liên hệ trên trang `Lữ đoàn 202` đã cũ, ghi lại số mới `0912345678` để yêu cầu cập nhật.

## 7. Quản trị hệ thống

Quản trị hệ thống là cấp tài khoản có quyền quản lý tổng thể. Quản trị viên phê duyệt tài khoản, gán đơn vị, quản lý đơn vị, sản phẩm, tin tức, văn bản, tài liệu, báo cáo và cấu hình hệ thống.

### 7.1. Xem dashboard tổng hợp

Mục đích:

- Theo dõi nhanh tình hình dữ liệu toàn hệ thống hoặc theo phạm vi quyền của tài khoản.

Điều kiện thực hiện:

- Đăng nhập bằng tài khoản quản trị.
- Có quyền truy cập `/Admin`.

URL thực hiện:

- `/Admin`

URL chụp ảnh:

- `/Admin`

Ảnh minh họa:

![Dashboard quản trị hệ thống](images/admin-dashboard-tong-hop.png)

Cần chụp những gì:

- Chụp các thẻ thống kê tổng hợp, biểu đồ nếu có và menu quản trị bên trái.

Hướng dẫn từng bước:

1. Đăng nhập bằng tài khoản admin.
2. Mở URL `/Admin`.
3. Xem các thẻ thống kê tổng quan.
4. Kiểm tra số lượng người dùng, đơn vị, sản phẩm, tin tức, tài liệu hoặc báo cáo nếu dashboard hiển thị.
5. Bấm vào thẻ thống kê nếu cần mở danh sách chi tiết.
6. Đối với tài khoản admin toàn hệ thống, số liệu có thể tính toàn bộ dữ liệu.
7. Đối với tài khoản đơn vị, số liệu chỉ tính đơn vị hiện tại theo quy định.

Kết quả:

- Admin nắm được tổng quan tình hình hệ thống.

Ví dụ:

- Admin vào `/Admin`, thấy có `156 người dùng`, `24 đơn vị`, `83 tài liệu`, `32 tin tức` và bấm vào thẻ `Tài liệu` để kiểm tra danh sách tài liệu.

### 7.2. Phê duyệt tài khoản mới

Mục đích:

- Kích hoạt tài khoản mới đăng ký để người dùng có thể đăng nhập và sử dụng hệ thống.

Điều kiện thực hiện:

- Có tài khoản admin.
- Có người dùng mới đăng ký và đang chờ duyệt.

URL thực hiện:

- `/Admin/Customer/List`
- `/Admin/Customer/Edit/{customerId}`

URL chụp ảnh:

- `/Admin/Customer/List`
- `/Admin/Customer/Edit/{customerId}`

Ảnh minh họa:

![Phê duyệt tài khoản mới](images/admin-phe-duyet-tai-khoan.png)

Cần chụp những gì:

- Chụp danh sách người dùng, bộ lọc tìm email và trang chi tiết có trường active/quyền.

Hướng dẫn từng bước:

1. Đăng nhập admin.
2. Vào `/Admin/Customer/List`.
3. Tại ô tìm kiếm email, nhập email người dùng mới đăng ký.
4. Bấm `Tìm kiếm`.
5. Tìm đúng dòng tài khoản cần duyệt.
6. Bấm `Sửa`.
7. Kiểm tra email, họ tên và thông tin liên hệ.
8. Bật trạng thái `Active` hoặc bỏ tùy chọn khóa tài khoản nếu có.
9. Gán vai trò/quyền phù hợp nếu cần.
10. Gán đơn vị cho người dùng nếu người dùng thuộc một đơn vị.
11. Bấm `Lưu`.
12. Thông báo cho người dùng đăng nhập lại.

Kết quả:

- Tài khoản mới được kích hoạt.
- Người dùng có thể đăng nhập bằng email và mật khẩu đã đăng ký.

Ví dụ:

- Admin tìm email `nguyenvana@donvi.mil.vn`, mở chi tiết, bật `Active`, gán đơn vị `Lữ đoàn 202`, sau đó bấm `Lưu`.

### 7.3. Gán người dùng vào đơn vị

Mục đích:

- Xác định người dùng thuộc đơn vị nào để hệ thống áp dụng phạm vi dữ liệu và quyền thao tác.

Điều kiện thực hiện:

- Người dùng đã có tài khoản.
- Đơn vị đã tồn tại trong hệ thống.
- Admin có quyền sửa người dùng.

URL thực hiện:

- `/Admin/Customer/Edit/{customerId}`

URL chụp ảnh:

- `/Admin/Customer/Edit/{customerId}`

Ảnh minh họa:

![Gán người dùng vào đơn vị](images/admin-gan-nguoi-dung-vao-don-vi.png)

Cần chụp những gì:

- Chụp khu vực thông tin đơn vị trong trang sửa người dùng.

Hướng dẫn từng bước:

1. Vào `/Admin/Customer/List`.
2. Tìm người dùng cần gán đơn vị bằng email hoặc họ tên.
3. Bấm `Sửa` tại dòng người dùng đó.
4. Trong trang chi tiết, tìm trường `Đơn vị`, `Đơn vị công tác` hoặc trường tương đương.
5. Chọn đơn vị phù hợp từ danh sách.
6. Nếu có trường vai trò, chọn vai trò người dùng phù hợp.
7. Kiểm tra lại thông tin.
8. Bấm `Lưu`.
9. Mở hồ sơ người dùng hoặc yêu cầu người dùng đăng nhập để kiểm tra đơn vị đã gán.

Kết quả:

- Người dùng được gán vào đơn vị.
- Người dùng trở thành người dùng đơn vị trong hệ thống.

Ví dụ:

- Gán `tranvanb@donvi.mil.vn` vào `Lữ đoàn 201` để tài khoản chỉ làm việc với dữ liệu của Lữ đoàn 201 theo quyền được cấp.

### 7.4. Quản lý đơn vị

Mục đích:

- Tạo mới, cập nhật và kiểm tra thông tin đơn vị trên hệ thống.

Điều kiện thực hiện:

- Admin có quyền quản lý đơn vị.

URL thực hiện:

- `/Admin/Vendor/List`
- `/Admin/Vendor/Edit/{vendorId}`

URL chụp ảnh:

- `/Admin/Vendor/List`
- `/Admin/Vendor/Edit/{vendorId}`

Ảnh minh họa:

![Quản lý đơn vị](images/admin-quan-ly-don-vi.png)

Cần chụp những gì:

- Chụp danh sách đơn vị và form sửa đơn vị.

Hướng dẫn từng bước:

1. Vào `/Admin/Vendor/List`.
2. Dùng bộ lọc tên đơn vị để tìm đơn vị cần sửa.
3. Bấm `Tìm kiếm`.
4. Bấm `Sửa` tại dòng đơn vị cần cập nhật.
5. Cập nhật `Tên đơn vị` nếu cần.
6. Cập nhật `Mô tả` hoặc thông tin giới thiệu.
7. Cập nhật địa chỉ, email, số điện thoại nếu form có các trường này.
8. Cập nhật ảnh đại diện/logo nếu cần.
9. Kiểm tra trạng thái hiển thị của đơn vị.
10. Bấm `Lưu`.
11. Mở `/don-vi/{vendorId}` để kiểm tra hiển thị public.

Kết quả:

- Thông tin đơn vị được tạo mới hoặc cập nhật.

Ví dụ:

- Sửa mô tả của `Lữ đoàn 202` thành `Đơn vị bảo đảm hậu cần, kỹ thuật phục vụ nhiệm vụ huấn luyện và sẵn sàng chiến đấu`.

### 7.5. Gán trưởng đơn vị

Mục đích:

- Gán một tài khoản làm người phụ trách/trưởng đơn vị để quản lý nhân viên và dữ liệu của đơn vị.

Điều kiện thực hiện:

- Đơn vị đã tồn tại.
- Tài khoản người phụ trách đã tồn tại và được kích hoạt.
- Admin có quyền sửa đơn vị hoặc quyền gán phụ trách.

URL thực hiện:

- `/Admin/Vendor/List`
- `/Admin/Vendor/Edit/{vendorId}`

URL chụp ảnh:

- `/Admin/Vendor/Edit/{vendorId}`

Ảnh minh họa:

![Gán trưởng đơn vị](images/admin-gan-truong-don-vi.png)

Cần chụp những gì:

- Chụp trường chọn người phụ trách/trưởng đơn vị trong form sửa đơn vị.

Hướng dẫn từng bước:

1. Vào `/Admin/Vendor/List`.
2. Tìm đơn vị cần gán trưởng.
3. Bấm `Sửa`.
4. Tìm trường `Người phụ trách`, `Trưởng đơn vị` hoặc trường tương đương.
5. Chọn tài khoản phụ trách trong danh sách.
6. Nếu cần, kiểm tra tài khoản đó đã thuộc đơn vị này chưa.
7. Kiểm tra lại thông tin.
8. Bấm `Lưu`.
9. Đăng nhập bằng tài khoản trưởng đơn vị để kiểm tra quyền.

Kết quả:

- Tài khoản được gán có vai trò trưởng đơn vị.

Ví dụ:

- Admin gán tài khoản `tran.hieu@donvi.mil.vn` làm trưởng đơn vị `Lữ đoàn 202`.

### 7.6. Quản lý sản phẩm hoặc năng lực

Mục đích:

- Tạo và cập nhật các sản phẩm, năng lực hoặc hạng mục được hiển thị trên cổng thông tin.

Điều kiện thực hiện:

- Admin có quyền quản lý sản phẩm.

URL thực hiện:

- `/Admin/Product/List`
- `/Admin/Product/Create`

URL chụp ảnh:

- `/Admin/Product/List`
- `/Admin/Product/Create`

Ảnh minh họa:

![Quản lý sản phẩm năng lực](images/admin-quan-ly-san-pham.png)

Cần chụp những gì:

- Chụp danh sách sản phẩm, nút thêm mới và form tạo sản phẩm.

Hướng dẫn từng bước:

1. Vào `/Admin/Product/List`.
2. Nếu cần sửa sản phẩm cũ, nhập tên sản phẩm vào bộ lọc và bấm `Tìm kiếm`.
3. Nếu cần thêm mới, bấm `Thêm mới`.
4. Tại ô `Tên sản phẩm`, nhập tên sản phẩm/năng lực.
5. Chọn `Danh mục`.
6. Chọn đơn vị quản lý hoặc đơn vị cung cấp nếu form có trường này.
7. Nhập `Mô tả ngắn` để hiển thị ở danh sách.
8. Nhập `Mô tả chi tiết` để hiển thị trong trang chi tiết.
9. Tải ảnh đại diện.
10. Cấu hình giá, tồn kho hoặc thuộc tính khác nếu hệ thống yêu cầu.
11. Bật `Published` nếu muốn hiển thị.
12. Bấm `Lưu`.

Kết quả:

- Sản phẩm hoặc năng lực được tạo hoặc cập nhật trong hệ thống.

Ví dụ:

- Tên sản phẩm: `Suất ăn dã ngoại`
- Danh mục: `Quân nhu`
- Đơn vị: `Lữ đoàn 202`
- Mô tả ngắn: `Suất ăn phục vụ huấn luyện dã ngoại và cơ động dài ngày.`

### 7.7. Quản lý tin tức và văn bản đăng trên cổng thông tin

Mục đích:

- Tạo, sửa, xuất bản và kiểm tra các bài tin tức hoặc văn bản trên hệ thống.

Điều kiện thực hiện:

- Admin có quyền quản lý bài viết.

URL thực hiện:

- `/Admin/Blog/List`
- `/Admin/Blog/Create`

URL chụp ảnh:

- `/Admin/Blog/List`
- `/Admin/Blog/Create`

Ảnh minh họa:

![Quản lý tin tức văn bản](images/admin-quan-ly-tin-tuc-van-ban.png)

Cần chụp những gì:

- Chụp danh sách bài viết, bộ lọc, form tạo bài viết và trường chọn loại bài.

Hướng dẫn từng bước:

1. Vào `/Admin/Blog/List`.
2. Dùng bộ lọc để tìm bài viết cũ nếu cần sửa.
3. Bấm `Thêm mới` nếu cần tạo bài viết mới.
4. Chọn loại bài viết là `Tin tức` nếu đây là bài tin.
5. Chọn loại bài viết là `Văn bản` nếu đây là bài đăng dạng văn bản.
6. Tại ô `Tiêu đề`, nhập tiêu đề bài viết.
7. Tại ô `Tóm tắt`, nhập tóm tắt ngắn gọn.
8. Tại ô `Nội dung`, nhập nội dung chi tiết.
9. Chọn ảnh đại diện nếu có.
10. Gán đơn vị đăng tin nếu form có trường đơn vị.
11. Bật `Published` nếu muốn xuất bản.
12. Bấm `Lưu`.
13. Mở `/blog/news` hoặc `/blog/documents` để kiểm tra hiển thị.

Kết quả:

- Bài viết được lưu và hiển thị theo loại đã chọn.

Ví dụ:

- Tạo tin tức với tiêu đề `Tổng kết công tác bảo đảm hậu cần tháng 5`, chọn loại `Tin tức`, bật `Published`, sau đó kiểm tra tại `/blog/news`.

### 7.8. Duyệt và xuất bản tài liệu

Mục đích:

- Kiểm tra tài liệu do người dùng gửi lên và xuất bản nếu hợp lệ.

Điều kiện thực hiện:

- Admin có quyền quản lý tài liệu.
- Có tài liệu mới gửi lên hoặc đang chờ duyệt.

URL thực hiện:

- `/Admin/DocumentPortalAdmin/List`
- `/Admin/DocumentPortalAdmin/Edit/{documentId}`

URL chụp ảnh:

- `/Admin/DocumentPortalAdmin/List`
- `/Admin/DocumentPortalAdmin/Edit/{documentId}`

Ảnh minh họa:

![Duyệt và xuất bản tài liệu](images/admin-duyet-tai-lieu.png)

Cần chụp những gì:

- Chụp danh sách tài liệu lọc theo trạng thái chưa xuất bản và trang sửa tài liệu có trường Published.

Hướng dẫn từng bước:

1. Vào `/Admin/DocumentPortalAdmin/List`.
2. Tại bộ lọc `Trạng thái`, chọn tài liệu chưa xuất bản nếu có.
3. Bấm `Tìm kiếm`.
4. Tìm tài liệu cần duyệt.
5. Bấm `Sửa`.
6. Kiểm tra `Tiêu đề`, `Mã tài liệu`, `Ngày ban hành`.
7. Kiểm tra `Danh mục`, `Loại tài liệu`, `Cơ quan ban hành`.
8. Kiểm tra `Phạm vi truy cập` để tránh công khai nhầm tài liệu nội bộ.
9. Mở hoặc tải file đính kèm để kiểm tra nội dung.
10. Nếu tài liệu đúng, bật `Published`.
11. Nếu tài liệu sai, sửa thông tin hoặc liên hệ người gửi để bổ sung.
12. Bấm `Lưu`.
13. Mở `/tai-lieu` để kiểm tra tài liệu đã hiển thị theo phạm vi.

Kết quả:

- Tài liệu hợp lệ được xuất bản.
- Tài liệu không hợp lệ được giữ lại để chỉnh sửa.

Ví dụ:

- Tài liệu `Kế hoạch bảo đảm quân nhu quý II` đã đầy đủ file và đúng phạm vi `Nội bộ`. Admin bật `Published` và bấm `Lưu`.

### 7.9. Cấu hình mẫu báo cáo

Mục đích:

- Tạo mẫu báo cáo gồm các chỉ tiêu để đơn vị nhập số liệu theo kỳ.

Điều kiện thực hiện:

- Admin có quyền cấu hình báo cáo.

URL thực hiện:

- `/Admin/ReportTemplateAdmin/List`
- `/Admin/ReportTemplateAdmin/Indicators?reportTemplateId={id}`

URL chụp ảnh:

- `/Admin/ReportTemplateAdmin/List`
- `/Admin/ReportTemplateAdmin/Indicators?reportTemplateId={id}`

Ảnh minh họa:

![Cấu hình mẫu báo cáo](images/admin-cau-hinh-mau-bao-cao.png)

Cần chụp những gì:

- Chụp danh sách mẫu báo cáo và màn hình cấu hình chỉ tiêu của mẫu.

Hướng dẫn từng bước:

1. Vào `/Admin/ReportTemplateAdmin/List`.
2. Bấm `Thêm mới` nếu cần tạo mẫu báo cáo mới.
3. Nhập tên mẫu báo cáo.
4. Nhập mô tả nếu có.
5. Chọn trạng thái sử dụng nếu form có tùy chọn active.
6. Bấm `Lưu`.
7. Mở phần `Chỉ tiêu` của mẫu báo cáo.
8. Tại màn hình chỉ tiêu, thêm từng chỉ tiêu cần thu thập.
9. Nhập tên chỉ tiêu.
10. Chọn kiểu dữ liệu, đơn vị tính hoặc thứ tự hiển thị nếu có.
11. Bấm `Lưu` sau khi cấu hình xong.

Kết quả:

- Mẫu báo cáo và các chỉ tiêu được tạo để sử dụng cho kỳ báo cáo.

Ví dụ:

- Mẫu báo cáo: `Báo cáo hậu cần tháng`
- Chỉ tiêu 1: `Lương thực tồn kho`, đơn vị tính `kg`
- Chỉ tiêu 2: `Nhiên liệu tồn kho`, đơn vị tính `lít`

### 7.10. Cấu hình kỳ báo cáo

Mục đích:

- Tạo kỳ báo cáo để các đơn vị nhập và gửi số liệu theo thời gian quy định.

Điều kiện thực hiện:

- Đã có mẫu báo cáo.
- Admin có quyền quản lý kỳ báo cáo.

URL thực hiện:

- `/Admin/ReportPeriodAdmin/List`

URL chụp ảnh:

- `/Admin/ReportPeriodAdmin/List`

Ảnh minh họa:

![Cấu hình kỳ báo cáo](images/admin-cau-hinh-ky-bao-cao.png)

Cần chụp những gì:

- Chụp danh sách kỳ báo cáo và form thêm/sửa kỳ báo cáo.

Hướng dẫn từng bước:

1. Vào `/Admin/ReportPeriodAdmin/List`.
2. Bấm `Thêm mới`.
3. Nhập tên kỳ báo cáo, ví dụ `Tháng 05/2026`.
4. Chọn mẫu báo cáo áp dụng.
5. Chọn ngày bắt đầu kỳ báo cáo.
6. Chọn ngày kết thúc kỳ báo cáo.
7. Chọn hạn nộp báo cáo nếu có.
8. Bật trạng thái mở kỳ nếu muốn đơn vị bắt đầu nhập.
9. Bấm `Lưu`.
10. Kiểm tra kỳ báo cáo đã xuất hiện trong danh sách.

Kết quả:

- Kỳ báo cáo được tạo và đơn vị có thể nhập số liệu nếu kỳ đang mở.

Ví dụ:

- Tên kỳ: `Tháng 05/2026`
- Mẫu báo cáo: `Báo cáo hậu cần tháng`
- Ngày bắt đầu: `01/05/2026`
- Ngày kết thúc: `31/05/2026`
- Hạn nộp: `05/06/2026`

### 7.11. Theo dõi dashboard báo cáo hậu cần

Mục đích:

- Theo dõi tình hình nộp báo cáo, số liệu tổng hợp và các chỉ số liên quan đến báo cáo hậu cần.

Điều kiện thực hiện:

- Admin có quyền xem dashboard báo cáo.
- Dữ liệu báo cáo đã được đơn vị nhập.

URL thực hiện:

- `/Admin/DashboardAdmin/Index`

URL chụp ảnh:

- `/Admin/DashboardAdmin/Index`

Ảnh minh họa:

![Dashboard báo cáo hậu cần](images/admin-dashboard-bao-cao-hau-can.png)

Cần chụp những gì:

- Chụp các thẻ thống kê báo cáo, biểu đồ và danh sách đơn vị/trạng thái nếu có.

Hướng dẫn từng bước:

1. Vào `/Admin/DashboardAdmin/Index`.
2. Chọn kỳ báo cáo nếu màn hình có bộ lọc kỳ.
3. Chọn đơn vị nếu cần xem theo đơn vị.
4. Bấm `Tìm kiếm` hoặc đợi dashboard tự cập nhật.
5. Xem số lượng báo cáo đã gửi, chưa gửi, trả lại hoặc đã khóa.
6. Xem các biểu đồ tổng hợp nếu có.
7. Bấm vào một nhóm dữ liệu để xem chi tiết nếu dashboard hỗ trợ.

Kết quả:

- Admin nắm được tình hình báo cáo của các đơn vị.

Ví dụ:

- Chọn kỳ `Tháng 05/2026`, dashboard hiển thị `18 đơn vị đã gửi`, `3 đơn vị chưa gửi`, admin liên hệ 3 đơn vị còn thiếu.

### 7.12. Cấu hình hệ thống

Mục đích:

- Kiểm tra và điều chỉnh các thiết lập chung của hệ thống.

Điều kiện thực hiện:

- Chỉ tài khoản quản trị hệ thống có quyền cấu hình mới được thao tác.

URL thực hiện:

- `/Admin/Setting/AllSettings`

URL chụp ảnh:

- `/Admin/Setting/AllSettings`

Ảnh minh họa:

![Cấu hình hệ thống](images/admin-cau-hinh-he-thong.png)

Cần chụp những gì:

- Chụp danh sách setting, ô tìm kiếm setting và nút sửa/lưu.

Hướng dẫn từng bước:

1. Đăng nhập bằng tài khoản admin hệ thống.
2. Mở `/Admin/Setting/AllSettings`.
3. Tại ô tìm kiếm, nhập tên setting cần kiểm tra nếu biết.
4. Bấm `Tìm kiếm`.
5. Xem giá trị setting hiện tại.
6. Nếu cần sửa, cập nhật giá trị mới vào ô giá trị.
7. Kiểm tra kỹ tên setting để tránh sửa nhầm.
8. Bấm `Lưu`.
9. Tải lại trang liên quan để kiểm tra tác động của setting.

Kết quả:

- Thiết lập hệ thống được cập nhật.

Cần lưu ý:

- Không sửa setting nếu không biết tác động.
- Nên ghi lại giá trị cũ trước khi thay đổi.

Ví dụ:

- Admin tìm setting liên quan đến số lượng bản ghi trên mỗi trang, đổi giá trị từ `10` thành `20`, bấm `Lưu`, sau đó mở danh sách tài liệu để kiểm tra mỗi trang hiển thị 20 bản ghi.

## 8. Ma trận tổng hợp quyền và phạm vi

| Cấp tài khoản | Được xem | Được thao tác chính | Không được thao tác |
| --- | --- | --- | --- |
| Khách chưa đăng nhập | Trang chủ, tin tức, văn bản, đơn vị, tài liệu công khai | Tìm kiếm, xem nội dung public, đăng ký tài khoản | Không vào admin, không đăng tài liệu nội bộ |
| Tài khoản chờ duyệt | Nội dung public, trang kết quả đăng ký | Đăng nhập kiểm tra trạng thái | Không dùng chức năng nội bộ |
| Người dùng đã duyệt | Hồ sơ cá nhân, nội dung nội bộ theo quyền | Sửa hồ sơ, đăng tài liệu, sửa tài liệu của mình | Không quản trị hệ thống |
| Người dùng đơn vị | Thông tin đơn vị, nội dung liên quan đơn vị | Tìm kiếm, xem chi tiết đơn vị, đăng tài liệu cá nhân | Không thao tác nghiệp vụ admin nếu chưa được cấp quyền |
| Nhân viên đơn vị | Admin theo quyền, dữ liệu đơn vị hiện tại | Quản lý tài liệu, nhập báo cáo, quản lý tin/sản phẩm nếu được cấp | Không quản lý toàn hệ thống |
| Trưởng đơn vị | Dashboard đơn vị, báo cáo đơn vị, nhân viên đơn vị | Cấp quyền nhân viên, theo dõi báo cáo, kiểm tra tài liệu | Không cấu hình toàn hệ thống nếu không phải admin |
| Quản trị hệ thống | Toàn bộ hệ thống | Phê duyệt tài khoản, gán đơn vị, quản lý dữ liệu, cấu hình báo cáo và hệ thống | Không giới hạn trong phạm vi quyền admin |

## 9. Danh sách URL cần chụp ảnh nhanh

| Chức năng | URL chụp ảnh |
| --- | --- |
| Trang chủ | `/` |
| Tìm kiếm toàn hệ thống | `/search/?q=202` |
| Tin tức | `/blog/news` |
| Văn bản | `/blog/documents` |
| Kho tài liệu | `/tai-lieu` |
| Đăng tài liệu | `/tai-lieu/upload` |
| Đơn vị | `/don-vi` |
| Chi tiết đơn vị | `/don-vi/{vendorId}` |
| Đăng ký | `/register/` |
| Đăng nhập | `/login/` |
| Hồ sơ cá nhân | `/customer/profile` |
| Sửa hồ sơ | `/customer/edit` |
| Admin dashboard | `/Admin` |
| Quản lý người dùng | `/Admin/Customer/List` |
| Sửa người dùng | `/Admin/Customer/Edit/{customerId}` |
| Quản lý đơn vị | `/Admin/Vendor/List` |
| Sửa đơn vị | `/Admin/Vendor/Edit/{vendorId}` |
| Quản lý sản phẩm | `/Admin/Product/List` |
| Thêm sản phẩm | `/Admin/Product/Create` |
| Quản lý tin tức/văn bản | `/Admin/Blog/List` |
| Thêm tin tức/văn bản | `/Admin/Blog/Create` |
| Quản lý tài liệu admin | `/Admin/DocumentPortalAdmin/List` |
| Thêm tài liệu admin | `/Admin/DocumentPortalAdmin/Create` |
| Sửa tài liệu admin | `/Admin/DocumentPortalAdmin/Edit/{documentId}` |
| Cấp quyền nhân viên đơn vị | `/Admin/VendorStaffAccess/Index` |
| Báo cáo đơn vị | `/Admin/UnitReportAdmin/Index` |
| Nhập báo cáo đơn vị | `/Admin/UnitReportAdmin/Edit/{reportId}` |
| Mẫu báo cáo | `/Admin/ReportTemplateAdmin/List` |
| Chỉ tiêu mẫu báo cáo | `/Admin/ReportTemplateAdmin/Indicators?reportTemplateId={id}` |
| Kỳ báo cáo | `/Admin/ReportPeriodAdmin/List` |
| Dashboard báo cáo hậu cần | `/Admin/DashboardAdmin/Index` |
| Cấu hình hệ thống | `/Admin/Setting/AllSettings` |
