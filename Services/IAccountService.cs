// Services/IAccountService.cs
using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.Utilities;

namespace HotelManagement.Services
{
    public interface IAccountService
    {
        ApiResponse<IEnumerable<Account>> GetAllAccounts();
        ApiResponse<Account> GetAccountById(int id);
        ApiResponse<Account> GetAccountByUsername(string username);
        ApiResponse<int> CreateAccount(AddAccount account);
        ApiResponse<bool> UpdateAccount(Account account);
        ApiResponse<bool> DeleteAccount(int id);
        ApiResponse<bool> ChangePassword(int id, string currentPassword, string newPassword);
        ApiResponse<bool> IsUsernameExists(string username);
        ApiResponse<bool> IsEmailExists(string email);
    }

    public class AccountService : IAccountService
    {
        private readonly DatabaseDapper _db;

        public AccountService(DatabaseDapper db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public ApiResponse<IEnumerable<Account>> GetAllAccounts()
        {
            try
            {
                var accounts = _db.QueryStoredProcedure<Account>("sp_Account_GetAll");
                return ApiResponse<IEnumerable<Account>>.SuccessResponse(accounts, "Lấy danh sách tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Account>>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Account> GetAccountById(int id)
        {
            try
            {
                var account = _db.QueryFirstOrDefaultStoredProcedure<Account>("sp_Account_GetById", new { MaTaiKhoan = id });

                if (account == null)
                    return ApiResponse<Account>.ErrorResponse("Không tìm thấy tài khoản");

                return ApiResponse<Account>.SuccessResponse(account, "Lấy thông tin tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Account>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Account> GetAccountByUsername(string username)
        {
            try
            {
                var account = _db.QueryFirstOrDefaultStoredProcedure<Account>("sp_Account_Login", new { TenTaiKhoan = username });

                if (account == null)
                    return ApiResponse<Account>.ErrorResponse("Không tìm thấy tài khoản");

                return ApiResponse<Account>.SuccessResponse(account, "Lấy thông tin tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Account>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<int> CreateAccount(AddAccount addAccount)
        {
            try
            {
                // Mã hóa mật khẩu trước khi lưu vào database
                string hashedPassword = PasswordHasher.HashPassword(addAccount.MatKhau);

                var parameters = new
                {
                    addAccount.TenTaiKhoan,
                    MatKhau = hashedPassword, // Lưu mật khẩu đã mã hóa
                    addAccount.TenHienThi,
                    addAccount.Email,
                    addAccount.Phone,
                    addAccount.MaVaiTro
                };

                var newId = _db.QueryFirstOrDefaultStoredProcedure<int>("sp_Account_Insert", parameters);
                return ApiResponse<int>.SuccessResponse(newId, "Tạo tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> UpdateAccount(Account account)
        {
            try
            {
                var parameters = new
                {
                    account.MaTaiKhoan,
                    account.TenTaiKhoan,
                    account.TenHienThi,
                    account.Email,
                    account.Phone,
                    account.MaVaiTro
                };

                int rowsAffected = _db.ExecuteStoredProcedure("sp_Account_Update", parameters);

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật tài khoản thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> DeleteAccount(int id)
        {
            try
            {
                int rowsAffected = _db.ExecuteStoredProcedure("sp_Account_Delete", new { MaTaiKhoan = id });

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa tài khoản thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> ChangePassword(int id, string currentPassword, string newPassword)
        {
            try
            {
                // Kiểm tra mật khẩu hiện tại
                var account = _db.QueryFirstOrDefaultStoredProcedure<Account>("sp_Account_GetById", new { MaTaiKhoan = id });

                if (account == null)
                    return ApiResponse<bool>.ErrorResponse("Không tìm thấy tài khoản");

                // Mã hóa mật khẩu hiện tại để so sánh
                string hashedCurrentPassword = PasswordHasher.HashPassword(currentPassword);

                if (account.MatKhau != hashedCurrentPassword)
                    return ApiResponse<bool>.ErrorResponse("Mật khẩu hiện tại không đúng");

                // Mã hóa mật khẩu mới
                string hashedNewPassword = PasswordHasher.HashPassword(newPassword);

                // Cập nhật mật khẩu mới
                int rowsAffected = _db.ExecuteStoredProcedure("sp_Account_ChangePassword",
                    new { MaTaiKhoan = id, MatKhau = hashedNewPassword });

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Đổi mật khẩu thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> IsUsernameExists(string username)
        {
            try
            {
                var account = _db.QueryFirstOrDefault<Account>(
                    "SELECT TOP 1 MaTaiKhoan FROM Account WHERE TenTaiKhoan = @TenTaiKhoan",
                    new { TenTaiKhoan = username });

                return ApiResponse<bool>.SuccessResponse(account != null);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> IsEmailExists(string email)
        {
            try
            {
                var account = _db.QueryFirstOrDefault<Account>(
                    "SELECT TOP 1 MaTaiKhoan FROM Account WHERE Email = @Email",
                    new { Email = email });

                if (account == null)
                    return ApiResponse<bool>.ErrorResponse("Email không tồn tại");

                return ApiResponse<bool>.SuccessResponse(true, "Email đã tồn tại");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }
    }
}