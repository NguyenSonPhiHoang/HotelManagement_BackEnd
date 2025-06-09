using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IRoleRepository
    {
        Task<ApiResponse<IEnumerable<Role>>> GetAllAsync();
        Task<ApiResponse<Role>> GetByIdAsync(string maVaiTro);
        Task<ApiResponse<int>> CreateAsync(AddRole addRole);
        Task<ApiResponse<bool>> UpdateAsync(Role role);
        Task<ApiResponse<bool>> DeleteAsync(string maVaiTro);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly DatabaseDapper _db;

        public RoleRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<IEnumerable<Role>>> GetAllAsync()
        {
            try
            {
                var roles = await _db.QueryStoredProcedureAsync<Role>("sp_GetAllRoles");
                return ApiResponse<IEnumerable<Role>>.SuccessResponse(roles, "Lấy danh sách vai trò thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Role>>.ErrorResponse($"Lỗi khi lấy danh sách vai trò: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Role>> GetByIdAsync(string maVaiTro)
        {
            try
            {
                var role = await _db.QueryFirstOrDefaultStoredProcedureAsync<Role>("sp_GetRoleById", new { MaVaiTro = maVaiTro });
                if (role == null)
                    return ApiResponse<Role>.ErrorResponse("Không tìm thấy vai trò");

                return ApiResponse<Role>.SuccessResponse(role, "Lấy thông tin vai trò thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Role>.ErrorResponse($"Lỗi khi lấy thông tin vai trò: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(AddRole addRole)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addRole.TenVaiTro))
                    return ApiResponse<int>.ErrorResponse("Tên vai trò không được để trống");

                var parameters = new
                {
                    TenVaiTro = addRole.TenVaiTro
                };

                var result = await _db.ExecuteStoredProcedureAsync("sp_InsertRole", parameters);
                if (result <= 0)
                    return ApiResponse<int>.ErrorResponse("Tạo vai trò thất bại");

                return ApiResponse<int>.SuccessResponse(result, "Tạo vai trò thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo vai trò: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Role role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role.MaVaiTro))
                    return ApiResponse<bool>.ErrorResponse("Mã vai trò không được để trống");

                if (string.IsNullOrWhiteSpace(role.TenVaiTro))
                    return ApiResponse<bool>.ErrorResponse("Tên vai trò không được để trống");

                var parameters = new
                {
                    role.MaVaiTro,
                    role.TenVaiTro
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_UpdateRole", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật vai trò thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật vai trò thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật vai trò: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string maVaiTro)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maVaiTro))
                    return ApiResponse<bool>.ErrorResponse("Mã vai trò không được để trống");

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_DeleteRole", new { MaVaiTro = maVaiTro });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa vai trò thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa vai trò thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa vai trò: {ex.Message}");
            }
        }
    }
}