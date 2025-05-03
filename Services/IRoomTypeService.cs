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
        ApiResponse<List<RoomType>> GetAllRoomTypes();
    }
    public class RoomTypeService : IRoomTypeService
    {
        private readonly DatabaseDapper _db;

        public RoomTypeService(DatabaseDapper db)
        {
            _db = db;
        }

        public ApiResponse<List<RoomType>> GetAllRoomTypes()
        {
            var roomTypes = _db.QueryStoredProcedure<RoomType>("sp_RoomType_GetAll");
            return ApiResponse<List<RoomType>>.SuccessResponse(roomTypes.ToList(), "Lấy loại phòng thành công");
        }
    }

}