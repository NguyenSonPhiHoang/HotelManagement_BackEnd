using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IRoomRepository
    {
        Task<ApiResponse<IEnumerable<Room>>> GetRoomsByFilterAsync(string trangThai, string loaiPhong, string tinhTrang);
        Task<ApiResponse<Room>> GetRoomBySoPhongAsync(string soPhong);
        Task<(ApiResponse<IEnumerable<Room>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? trangThai = null, 
            int? loaiPhong = null, string? tinhTrang = null, string? sortBy = "MaPhong", string? sortOrder = "ASC");
        Task<ApiResponse<Room>> GetByIdAsync(int maPhong);
        Task<ApiResponse<int>> CreateAsync(Room room);
        Task<ApiResponse<bool>> UpdateAsync(Room room);
        Task<ApiResponse<bool>> DeleteAsync(int maPhong);
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly DatabaseDapper _db;

        public RoomRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<IEnumerable<Room>>> GetRoomsByFilterAsync(string trangThai, string loaiPhong, string tinhTrang)
        {
            try
            {
                var parameters = new
                {
                    TrangThai = trangThai,
                    LoaiPhong = loaiPhong,
                    TinhTrang = tinhTrang
                };

                var rooms = await _db.QueryStoredProcedureAsync<Room>("sp_Room_Filter", parameters);
                return ApiResponse<IEnumerable<Room>>.SuccessResponse(rooms, "Lọc phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Room>>.ErrorResponse($"Lỗi khi lọc phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Room>> GetRoomBySoPhongAsync(string soPhong)
        {
            try
            {
                var room = await _db.QueryFirstOrDefaultStoredProcedureAsync<Room>("sp_Room_SearchBySoPhong", new { SoPhong = soPhong });
                if (room == null)
                    return ApiResponse<Room>.ErrorResponse($"Không tìm thấy phòng có số {soPhong}");

                return ApiResponse<Room>.SuccessResponse(room, "Tìm phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Room>.ErrorResponse($"Lỗi khi tìm phòng: {ex.Message}");
            }
        }

        public async Task<(ApiResponse<IEnumerable<Room>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? trangThai = null, 
            int? loaiPhong = null, string? tinhTrang = null, string? sortBy = "MaPhong", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_Room_GetAll",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        TrangThai = trangThai,
                        LoaiPhong = loaiPhong,
                        TinhTrang = tinhTrang,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<Room>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<Room>>.SuccessResponse(items, "Lấy danh sách phòng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<Room>>.ErrorResponse($"Lỗi khi lấy danh sách phòng: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<Room>> GetByIdAsync(int maPhong)
        {
            try
            {
                var room = await _db.QueryFirstOrDefaultStoredProcedureAsync<Room>("sp_Room_GetById", new { MaPhong = maPhong });
                if (room == null)
                    return ApiResponse<Room>.ErrorResponse($"Không tìm thấy phòng với mã {maPhong}");

                return ApiResponse<Room>.SuccessResponse(room, "Lấy thông tin phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Room>.ErrorResponse($"Lỗi khi lấy thông tin phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(Room room)
        {
            try
            {
                var parameters = new
                {
                    room.SoPhong,
                    room.LoaiPhong,
                    room.GiaPhong,
                    room.TrangThai,
                    room.TinhTrang
                };

                var maPhong = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Room_Create", parameters);
                return ApiResponse<int>.SuccessResponse(maPhong, "Tạo phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Room room)
        {
            try
            {
                var parameters = new
                {
                    room.MaPhong,
                    room.SoPhong,
                    room.LoaiPhong,
                    room.GiaPhong,
                    room.TrangThai,
                    room.TinhTrang
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Room_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maPhong)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Room_Delete", new { MaPhong = maPhong });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa phòng: {ex.Message}");
            }
        }
    }
}