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
        ApiResponse<List<Room>> GetRoomsByFilter(string trangThai, string loaiPhong, string tinhTrang);
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

                var rooms = _db.QueryStoredProcedure<Room>("sp_GetRoomsByFilter", parameters);
                return ApiResponse<List<Room>>.SuccessResponse(rooms.ToList(), "Lọc phòng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phòng");
                return ApiResponse<List<Room>>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }

}