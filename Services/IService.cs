using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IServiceService
    {
        ApiResponse<List<Service>> GetAllServices();
        ApiResponse<Service> GetServiceById(int maDichVu);
        ApiResponse<Service> CreateService(AddService service);
        ApiResponse<Service> UpdateService(int maDichVu, UpdateService service);
        ApiResponse<int> DeleteService(int maDichVu);
    }

    public class ServiceService : IServiceService
    {
        private readonly DatabaseDapper _db;

        public ServiceService(DatabaseDapper db)
        {
            _db = db;
        }

        public ApiResponse<List<Service>> GetAllServices()
        {
            try
            {
                var services = _db.QueryStoredProcedure<Service>("sp_GetAllServices").ToList();
                return services.Any()
                    ? ApiResponse<List<Service>>.SuccessResponse(services, "Lấy danh sách dịch vụ thành công")
                    : ApiResponse<List<Service>>.ErrorResponse("Không có dịch vụ nào");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<Service>>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Service> GetServiceById(int maDichVu)
        {
            try
            {
                var service = _db.QueryFirstOrDefaultStoredProcedure<Service>(
                    "sp_GetServiceById",
                    new { MaDichVu = maDichVu }
                );

                return service != null
                    ? ApiResponse<Service>.SuccessResponse(service, "Lấy dịch vụ thành công")
                    : ApiResponse<Service>.ErrorResponse("Không tìm thấy dịch vụ");
            }
            catch (Exception ex)
            {
                return ApiResponse<Service>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Service> CreateService(AddService service)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(service.TenDichVu))
            {
                return ApiResponse<Service>.ErrorResponse("Tên dịch vụ không được để trống");
            }

            if (service.Gia <= 0)
            {
                return ApiResponse<Service>.ErrorResponse("Giá dịch vụ phải lớn hơn 0");
            }

            try
            {
                var result = _db.QueryFirstOrDefault<Service>(
                    "sp_CreateService",
                    new
                    {
                        TenDichVu = service.TenDichVu,
                        MaLoaiDV = service.MaLoaiDV,
                        Gia = service.Gia
                    }
                );

                return result != null
                    ? ApiResponse<Service>.SuccessResponse(result, "Tạo dịch vụ thành công")
                    : ApiResponse<Service>.ErrorResponse("Tạo dịch vụ thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<Service>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Service> UpdateService(int maDichVu, UpdateService service)
        {
            // Validation
            if (maDichVu <= 0)
            {
                return ApiResponse<Service>.ErrorResponse("Mã dịch vụ không hợp lệ");
            }

            try
            {
                var result = _db.QueryFirstOrDefault<Service>(
                    "sp_UpdateService",
                    new
                    {
                        MaDichVu = maDichVu,
                        TenDichVu = service.TenDichVu,
                        MaLoaiDV = service.MaLoaiDV,
                        Gia = service.Gia
                    }
                );

                return result != null
                    ? ApiResponse<Service>.SuccessResponse(result, "Cập nhật dịch vụ thành công")
                    : ApiResponse<Service>.ErrorResponse("Cập nhật dịch vụ thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<Service>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<int> DeleteService(int maDichVu)
        {
            // Validation
            if (maDichVu <= 0)
            {
                return ApiResponse<int>.ErrorResponse("Mã dịch vụ không hợp lệ");
            }

            try
            {
                int result = _db.ExecuteStoredProcedure(
                    "sp_DeleteService",
                    new { MaDichVu = maDichVu }
                );

                return result == -1
                    ? ApiResponse<int>.SuccessResponse(result, "Xóa dịch vụ thành công")
                    : ApiResponse<int>.ErrorResponse("Xóa dịch vụ thất bại");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }
    }
}
