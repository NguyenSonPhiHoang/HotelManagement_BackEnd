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
    public interface IRoomService
    {
        ApiResponse<List<Room>> GetAllAsync();
        ApiResponse<Room> GetByIdAsync(int id);
        ApiResponse<Room> CreateAsync(AddRoom room);
        ApiResponse<Room> UpdateAsync(int id, AddRoom room);
        ApiResponse<bool> DeleteAsync(int id);
    }

    public class RoomService : IRoomService
    {
        private readonly DatabaseDapper _db;
        private readonly ILogger<RoomService> _logger;

        public RoomService(DatabaseDapper db, ILogger<RoomService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ApiResponse<Room> CreateAsync(AddRoom room)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SoPhong", room.SoPhong);
                parameters.Add("@GiaPhong", room.GiaPhong);
                parameters.Add("@TrangThai", room.TrangThai);
                parameters.Add("@LoaiPhong", room.LoaiPhong);

                var result = _db.QueryFirstOrDefaultStoredProcedure<Room>("sp_CreateRoom", parameters);
                return result != null
                    ? ApiResponse<Room>.SuccessResponse(result, "Tạo phòng thành công")
                    : ApiResponse<Room>.ErrorResponse("Tạo thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phòng");
                return ApiResponse<Room>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> DeleteAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaPhong", id);

                var result = _db.ExecuteStoredProcedure("sp_DeleteRoom", parameters);
                return result == -1
                    ? ApiResponse<bool>.SuccessResponse(true, "Xoá thành công")
                    : ApiResponse<bool>.ErrorResponse("Xoá thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá phòng");
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<List<Room>> GetAllAsync()
        {
            try
            {
                var result = _db.QueryStoredProcedure<Room>("sp_GetAllRooms");
                return ApiResponse<List<Room>>.SuccessResponse([.. result], "Thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phòng");
                return ApiResponse<List<Room>>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Room> GetByIdAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaPhong", id);

                var room = _db.QueryFirstOrDefaultStoredProcedure<Room>("sp_GetRoomById", parameters);
                return room != null
                    ? ApiResponse<Room>.SuccessResponse(room, "Thành công")
                    : ApiResponse<Room>.ErrorResponse("Không tìm thấy phòng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phòng theo ID");
                return ApiResponse<Room>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Room> UpdateAsync(int id, AddRoom room)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@MaPhong", id);
                parameters.Add("@SoPhong", room.SoPhong);
                parameters.Add("@GiaPhong", room.GiaPhong);
                parameters.Add("@TrangThai", room.TrangThai);
                parameters.Add("@LoaiPhong", room.LoaiPhong);

                var result = _db.QueryFirstOrDefaultStoredProcedure<Room>("sp_UpdateRoom", parameters);
                return result != null
                    ? ApiResponse<Room>.SuccessResponse(result, "Cập nhật thành công")
                    : ApiResponse<Room>.ErrorResponse("Cập nhật thất bại"+result?.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật phòng");
                return ApiResponse<Room>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }
    }
}