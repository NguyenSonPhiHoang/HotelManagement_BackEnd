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
        Task<ApiResponse<bool>> UpdateStatusAsync(int maDatPhong, string trangThai);
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
                if (booking == null || booking.MaKhachHang <= 0 || booking.MaPhong <= 0)
                {
                    Console.WriteLine("Dữ liệu đặt phòng không hợp lệ");
                    return ApiResponse<int>.ErrorResponse("Dữ liệu đặt phòng không hợp lệ");
                }

                booking.TrangThai = "Pending"; // Đảm bảo trạng thái mặc định
                var parameters = new
                {
                    booking.MaKhachHang,
                    booking.MaPhong,
                    booking.GioCheckIn,
                    booking.GioCheckOut,
                    booking.TrangThai,
                    booking.NgayDat
                };

                Console.WriteLine($"Gọi sp_Booking_Create với MaKhachHang: {booking.MaKhachHang}, MaPhong: {booking.MaPhong}");
                var maDatPhong = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Booking_Create", parameters);
                if (maDatPhong <= 0)
                {
                    Console.WriteLine("Tạo đặt phòng thất bại");
                    return ApiResponse<int>.ErrorResponse("Tạo đặt phòng thất bại");
                }

                Console.WriteLine($"Tạo đặt phòng thành công: MaDatPhong = {maDatPhong}");
                return ApiResponse<int>.SuccessResponse(maDatPhong, "Tạo đặt phòng thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo đặt phòng: {ex.Message}");
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Booking booking)
        {
            try
            {
                if (booking == null || booking.MaDatPhong <= 0)
                {
                    Console.WriteLine("Dữ liệu đặt phòng không hợp lệ");
                    return ApiResponse<bool>.ErrorResponse("Dữ liệu đặt phòng không hợp lệ");
                }

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

                Console.WriteLine($"Gọi sp_Booking_Update với MaDatPhong: {booking.MaDatPhong}");
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Booking_Update", parameters);
                if (rowsAffected <= 0)
                {
                    Console.WriteLine("Cập nhật đặt phòng thất bại");
                    return ApiResponse<bool>.ErrorResponse("Cập nhật đặt phòng thất bại");
                }

                Console.WriteLine($"Cập nhật đặt phòng thành công: MaDatPhong = {booking.MaDatPhong}");
                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật đặt phòng thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật đặt phòng: {ex.Message}");
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maDatPhong)
        {
            try
            {
                if (maDatPhong <= 0)
                {
                    Console.WriteLine("Mã đặt phòng không hợp lệ");
                    return ApiResponse<bool>.ErrorResponse("Mã đặt phòng không hợp lệ");
                }

                Console.WriteLine($"Gọi sp_Booking_Delete với MaDatPhong: {maDatPhong}");
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Booking_Delete", new { MaDatPhong = maDatPhong });
                if (rowsAffected <= 0)
                {
                    Console.WriteLine("Xóa đặt phòng thất bại");
                    return ApiResponse<bool>.ErrorResponse("Xóa đặt phòng thất bại");
                }

                Console.WriteLine($"Xóa đặt phòng thành công: MaDatPhong = {maDatPhong}");
                return ApiResponse<bool>.SuccessResponse(true, "Xóa đặt phòng thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa đặt phòng: {ex.Message}");
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Booking>> GetByIdAsync(int maDatPhong)
        {
            try
            {
                if (maDatPhong <= 0)
                {
                    Console.WriteLine("Mã đặt phòng không hợp lệ");
                    return ApiResponse<Booking>.ErrorResponse("Mã đặt phòng không hợp lệ");
                }

                Console.WriteLine($"Gọi sp_Booking_GetById với MaDatPhong: {maDatPhong}");
                var booking = await _db.QueryFirstOrDefaultStoredProcedureAsync<Booking>("sp_Booking_GetById", new { MaDatPhong = maDatPhong });
                if (booking == null)
                {
                    Console.WriteLine($"Không tìm thấy đặt phòng với MaDatPhong: {maDatPhong}");
                    return ApiResponse<Booking>.ErrorResponse("Không tìm thấy đặt phòng");
                }

                Console.WriteLine($"Lấy thông tin đặt phòng thành công: MaDatPhong = {maDatPhong}");
                return ApiResponse<Booking>.SuccessResponse(booking, "Lấy thông tin đặt phòng thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin đặt phòng: {ex.Message}");
                return ApiResponse<Booking>.ErrorResponse($"Lỗi khi lấy thông tin đặt phòng: {ex.Message}");
            }
        }

        public async Task<(ApiResponse<IEnumerable<Booking>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaDatPhong", string? sortOrder = "ASC")
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    Console.WriteLine("Thông số phân trang không hợp lệ");
                    return (ApiResponse<IEnumerable<Booking>>.ErrorResponse("Thông số phân trang không hợp lệ"), 0);
                }

                Console.WriteLine($"Gọi sp_Booking_GetAll với PageNumber: {pageNumber}, PageSize: {pageSize}");
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

                Console.WriteLine($"Lấy danh sách đặt phòng thành công: {items.Count} mục, Tổng: {totalCount}");
                return (ApiResponse<IEnumerable<Booking>>.SuccessResponse(items, "Lấy danh sách đặt phòng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách đặt phòng: {ex.Message}");
                return (ApiResponse<IEnumerable<Booking>>.ErrorResponse($"Lỗi khi lấy danh sách đặt phòng: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<Booking>> SearchByMaDatPhongAsync(int maDatPhong)
        {
            try
            {
                if (maDatPhong <= 0)
                {
                    Console.WriteLine("Mã đặt phòng không hợp lệ");
                    return ApiResponse<Booking>.ErrorResponse("Mã đặt phòng không hợp lệ");
                }

                Console.WriteLine($"Gọi sp_Booking_SearchByMaDatPhong với MaDatPhong: {maDatPhong}");
                var booking = await _db.QueryFirstOrDefaultStoredProcedureAsync<Booking>("sp_Booking_SearchByMaDatPhong", new { MaDatPhong = maDatPhong });
                if (booking == null)
                {
                    Console.WriteLine($"Không tìm thấy đặt phòng với MaDatPhong: {maDatPhong}");
                    return ApiResponse<Booking>.ErrorResponse($"Không tìm thấy đặt phòng với mã {maDatPhong}");
                }

                Console.WriteLine($"Tìm kiếm đặt phòng thành công: MaDatPhong = {maDatPhong}");
                return ApiResponse<Booking>.SuccessResponse(booking, "Tìm kiếm đặt phòng thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm đặt phòng: {ex.Message}");
                return ApiResponse<Booking>.ErrorResponse($"Lỗi khi tìm kiếm đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int maDatPhong, string trangThai)
        {
            try
            {
                if (maDatPhong <= 0)
                {
                    Console.WriteLine("Mã đặt phòng không hợp lệ");
                    return ApiResponse<bool>.ErrorResponse("Mã đặt phòng không hợp lệ");
                }

                if (!new[] { "Pending", "Confirmed", "Cancelled" }.Contains(trangThai))
                {
                    Console.WriteLine($"Trạng thái không hợp lệ: {trangThai}");
                    return ApiResponse<bool>.ErrorResponse("Trạng thái không hợp lệ");
                }

                Console.WriteLine($"Gọi sp_Booking_UpdateStatus với MaDatPhong: {maDatPhong}, TrangThai: {trangThai}");
                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Booking_UpdateStatus",
                    new { MaDatPhong = maDatPhong, TrangThai = trangThai });
                if (result <= 0)
                {
                    Console.WriteLine($"Cập nhật trạng thái thất bại cho MaDatPhong: {maDatPhong}");
                    return ApiResponse<bool>.ErrorResponse("Cập nhật trạng thái thất bại");
                }

                Console.WriteLine($"Cập nhật trạng thái thành công cho MaDatPhong: {maDatPhong}, TrangThai: {trangThai}");
                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật trạng thái thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật trạng thái: {ex.Message}");
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }
    }
}