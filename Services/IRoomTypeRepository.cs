using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IRoomTypeRepository
    {
        Task<(ApiResponse<IEnumerable<RoomType>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null);
        Task<ApiResponse<RoomType>> GetByIdAsync(int maLoaiPhong);
        Task<ApiResponse<int>> CreateAsync(RoomType roomType);
        Task<ApiResponse<bool>> UpdateAsync(RoomType roomType);
        Task<ApiResponse<bool>> DeleteAsync(int maLoaiPhong);
    }

    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly DatabaseDapper _db;

        public RoomTypeRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<(ApiResponse<IEnumerable<RoomType>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_RoomType_GetAll",
                    new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

                var items = (await reader.ReadAsync<RoomType>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<RoomType>>.SuccessResponse(items, "Lấy danh sách loại phòng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<RoomType>>.ErrorResponse($"Lỗi khi lấy danh sách loại phòng: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<RoomType>> GetByIdAsync(int maLoaiPhong)
        {
            try
            {
                var roomType = await _db.QueryFirstOrDefaultStoredProcedureAsync<RoomType>("sp_RoomType_GetById", new { MaLoaiPhong = maLoaiPhong });
                if (roomType == null)
                    return ApiResponse<RoomType>.ErrorResponse($"Không tìm thấy loại phòng với mã {maLoaiPhong}");

                return ApiResponse<RoomType>.SuccessResponse(roomType, "Lấy thông tin loại phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoomType>.ErrorResponse($"Lỗi khi lấy thông tin loại phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(RoomType roomType)
        {
            try
            {
                var parameters = new
                {
                    roomType.TenLoaiPhong,
                    roomType.GiaPhong,
                    roomType.MoTa
                };

                var maLoaiPhong = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_RoomType_Create", parameters);
                return ApiResponse<int>.SuccessResponse(maLoaiPhong, "Tạo loại phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo loại phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(RoomType roomType)
        {
            try
            {
                var parameters = new
                {
                    roomType.MaLoaiPhong,
                    roomType.TenLoaiPhong,
                    roomType.GiaPhong,
                    roomType.MoTa
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_RoomType_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật loại phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật loại phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật loại phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maLoaiPhong)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_RoomType_Delete", new { MaLoaiPhong = maLoaiPhong });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa loại phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa loại phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa loại phòng: {ex.Message}");
            }
        }
    }
}