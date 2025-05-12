using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IServiceTypeRepository
    {
        ApiResponse<IEnumerable<ServiceType>> GetAllServiceTypes();
        ApiResponse<ServiceType> GetServiceTypeById(int maLoaiDichVu);
        ApiResponse<ServiceType> CreateServiceType(AddServiceType serviceType);
        ApiResponse<ServiceType> UpdateServiceType(int maLoaiDichVu, UpdateServiceType serviceType);
        ApiResponse<int> DeleteServiceType(int maLoaiDichVu);
    }
    public class ServiceTypeRepository : IServiceTypeRepository
    {
        private readonly DatabaseDapper _db;

        public ServiceTypeRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public ApiResponse<IEnumerable<ServiceType>> GetAllServiceTypes()
        {
            try
            {
                var serviceTypes = _db.QueryStoredProcedure<ServiceType>("sp_GetAllServiceTypes").ToList();
                return serviceTypes.Count != 0
                    ? ApiResponse<IEnumerable<ServiceType>>.SuccessResponse(serviceTypes, "Lấy danh sách loại dịch vụ thành công")
                    : ApiResponse<IEnumerable<ServiceType>>.ErrorResponse("Không có loại dịch vụ nào");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ServiceType>>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<ServiceType> GetServiceTypeById(int maLoaiDV)
        {
            try
            {
                var serviceType = _db.QueryFirstOrDefaultStoredProcedure<ServiceType>(
                    "sp_GetServiceTypeById",
                    new { MaLoaiDV = maLoaiDV }
                );

                return serviceType != null
                    ? ApiResponse<ServiceType>.SuccessResponse(serviceType, "Lấy loại dịch vụ thành công")
                    : ApiResponse<ServiceType>.ErrorResponse("Không tìm thấy loại dịch vụ");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceType>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<ServiceType> CreateServiceType(AddServiceType serviceType)
        {
            if (string.IsNullOrWhiteSpace(serviceType.TenLoaiDV))
            {
                return ApiResponse<ServiceType>.ErrorResponse("Tên loại dịch vụ không được để trống");
            }

            try
            {
                var result = _db.QueryFirstOrDefault<ServiceType>(
                    "sp_CreateServiceType",
                    new
                    {
                        TenLoaiDV = serviceType.TenLoaiDV,
                        MoTa = serviceType.MoTa
                    }
                );

                return result != null
                    ? ApiResponse<ServiceType>.SuccessResponse(result, "Tạo loại dịch vụ thành công")
                    : ApiResponse<ServiceType>.ErrorResponse("Tạo loại dịch vụ thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceType>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<ServiceType> UpdateServiceType(int maLoaiDV, UpdateServiceType serviceType)
        {
            try
            {
                var result = _db.QueryFirstOrDefault<ServiceType>(
                    "sp_UpdateServiceType",
                    new
                    {
                        MaLoaiDV = maLoaiDV,
                        TenLoaiDV = serviceType.TenLoaiDV,
                        MoTa = serviceType.MoTa
                    }
                );

                return result != null
                    ? ApiResponse<ServiceType>.SuccessResponse(result, "Cập nhật loại dịch vụ thành công")
                    : ApiResponse<ServiceType>.ErrorResponse("Cập nhật loại dịch vụ thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<ServiceType>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<int> DeleteServiceType(int maLoaiDV)
        {
            try
            {
                int result = _db.ExecuteStoredProcedure(
                    "sp_DeleteServiceType",
                    new { MaLoaiDV = maLoaiDV }
                );
         
                return result == -1
                    ? ApiResponse<int>.SuccessResponse(result, "Xóa loại dịch vụ thành công")
                    : ApiResponse<int>.ErrorResponse("Xóa loại dịch vụ thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }


    }
}