using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IRoomService
    {
        ApiResponse<List<Room>> GetRoomsByFilter(string trangThai, string loaiPhong, string tinhTrang);
        ApiResponse<Room> GetRoomBySoPhong(string soPhong);


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

        public ApiResponse<List<Room>> GetRoomsByFilter(string trangThai, string loaiPhong, string tinhTrang)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TrangThai", trangThai);
                parameters.Add("@LoaiPhong", loaiPhong);
                parameters.Add("@TinhTrang", tinhTrang);

                var rooms = _db.QueryStoredProcedure<Room>("sp_Room_Filter", parameters);
                return ApiResponse<List<Room>>.SuccessResponse(rooms.ToList(), "Lọc phòng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phòng");
                return ApiResponse<List<Room>>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public ApiResponse<Room> GetRoomBySoPhong(string soPhong)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SoPhong", soPhong);

                var room = _db.QueryFirstOrDefaultStoredProcedure<Room>("sp_Room_SearchBySoPhong", parameters);

                if (room == null)
                {
                    return ApiResponse<Room>.ErrorResponse($"Không tìm thấy phòng có số {soPhong}");
                }

                return ApiResponse<Room>.SuccessResponse(room, "Tìm phòng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tìm phòng với số phòng {soPhong}");
                return ApiResponse<Room>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }

}