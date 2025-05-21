using HotelManagement.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.Utilities;
using HotelManagement.DataReader;
using System.Data;

namespace HotelManagement.Services
{
    public interface IAccountRepository
    {
        Task<(ApiResponse<IEnumerable<Account>> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaTaiKhoan", string? sortOrder = "ASC");
        Task<ApiResponse<Account>> GetByIdAsync(int id);
        Task<ApiResponse<Account>> GetByUsernameAsync(string username);
        Task<ApiResponse<int>> CreateAsync(AddAccount account);
        Task<ApiResponse<bool>> UpdateAsync(Account account);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> ChangePasswordAsync(int id, string currentPassword, string newPassword);
        Task<ApiResponse<bool>> IsUsernameExistsAsync(string username);
        Task<ApiResponse<bool>> IsEmailExistsAsync(string email);
        Task<ApiResponse<Account>> SearchByUsernameAsync(string username);
    }
    public class AccountRepository : IAccountRepository
    {
        private readonly DatabaseDapper _db;

        public AccountRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<(ApiResponse<IEnumerable<Account>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaTaiKhoan", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_Account_GetAll",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<Account>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<Account>>.SuccessResponse(items, "Lấy danh sách tài khoản thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<Account>>.ErrorResponse($"Lỗi khi lấy danh sách tài khoản: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<Account>> GetByIdAsync(int id)
        {
            try
            {
                var account = await _db.QueryFirstOrDefaultStoredProcedureAsync<Account>("sp_Account_GetById", new { MaTaiKhoan = id });
                if (account == null)
                    return ApiResponse<Account>.ErrorResponse("Không tìm thấy tài khoản");

                return ApiResponse<Account>.SuccessResponse(account, "Lấy thông tin tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Account>.ErrorResponse($"Lỗi khi lấy tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Account>> GetByUsernameAsync(string username)
        {
            try
            {
                var account = await _db.QueryFirstOrDefaultStoredProcedureAsync<Account>("sp_Account_Login", new { TenTaiKhoan = username });
                if (account == null)
                    return ApiResponse<Account>.ErrorResponse("Không tìm thấy tài khoản");

                return ApiResponse<Account>.SuccessResponse(account, "Lấy thông tin tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Account>.ErrorResponse($"Lỗi khi lấy tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(AddAccount addAccount)
        {
            try
            {
                string hashedPassword = PasswordHasher.HashPassword(addAccount.MatKhau);
                var parameters = new
                {
                    addAccount.TenTaiKhoan,
                    MatKhau = hashedPassword,
                    addAccount.TenHienThi,
                    addAccount.Email,
                    addAccount.Phone,
                    addAccount.MaVaiTro
                };

                var newId = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Account_Insert", parameters);
                return ApiResponse<int>.SuccessResponse(newId, "Tạo tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Account account)
        {
            try
            {
                // Tạo câu SQL trực tiếp để gọi stored procedure
                string sql = "EXEC sp_Account_Update @MaTaiKhoan, @TenTaiKhoan, @TenHienThi, @Email, @Phone, @MaVaiTro; SELECT @@ROWCOUNT AS RowsAffected;";

                var parameters = new
                {
                    account.MaTaiKhoan,
                    account.TenTaiKhoan,
                    account.TenHienThi,
                    account.Email,
                    account.Phone,
                    account.MaVaiTro
                };

                // Gọi stored procedure và nhận kết quả
                var result = await _db.QueryFirstOrDefaultAsync<int>(sql, parameters);

                // Kiểm tra kết quả
                if (result <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật tài khoản thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật tài khoản: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                int rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Account_Delete", new { MaTaiKhoan = id });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa tài khoản thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(int id, string currentPassword, string newPassword)
        {
            try
            {
                var account = await _db.QueryFirstOrDefaultStoredProcedureAsync<Account>("sp_Account_GetById", new { MaTaiKhoan = id });
                if (account == null)
                    return ApiResponse<bool>.ErrorResponse("Không tìm thấy tài khoản");

                string hashedCurrentPassword = PasswordHasher.HashPassword(currentPassword);
                if (account.MatKhau != hashedCurrentPassword)
                    return ApiResponse<bool>.ErrorResponse("Mật khẩu hiện tại không đúng");

                string hashedNewPassword = PasswordHasher.HashPassword(newPassword);
                int rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Account_ChangePassword",
                    new { MaTaiKhoan = id, MatKhau = hashedNewPassword });

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Đổi mật khẩu thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi đổi mật khẩu: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> IsUsernameExistsAsync(string username)
        {
            try
            {
                var account = await _db.QueryFirstOrDefaultAsync<Account>(
                    "SELECT TOP 1 MaTaiKhoan FROM Account WHERE TenTaiKhoan = @TenTaiKhoan",
                    new { TenTaiKhoan = username });

                return ApiResponse<bool>.SuccessResponse(account != null, account != null ? "Tên tài khoản đã tồn tại" : "Tên tài khoản hợp lệ");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi kiểm tra tên tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> IsEmailExistsAsync(string email)
        {
            try
            {
                var account = await _db.QueryFirstOrDefaultAsync<Account>(
                    "SELECT TOP 1 MaTaiKhoan FROM Account WHERE Email = @Email",
                    new { Email = email });

                return ApiResponse<bool>.SuccessResponse(account != null, account != null ? "Email đã tồn tại" : "Email hợp lệ");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi kiểm tra email: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Account>> SearchByUsernameAsync(string username)
        {
            try
            {
                var account = await _db.QueryFirstOrDefaultStoredProcedureAsync<Account>(
                    "sp_Account_SearchByUsername",
                    new { TenTaiKhoan = username });

                if (account == null)
                    return ApiResponse<Account>.ErrorResponse($"Không tìm thấy tài khoản với tên {username}");

                return ApiResponse<Account>.SuccessResponse(account, "Tìm kiếm tài khoản thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Account>.ErrorResponse($"Lỗi khi tìm kiếm tài khoản: {ex.Message}");
            }
        }
    }
}