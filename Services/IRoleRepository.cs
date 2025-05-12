using HotelManagement.Model;
using HotelManagement.DataReader;

namespace HotelManagement.Services
{
    public interface IRoleRepository
    {
        ApiResponse<List<Role>> GetAllRoles();
        ApiResponse<Role> GetRoleById(string maVaiTro);
        ApiResponse<int> CreateRole(Role role);
        ApiResponse<int> UpdateRole(Role role);
        ApiResponse<int> DeleteRole(string maVaiTro);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly DatabaseDapper _db;

        public RoleRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public ApiResponse<List<Role>> GetAllRoles()
        {
            try
            {
                var roles = _db.QueryStoredProcedure<Role>("sp_GetAllRoles").ToList();
                return roles.Any() 
                    ? ApiResponse<List<Role>>.SuccessResponse(roles, "Lấy danh sách vai trò thành công")
                    : ApiResponse<List<Role>>.ErrorResponse("Không có vai trò nào");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<Role>>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Role> GetRoleById(string maVaiTro)
        {
            try
            {
                var role = _db.QueryFirstOrDefaultStoredProcedure<Role>(
                    "sp_GetRoleById", 
                    new { MaVaiTro = maVaiTro }
                );

                return role != null 
                    ? ApiResponse<Role>.SuccessResponse(role, "Lấy vai trò thành công")
                    : ApiResponse<Role>.ErrorResponse("Không tìm thấy vai trò");
            }
            catch (Exception ex)
            {
                return ApiResponse<Role>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<int> CreateRole(Role role)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(role.MaVaiTro))
            {
                return ApiResponse<int>.ErrorResponse("Mã vai trò không được để trống");
            }

            if (string.IsNullOrWhiteSpace(role.TenVaiTro))
            {
                return ApiResponse<int>.ErrorResponse("Tên vai trò không được để trống");
            }

            try
            {
                int result = _db.ExecuteStoredProcedure(
                    "sp_InsertRole", 
                    new { 
                        MaVaiTro = role.MaVaiTro, 
                        TenVaiTro = role.TenVaiTro 
                    }
                );

                return result > 0 
                    ? ApiResponse<int>.SuccessResponse(result, "Tạo vai trò thành công")
                    : ApiResponse<int>.ErrorResponse("Tạo vai trò thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<int> UpdateRole(Role role)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(role.MaVaiTro))
            {
                return ApiResponse<int>.ErrorResponse("Mã vai trò không được để trống");
            }

            if (string.IsNullOrWhiteSpace(role.TenVaiTro))
            {
                return ApiResponse<int>.ErrorResponse("Tên vai trò không được để trống");
            }

            try
            {
                int result = _db.ExecuteStoredProcedure(
                    "sp_UpdateRole", 
                    new { 
                        MaVaiTro = role.MaVaiTro, 
                        TenVaiTro = role.TenVaiTro 
                    }
                );

                return result > 0 
                    ? ApiResponse<int>.SuccessResponse(result, "Cập nhật vai trò thành công")
                    : ApiResponse<int>.ErrorResponse("Cập nhật vai trò thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<int> DeleteRole(string maVaiTro)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(maVaiTro))
            {
                return ApiResponse<int>.ErrorResponse("Mã vai trò không được để trống");
            }

            try
            {
                int result = _db.ExecuteStoredProcedure(
                    "sp_DeleteRole", 
                    new { MaVaiTro = maVaiTro }
                );

                return result > 0 
                    ? ApiResponse<int>.SuccessResponse(result, "Xóa vai trò thành công")
                    : ApiResponse<int>.ErrorResponse("Xóa vai trò thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }
    }
}