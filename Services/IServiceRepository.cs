using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IServiceRepository
    {
        Task<(ApiResponse<IEnumerable<Service>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaDichVu", string? sortOrder = "ASC");
        Task<ApiResponse<Service>> GetByIdAsync(int maDichVu);
        Task<ApiResponse<Service>> CreateAsync(AddService service);
        Task<ApiResponse<Service>> UpdateAsync(int maDichVu, UpdateService service);
        Task<ApiResponse<bool>> DeleteAsync(int maDichVu);
    }

    public class ServiceRepository : IServiceRepository
    {
        private readonly DatabaseDapper _db;

        public ServiceRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<(ApiResponse<IEnumerable<Service>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaDichVu", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_GetAllServices",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<Service>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<Service>>.SuccessResponse(items, "Lấy danh sách dịch vụ thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<Service>>.ErrorResponse($"Lỗi khi lấy danh sách dịch vụ: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<Service>> GetByIdAsync(int maDichVu)
        {
            try
            {
                var service = await _db.QueryFirstOrDefaultStoredProcedureAsync<Service>("sp_GetServiceById", new { MaDichVu = maDichVu });
                if (service == null)
                    return ApiResponse<Service>.ErrorResponse("Không tìm thấy dịch vụ");

                return ApiResponse<Service>.SuccessResponse(service, "Lấy thông tin dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Service>.ErrorResponse($"Lỗi khi lấy thông tin dịch vụ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Service>> CreateAsync(AddService service)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(service.TenDichVu))
                    return ApiResponse<Service>.ErrorResponse("Tên dịch vụ không được để trống");

                if (service.Gia <= 0)
                    return ApiResponse<Service>.ErrorResponse("Giá dịch vụ phải lớn hơn 0");

                var parameters = new
                {
                    service.TenDichVu,
                    service.MaLoaiDV,
                    service.Gia
                };

                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<Service>("sp_CreateService", parameters);
                if (result == null)
                    return ApiResponse<Service>.ErrorResponse("Tạo dịch vụ thất bại");

                return ApiResponse<Service>.SuccessResponse(result, "Tạo dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Service>.ErrorResponse($"Lỗi khi tạo dịch vụ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Service>> UpdateAsync(int maDichVu, UpdateService service)
        {
            try
            {
                if (maDichVu <= 0)
                    return ApiResponse<Service>.ErrorResponse("Mã dịch vụ không hợp lệ");

                var parameters = new
                {
                    MaDichVu = maDichVu,
                    service.TenDichVu,
                    service.MaLoaiDV,
                    service.Gia
                };

                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<Service>("sp_UpdateService", parameters);
                if (result == null)
                    return ApiResponse<Service>.ErrorResponse("Cập nhật dịch vụ thất bại");

                return ApiResponse<Service>.SuccessResponse(result, "Cập nhật dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Service>.ErrorResponse($"Lỗi khi cập nhật dịch vụ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maDichVu)
        {
            try
            {
                if (maDichVu <= 0)
                    return ApiResponse<bool>.ErrorResponse("Mã dịch vụ không hợp lệ");

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_DeleteService", new { MaDichVu = maDichVu });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa dịch vụ thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa dịch vụ: {ex.Message}");
            }
        }
    }
}