using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IServiceTypeRepository
    {
        Task<(ApiResponse<IEnumerable<ServiceType>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaLoaiDV", string? sortOrder = "ASC");
        Task<ApiResponse<ServiceType>> GetByIdAsync(int maLoaiDichVu);
        Task<ApiResponse<ServiceType>> CreateAsync(AddServiceType serviceType);
        Task<ApiResponse<ServiceType>> UpdateAsync(int maLoaiDichVu, UpdateServiceType serviceType);
        Task<ApiResponse<bool>> DeleteAsync(int maLoaiDichVu);
    }

    public class ServiceTypeRepository : IServiceTypeRepository
    {
        private readonly DatabaseDapper _db;

        public ServiceTypeRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<(ApiResponse<IEnumerable<ServiceType>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaLoaiDV", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_GetAllServiceTypes",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<ServiceType>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<ServiceType>>.SuccessResponse(items, "Lấy danh sách loại dịch vụ thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<ServiceType>>.ErrorResponse($"Lỗi khi lấy danh sách loại dịch vụ: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<ServiceType>> GetByIdAsync(int maLoaiDichVu)
        {
            try
            {
                var serviceType = await _db.QueryFirstOrDefaultStoredProcedureAsync<ServiceType>("sp_GetServiceTypeById", new { MaLoaiDV = maLoaiDichVu });
                if (serviceType == null)
                    return ApiResponse<ServiceType>.ErrorResponse("Không tìm thấy loại dịch vụ");

                return ApiResponse<ServiceType>.SuccessResponse(serviceType, "Lấy thông tin loại dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceType>.ErrorResponse($"Lỗi khi lấy thông tin loại dịch vụ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ServiceType>> CreateAsync(AddServiceType serviceType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serviceType.TenLoaiDV))
                    return ApiResponse<ServiceType>.ErrorResponse("Tên loại dịch vụ không được để trống");

                var parameters = new
                {
                    serviceType.TenLoaiDV,
                    serviceType.MoTa
                };

                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<ServiceType>("sp_CreateServiceType", parameters);
                if (result == null)
                    return ApiResponse<ServiceType>.ErrorResponse("Tạo loại dịch vụ thất bại");

                return ApiResponse<ServiceType>.SuccessResponse(result, "Tạo loại dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceType>.ErrorResponse($"Lỗi khi tạo loại dịch vụ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ServiceType>> UpdateAsync(int maLoaiDichVu, UpdateServiceType serviceType)
        {
            try
            {
                var parameters = new
                {
                    MaLoaiDV = maLoaiDichVu,
                    serviceType.TenLoaiDV,
                    serviceType.MoTa
                };

                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<ServiceType>("sp_UpdateServiceType", parameters);
                if (result == null)
                    return ApiResponse<ServiceType>.ErrorResponse("Cập nhật loại dịch vụ thất bại");

                return ApiResponse<ServiceType>.SuccessResponse(result, "Cập nhật loại dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceType>.ErrorResponse($"Lỗi khi cập nhật loại dịch vụ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maLoaiDichVu)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_DeleteServiceType", new { MaLoaiDV = maLoaiDichVu });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa loại dịch vụ thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa loại dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa loại dịch vụ: {ex.Message}");
            }
        }
    }
}