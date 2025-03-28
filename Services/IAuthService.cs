using HotelManagement.DataReader;
using HotelManagement.Model;
using HotelManagement.Utilities;
using System;

namespace HotelManagement.Services
{
    public interface IAuthService
    {
        TokenResponse Login(LoginRequest request);
        ApiResponse<int> Register(RegisterRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly DatabaseDapper _db;

        public AuthService(DatabaseDapper db)
        {
            _db = db;
        }

        public TokenResponse Login(LoginRequest request)
        {
            try
            {
                // Mã hóa mật khẩu đầu vào 
                var hashedPassword = PasswordHasher.HashPassword(request.MatKhau);

                var parameters = new
                {
                    TenTaiKhoan = request.TenTaiKhoan,
                    MatKhau = hashedPassword  // Gửi mật khẩu đã mã hóa để so sánh
                };

                // Gọi stored procedure đăng nhập
                var result = _db.QueryFirstOrDefaultStoredProcedure<TokenResponse>("sp_Login", parameters);
                return result;
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây nếu cần
                return null;
            }
        }

        public ApiResponse<int> Register(RegisterRequest request)
        {
            // Kiểm tra ràng buộc
            if (!ValidateRegisterRequest(request, out string errorMessage))
            {
                return ApiResponse<int>.ErrorResponse(errorMessage);
            }

            try
            {
                // Mã hóa mật khẩu trước khi lưu vào database
                var hashedPassword = PasswordHasher.HashPassword(request.MatKhau);

                var parameters = new
                {
                    TenTaiKhoan = request.TenTaiKhoan,
                    MatKhau = hashedPassword,
                    TenHienThi = request.TenHienThi,
                    Email = request.Email,
                    Phone = request.Phone,
                    MaVaiTro = request.MaVaiTro
                };

                var result = _db.QueryFirstOrDefaultStoredProcedure<dynamic>("sp_Register", parameters);

                if (result != null && result.MaTaiKhoan > 0)
                {
                    return ApiResponse<int>.SuccessResponse(
                        (int)result.MaTaiKhoan,
                        result.ThongBao ?? "Đăng ký thành công");
                }
                else
                {
                    return ApiResponse<int>.ErrorResponse(
                        result?.ThongBao ?? "Đăng ký thất bại");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        // Phương thức kiểm tra các ràng buộc
        private bool ValidateRegisterRequest(RegisterRequest request, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(request.TenTaiKhoan) || request.TenTaiKhoan.Length < 5)
            {
                errorMessage = "Tên tài khoản phải có ít nhất 5 ký tự";
                return false;
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrEmpty(request.MatKhau) || request.MatKhau.Length < 6)
            {
                errorMessage = "Mật khẩu phải có ít nhất 6 ký tự";
                return false;
            }

            // Kiểm tra email
            if (string.IsNullOrEmpty(request.Email) || !System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                errorMessage = "Email phải có định dạng @gmail.com";
                return false;
            }

            // Kiểm tra số điện thoại
            if (string.IsNullOrEmpty(request.Phone) || !System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^\d{10}$"))
            {
                errorMessage = "Số điện thoại phải có 10 chữ số";
                return false;
            }

            return true;
        }
    }
}