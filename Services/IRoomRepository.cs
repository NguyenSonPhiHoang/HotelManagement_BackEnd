using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IRoomRepository
    {
        Task<ApiResponse<List<Room>>> GetRoomsByFilter(string trangThai, string loaiPhong, string tinhTrang);
        Task<ApiResponse<Room>> GetRoomBySoPhong(string soPhong);
        Task<(ApiResponse<List<Room>> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, string? trangThai = null, int? loaiPhong = null, string? tinhTrang = null, string? sortBy = "MaPhong", string? sortOrder = "ASC");
        Task<ApiResponse<Room>> GetByIdAsync(int maPhong);
        Task<ApiResponse<int>> CreateAsync(Room room);
        Task<ApiResponse<int>> UpdateAsync(Room room);
        Task<ApiResponse<int>> DeleteAsync(int maPhong);
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly DatabaseDapper _db;

        public RoomRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<Room>>> GetRoomsByFilter(string trangThai, string loaiPhong, string tinhTrang)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TrangThai", trangThai);
                parameters.Add("@LoaiPhong", loaiPhong);
                parameters.Add("@TinhTrang", tinhTrang);

                var rooms = await _db.QueryStoredProcedureAsync<Room>("sp_Room_Filter", parameters);
                return ApiResponse<List<Room>>.SuccessResponse(rooms.ToList(), "Lọc phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<Room>>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<ApiResponse<Room>> GetRoomBySoPhong(string soPhong)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SoPhong", soPhong);

                var room = await _db.QueryFirstOrDefaultStoredProcedureAsync<Room>("sp_Room_SearchBySoPhong", parameters);

                if (room == null)
                {
                    return ApiResponse<Room>.ErrorResponse($"Không tìm thấy phòng có số {soPhong}");
                }

                return ApiResponse<Room>.SuccessResponse(room, "Tìm phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Room>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<(ApiResponse<List<Room>> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, string? trangThai = null, int? loaiPhong = null, string? tinhTrang = null, string? sortBy = "MaPhong", string? sortOrder = "ASC")
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

                return (ApiResponse<List<Room>>.SuccessResponse(items, "Lấy danh sách phòng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<List<Room>>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message), 0);
            }
        }

        public async Task<ApiResponse<Room>> GetByIdAsync(int maPhong)
        {
            try
            {
                var room = await _db.QueryFirstOrDefaultStoredProcedureAsync<Room>("sp_Room_GetById", new { MaPhong = maPhong });

                if (room == null)
                {
                    return ApiResponse<Room>.ErrorResponse($"Không tìm thấy phòng với mã {maPhong}");
                }

                return ApiResponse<Room>.SuccessResponse(room, "Lấy thông tin phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Room>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
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
                return ApiResponse<int>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<ApiResponse<int>> UpdateAsync(Room room)
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

                var maPhong = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Room_Update", parameters);
                return ApiResponse<int>.SuccessResponse(maPhong, "Cập nhật phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        public async Task<ApiResponse<int>> DeleteAsync(int maPhong)
        {
            try
            {
                await _db.ExecuteStoredProcedureAsync("sp_Room_Delete", new { MaPhong = maPhong });
                return ApiResponse<int>.SuccessResponse(maPhong, "Xóa phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse("Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}