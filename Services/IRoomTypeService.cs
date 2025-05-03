using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagement.DataReader;
using HotelManagement.Model;
using HotelManagement_BackEnd.Model;

namespace HotelManagement_BackEnd.Services
{
    public interface IRoomTypeService
    {
        ApiResponse<List<RoomType>> GetAllAsync();
        ApiResponse<RoomType> GetByIdAsync(int id);
        ApiResponse<RoomType> CreateAsync(AddRoomType roomType);
        ApiResponse<RoomType> UpdateAsync(int id, AddRoomType roomType);
        ApiResponse<string> DeleteAsync(int id);
    }

    public class RoomTypeService : IRoomTypeService
    {
        private readonly DatabaseDapper _db;
        private readonly ILogger<RoomTypeService> _logger;

        public RoomTypeService(DatabaseDapper db, ILogger<RoomTypeService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ApiResponse<RoomType> CreateAsync(AddRoomType dto)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TenLoaiPhong", dto.TenLoaiPhong);
                var roomType = _db.QueryFirstOrDefaultStoredProcedure<RoomType>("sp_CreateRoomType", parameters);
                return roomType != null ? ApiResponse<RoomType>.SuccessResponse(roomType, "Thanh cong") : ApiResponse<RoomType>.ErrorResponse("That bai");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all room types");
                return ApiResponse<RoomType>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<string> DeleteAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaLoaiPhong", id);

                var roomType = _db.ExecuteStoredProcedure("sp_DeleteRoomType", parameters);
                return roomType == -1 ? ApiResponse<string>.SuccessResponse("Thanh cong") : ApiResponse<string>.ErrorResponse("That bai");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all room types");
                return ApiResponse<string>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<List<RoomType>> GetAllAsync()
        {
            try
            {
                var roomTypes = _db.QueryStoredProcedure<RoomType>("sp_GetAllRoomTypes");
                return ApiResponse<List<RoomType>>.SuccessResponse([.. roomTypes], "Thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách loại phòng");
                return ApiResponse<List<RoomType>>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<RoomType> GetByIdAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaLoaiPhong", id);
                // var roomType = _db.QueryFirstOrDefaultStoredProcedure<RoomType>("sp_GetRoomTypeById", parameters);
                using var multi = _db.QueryMultiple("sp_GetRoomTypeById",parameters);
                var roomType = multi.ReadFirstOrDefault<RoomType>();
                var rooms = multi.Read<Room>().ToList();
                roomType.Rooms=rooms;
                return roomType != null
                    ? ApiResponse<RoomType>.SuccessResponse(roomType, "Thành công")
                    : ApiResponse<RoomType>.ErrorResponse("Không tìm thấy loại phòng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy loại phòng theo ID");
                return ApiResponse<RoomType>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<RoomType> UpdateAsync(int id, AddRoomType dto)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaLoaiPhong", id);
                parameters.Add("@TenLoaiPhong", dto.TenLoaiPhong);
                var roomType = _db.QueryFirstOrDefaultStoredProcedure<RoomType>("sp_UpdateRoomType", parameters);
                return roomType != null ? ApiResponse<RoomType>.SuccessResponse(roomType, "Thanh cong") : ApiResponse<RoomType>.ErrorResponse("That bai");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all room types");
                return ApiResponse<RoomType>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }
    }
}