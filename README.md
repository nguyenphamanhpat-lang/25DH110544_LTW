# MyStore – ASP.NET MVC5 + Entity Framework 6 + SQL Server

Bài tập website bán hàng theo các slide **Web BE Development** (Project Setup, Admin Site, Customer Site). Cấu trúc code tách riêng `Controllers`, `Models`, `Views`, `Areas/Admin`, `Services`, `Security`.

## Môi trường
- Windows + Visual Studio 2019/2022 (workload **ASP.NET and web development**), .NET Framework **4.8**.
- SQL Server / SQL Server Express / LocalDB, quyền tạo cơ sở dữ liệu.
- NuGet Restore để tải **ASP.NET MVC 5.2.9**, **Entity Framework 6.4.4**.

## Chạy dự án
1. Trong SQL Server Management Studio, chạy `database/MyStore.sql` trên một server trống hoặc chưa có database MyStore (script tạo bảng, dữ liệu minh họa). **Không chạy lại trên DB đã có các bảng.**
2. Mở `MyStoreMVC5.sln`, Restore NuGet packages và Build.
3. Sửa `MyStoreMVC5/Web.config`, connection string `MyStore`, theo SQL Server của máy, ví dụ `Data Source=.\\SQLEXPRESS;Initial Catalog=MyStore;Integrated Security=True`.
4. Nhấn **F5** (IIS Express).
5. Trên máy cục bộ, truy cập `/Admin/Setup` để **tạo tài khoản Admin đầu tiên**. Trang chỉ hoạt động từ `localhost`, và chỉ khi chưa có tài khoản Admin.
6. Đăng nhập Admin ở `/Account/Login`, thêm danh mục, sản phẩm và hình. Khách hàng tự đăng ký tại `/Account/Register`.

> Các file `database2024.txt` đính kèm có hai biến thể: một file viết sai `ProductDecription`, một file dùng `ProductDescription`. Dự án chọn tên chính tả đúng **ProductDescription** theo bản thứ hai.

## Các luồng chính
- **Customer**: Home, danh sách theo danh mục, tìm kiếm theo tên/mô tả/danh mục, lọc khoảng giá, sắp xếp, phân trang; chi tiết sản phẩm; giỏ hàng session (thêm, cập nhật, xóa); đăng ký/đăng nhập/đăng xuất; sửa hồ sơ; checkout **thanh toán khi nhận hàng (COD)**; trang xác nhận; lịch sử và chi tiết các đơn chính mình.
- **Admin** (Area `/Admin`): trang tổng quan; CRUD danh mục và sản phẩm (kèm upload ảnh); tìm kiếm/sắp xếp/phân trang sản phẩm; CRUD tài khoản Admin; tìm kiếm/phân trang khách hàng; danh sách/chi tiết đơn hàng và cập nhật trạng thái thanh toán.
- **Bảo mật cơ bản**: hash mật khẩu PBKDF2 + salt, Forms Authentication, xác minh vai trò từ DB, anti-forgery token cho POST, giá checkout lấy từ DB, giới hạn định dạng/kích thước upload ảnh, không xóa admin đang đăng nhập hoặc admin cuối cùng.

## Những điều cần biết
- Dùng **EF6 DbContext mapping sang CSDL MyStore được cung cấp**, không tạo file `.edmx` tự động bằng designer như thao tác DB First trong slide. Bảng và quan hệ đúng theo SQL đã cung cấp.
- Thanh toán trực tuyến PayPal trong slide là **optional**, chưa tích hợp; chỉ có COD. Trạng thái `COD_PENDING` không có nghĩa đã thu tiền.
- Sản phẩm không có tồn kho vì sơ đồ SQL ban đầu không có trường Stock.
- Bản nguồn chưa được build/chạy end-to-end bằng Visual Studio + SQL Server trên môi trường Windows. Cần thực hiện kiểm thử theo checklist bên dưới trước khi nộp hoặc triển khai.
- Với triển khai thực tế: cần bật HTTPS, thay `customErrors`, dùng logging, tách cấu hình bí mật, xem lại upload và phân quyền; không xem đây là cấu hình production.

## Kiểm thử thủ công khi chạy được
1. `/Admin/Setup` tạo admin đầu tiên, thử tạo lại (phải bị chặn).
2. Admin tạo danh mục, sản phẩm có/không ảnh, sửa, tìm kiếm, sắp xếp, phân trang, xóa sản phẩm chưa có đơn.
3. Guest xem chi tiết sản phẩm, giỏ hàng tăng/giảm/xóa.
4. Khách hàng đăng ký, đăng nhập, sửa hồ sơ, checkout COD, xem xác nhận và lịch sử đơn.
5. Admin xem đơn của khách và đổi trạng thái từ COD_PENDING sang PAID; kiểm tra thống kê doanh thu.
6. Khách A không thể mở đơn của khách B; khách thường không truy cập Area Admin; thử POST thiếu anti-forgery token.
