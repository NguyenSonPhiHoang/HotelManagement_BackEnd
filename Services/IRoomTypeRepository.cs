using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IRoomTypeRepository
    {
        Task<(ApiResponse<List<RoomType>> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ApiResponse<RoomType>> GetByIdAsync(int maLoaiPhong);
        Task<ApiResponse<int>> CreateAsync(RoomType roomType);
        Task<ApiResponse<int>> UpdateAsync(RoomType roomType);
        Task<ApiResponse<int>> DeleteAsync(int maLoaiPhong);
    }

    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly DatabaseDapper _db;

        public RoomTypeRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<(ApiResponse<List<RoomType>> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_RoomType_GetAll",
                    new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

                var items = (await reader.ReadAsync<RoomType>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<List<RoomType>>.SuccessResponse(items, "Lấy danh sách loại phòng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<List<RoomType>>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message), 0);
            }
        }

        public async Task<ApiResponse<RoomType>> GetByIdAsync(int maLoaiPhong)
        {
            try
            {
                var roomType = await _db.QueryFirstOrDefaultStoredProcedureAsync<RoomType>("sp_RoomType_GetById", new { MaLoaiPhong = maLoaiPhong });

                if (roomType == null)
                {
                    return ApiResponse<RoomType>.ErrorResponse($"Không tìm thấy loại phòng với mã {maLoaiPhong}");
                }

                return ApiResponse<RoomType>.SuccessResponse(roomType, "Lấy thông tin loại phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<RoomType>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
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
                return ApiResponse<int>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<ApiResponse<int>> UpdateAsync(RoomType roomType)
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

                var maLoaiPhong = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_RoomType_Update", parameters);
                return ApiResponse<int>.SuccessResponse(maLoaiPhong, "Cập nhật loại phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<ApiResponse<int>> DeleteAsync(int maLoaiPhong)
        {
            try
            {
                await _db.ExecuteStoredProcedureAsync("sp_RoomType_Delete", new { MaLoaiPhong = maLoaiPhong });
                return ApiResponse<int>.SuccessResponse(maLoaiPhong, "Xóa loại phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}