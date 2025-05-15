using HotelManagement.DataReader;
using HotelManagement.Model;
using HotelManagement.Utilities;
using System;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IAuthRepository
    {
        Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<int>> RegisterAsync(RegisterRequest request);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly DatabaseDapper _db;

        public AuthRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var hashedPassword = PasswordHasher.HashPassword(request.MatKhau);
                var parameters = new
                {
                    TenTaiKhoan = request.TenTaiKhoan,
                    MatKhau = hashedPassword
                };

                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<TokenResponse>("sp_Login", parameters);
                if (result == null || result.MaTaiKhoan <= 0)
                    return ApiResponse<TokenResponse>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng");

                return ApiResponse<TokenResponse>.SuccessResponse(result, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<TokenResponse>.ErrorResponse($"Lỗi khi đăng nhập: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> RegisterAsync(RegisterRequest request)
        {
            if (!ValidateRegisterRequest(request, out string errorMessage))
                return ApiResponse<int>.ErrorResponse(errorMessage);

            try
            {
                var hashedPassword = PasswordHasher.HashPassword(request.MatKhau);
                var parameters = new
                {
                    request.TenTaiKhoan,
                    MatKhau = hashedPassword,
                    request.TenHienThi,
                    request.Email,
                    request.Phone,
                    request.MaVaiTro
                };

                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<dynamic>("sp_Register", parameters);
                if (result != null && result.MaTaiKhoan > 0)
                    return ApiResponse<int>.SuccessResponse((int)result.MaTaiKhoan, result.ThongBao ?? "Đăng ký thành công");

                return ApiResponse<int>.ErrorResponse(result?.ThongBao ?? "Đăng ký thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi đăng ký: {ex.Message}");
            }
        }

        private bool ValidateRegisterRequest(RegisterRequest request, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(request.TenTaiKhoan) || request.TenTaiKhoan.Length < 5)
            {
                errorMessage = "Tên tài khoản phải có ít nhất 5 ký tự";
                return false;
            }

            if (string.IsNullOrEmpty(request.MatKhau) || request.MatKhau.Length < 6)
            {
                errorMessage = "Mật khẩu phải có ít nhất 6 ký tự";
                return false;
            }

            if (string.IsNullOrEmpty(request.Email) || !System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                errorMessage = "Email phải có định dạng @gmail.com";
                return false;
            }

            if (string.IsNullOrEmpty(request.Phone) || !System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^\d{10}$"))
            {
                errorMessage = "Số điện thoại phải có 10 chữ số";
                return false;
            }

            return true;
        }
    }
}