using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IBookingRepository
    {
        Task<ApiResponse<int>> CreateAsync(Booking booking);
        Task<ApiResponse<bool>> UpdateAsync(Booking booking);
        Task<ApiResponse<bool>> DeleteAsync(int maDatPhong);
        Task<ApiResponse<Booking>> GetByIdAsync(int maDatPhong);
        Task<(ApiResponse<IEnumerable<Booking>> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaDatPhong", string? sortOrder = "ASC");
        Task<ApiResponse<Booking>> SearchByMaDatPhongAsync(int maDatPhong);
    }

    public class BookingRepository : IBookingRepository
    {
        private readonly DatabaseDapper _db;

        public BookingRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<int>> CreateAsync(Booking booking)
        {
            try
            {
                var parameters = new
                {
                    booking.MaKhachHang,
                    booking.MaPhong,
                    booking.GioCheckIn,
                    booking.GioCheckOut,
                    booking.TrangThai,
                    booking.NgayDat
                };

                var maDatPhong = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Booking_Create", parameters);
                return ApiResponse<int>.SuccessResponse(maDatPhong, "Tạo đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Booking booking)
        {
            try
            {
                var parameters = new
                {
                    booking.MaDatPhong,
                    booking.MaKhachHang,
                    booking.MaPhong,
                    booking.GioCheckIn,
                    booking.GioCheckOut,
                    booking.TrangThai,
                    booking.NgayDat
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Booking_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật đặt phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maDatPhong)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Booking_Delete", new { MaDatPhong = maDatPhong });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa đặt phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Booking>> GetByIdAsync(int maDatPhong)
        {
            try
            {
                var booking = await _db.QueryFirstOrDefaultStoredProcedureAsync<Booking>("sp_Booking_GetById", new { MaDatPhong = maDatPhong });
                if (booking == null)
                    return ApiResponse<Booking>.ErrorResponse("Không tìm thấy đặt phòng");

                return ApiResponse<Booking>.SuccessResponse(booking, "Lấy thông tin đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Booking>.ErrorResponse($"Lỗi khi lấy thông tin đặt phòng: {ex.Message}");
            }
        }

        public async Task<(ApiResponse<IEnumerable<Booking>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaDatPhong", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_Booking_GetAll",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<Booking>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<Booking>>.SuccessResponse(items, "Lấy danh sách đặt phòng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<Booking>>.ErrorResponse($"Lỗi khi lấy danh sách đặt phòng: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<Booking>> SearchByMaDatPhongAsync(int maDatPhong)
        {
            try
            {
                var booking = await _db.QueryFirstOrDefaultStoredProcedureAsync<Booking>("sp_Booking_SearchByMaDatPhong", new { MaDatPhong = maDatPhong });
                if (booking == null)
                    return ApiResponse<Booking>.ErrorResponse($"Không tìm thấy đặt phòng với mã {maDatPhong}");

                return ApiResponse<Booking>.SuccessResponse(booking, "Tìm kiếm đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Booking>.ErrorResponse($"Lỗi khi tìm kiếm đặt phòng: {ex.Message}");
            }
        }
    }
}