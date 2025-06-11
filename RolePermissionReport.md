# Báo cáo phân quyền chức năng theo Role

## 1. Admin
- **Quyền sử dụng:** Toàn quyền với tất cả các chức năng quản lý hệ thống.
- **Chức năng/API:**
  - Quản lý phòng: GET/POST/PUT/DELETE `/api/rooms`, `/api/rooms/{maPhong}`
  - Quản lý loại phòng: GET/POST/PUT/DELETE `/api/room-types`, `/api/room-types/{id}`
  - Quản lý đặt phòng: GET/POST/PUT `/api/bookings`, `/api/bookings/{maDatPhong}`
  - Quản lý dịch vụ: GET/POST/PUT/DELETE `/api/services`, `/api/services/{maDichVu}`
  - Quản lý loại dịch vụ: GET/POST/PUT/DELETE `/api/service-types`, `/api/service-types/{maLoaiDV}`
  - Quản lý hóa đơn: GET/POST/PUT/DELETE `/api/invoices`, `/api/invoices/{maHoaDon}`
  - Quản lý thanh toán: GET/POST `/api/payments`, `/api/payments/{id}`
  - Quản lý khách hàng: GET/POST/PUT `/api/customers`, `/api/customers/{maKhachHang}`
  - Quản lý chương trình tích điểm: GET/POST/PUT/DELETE `/api/point-programs`, `/api/point-programs/{maCT}`
  - Quản lý lịch sử tích điểm: GET `/api/point-history`
  - Quản lý tài khoản: GET/POST/PUT/DELETE `/api/accounts`, `/api/accounts/{maTaiKhoan}`
  - Quản lý phân quyền/vai trò: GET/POST/PUT/DELETE `/api/roles`, `/api/roles/{maVaiTro}`
  - Thống kê, báo cáo doanh thu: GET `/api/invoices/revenue-statistics`
  - Xác thực, kích hoạt tài khoản, đổi mật khẩu, kiểm tra username/email: `/api/auth/*`, `/api/accounts/validate/*`

## 2. Staff
- **Quyền sử dụng:** Được phép sử dụng hầu hết các chức năng quản lý nghiệp vụ khách sạn, trừ các chức năng quản lý tài khoản, phân quyền hệ thống.
- **Chức năng/API:**
  - Quản lý phòng: GET/POST/PUT/DELETE `/api/rooms`, `/api/rooms/{maPhong}`
  - Quản lý loại phòng: GET/POST/PUT/DELETE `/api/room-types`, `/api/room-types/{id}`
  - Quản lý đặt phòng: GET/POST/PUT `/api/bookings`, `/api/bookings/{maDatPhong}`
  - Quản lý dịch vụ: GET/POST/PUT/DELETE `/api/services`, `/api/services/{maDichVu}`
  - Quản lý loại dịch vụ: GET/POST/PUT/DELETE `/api/service-types`, `/api/service-types/{maLoaiDV}`
  - Quản lý hóa đơn: GET/POST/PUT/DELETE `/api/invoices`, `/api/invoices/{maHoaDon}`
  - Quản lý thanh toán: GET/POST `/api/payments`, `/api/payments/{id}`
  - Quản lý khách hàng: GET/POST/PUT `/api/customers`, `/api/customers/{maKhachHang}`
  - Quản lý chương trình tích điểm: GET/POST/PUT/DELETE `/api/point-programs`, `/api/point-programs/{maCT}`
  - Quản lý lịch sử tích điểm: GET `/api/point-history`
  - Thống kê, báo cáo doanh thu: GET `/api/invoices/revenue-statistics`
  - Đặt phòng, xác nhận đặt phòng: POST/PUT `/api/bookings`, `/api/bookings/{maDatPhong}/confim`
  - Không được phép thao tác với các API quản lý tài khoản (`/api/accounts`) và phân quyền (`/api/roles`).

## 3. Customer (Client)
- **Quyền sử dụng:** Chỉ được phép sử dụng các chức năng liên quan đến đặt phòng, xem phòng, xem dịch vụ, đăng ký, đăng nhập, xem hóa đơn của mình.
- **Chức năng/API:**
  - Xem danh sách phòng, loại phòng, dịch vụ: GET `/api/rooms`, `/api/room-types`, `/api/services`
  - Đặt phòng, xem lịch sử đặt phòng của mình: GET/POST `/api/bookings`, `/api/bookings/{maDatPhong}`
  - Đăng ký tài khoản, đăng nhập: POST `/api/auth/registers`, `/api/auth/login`
  - Xem thông tin tài khoản của mình: GET `/api/accounts/username/{username}`
  - Xem hóa đơn, lịch sử giao dịch của mình: GET `/api/invoices`, `/api/invoices/{maHoaDon}`
  - Không được phép truy cập các chức năng quản lý, chỉnh sửa, xóa dữ liệu hệ thống.

---

## Bảng tổng hợp phân quyền

| Chức năng/API                  | Admin | Staff | Customer |
|---------------------------------|:-----:|:-----:|:--------:|
| Quản lý phòng                   |   ✔   |   ✔   |    X     |
| Quản lý loại phòng              |   ✔   |   ✔   |    X     |
| Quản lý đặt phòng               |   ✔   |   ✔   |    ✔     |
| Quản lý dịch vụ                 |   ✔   |   ✔   |    X     |
| Quản lý loại dịch vụ            |   ✔   |   ✔   |    X     |
| Quản lý hóa đơn                 |   ✔   |   ✔   |    ✔     |
| Quản lý thanh toán              |   ✔   |   ✔   |    ✔     |
| Quản lý khách hàng              |   ✔   |   ✔   |    ✔     |
| Quản lý chương trình tích điểm  |   ✔   |   ✔   |    X     |
| Quản lý lịch sử tích điểm       |   ✔   |   ✔   |    X     |
| Quản lý tài khoản               |   ✔   |   X   |    ✔     |
| Quản lý phân quyền/vai trò      |   ✔   |   X   |    X     |
| Đăng ký/Đăng nhập               |   ✔   |   ✔   |    ✔     |
| Thống kê/Báo cáo                |   ✔   |   ✔   |    X     |

---

*Báo cáo này được sinh tự động dựa trên cấu trúc controller và yêu cầu phân quyền bạn đã cung cấp.*
