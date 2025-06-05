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
        Task<ApiResponse<bool>> UpdateAsync(int maDatPhong, BookingUpdateRequest BookingUpdateRequest);
        Task<ApiResponse<bool>> DeleteAsync(int maDatPhong);
        Task<ApiResponse<Booking>> GetByIdAsync(int maDatPhong);
        Task<(ApiResponse<IEnumerable<Booking>> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaDatPhong", string? sortOrder = "ASC");
        Task<ApiResponse<bool>> UpdateStatusAsync(int maDatPhong, string trangThai);
        Task<ApiResponse<bool>> CompleteOrCancelBookingAsync(int maDatPhong, string action);
        Task<bool> UpdateRoomStatusInTransaction(int maPhong, string trangThai, string? tinhTrang = null);
    }

    public class BookingRepository : IBookingRepository
    {
        private readonly DatabaseDapper _db;
        private readonly IRoomRepository _roomRepository;


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

                // Kiểm tra LoaiTinhTien
                if (string.IsNullOrWhiteSpace(booking.LoaiTinhTien) ||
                    !new[] { "Hourly", "Nightly" }.Contains(booking.LoaiTinhTien.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Loại tính tiền không hợp lệ: {booking.LoaiTinhTien}");
                    return ApiResponse<int>.ErrorResponse("Loại tính tiền không hợp lệ");
                }

                // *** BƯỚC MỚI: Kiểm tra phòng có thể đặt được không ***
                var availabilityCheck = await _roomRepository.CheckRoomAvailabilityAsync(booking.MaPhong);
                if (!availabilityCheck.Success)
                {
                    Console.WriteLine($"Lỗi kiểm tra phòng: {availabilityCheck.Message}");
                    return ApiResponse<int>.ErrorResponse(availabilityCheck.Message);
                }

                if (!availabilityCheck.Data.IsAvailable)
                {
                    Console.WriteLine($"Phòng không thể đặt: {availabilityCheck.Data.Message}");
                    return ApiResponse<int>.ErrorResponse(availabilityCheck.Data.Message);
                }

                // Lấy giá phòng từ thông tin phòng đã kiểm tra
                var room = availabilityCheck.Data.RoomInfo;

                // Tính tổng tiền
                decimal tongTien = CalculateTotalPrice(booking, room.GiaPhong, booking.LoaiTinhTien);
                booking.TongTien = tongTien;
                booking.TrangThai = "Pending";

                // *** SỬ DỤNG TRANSACTION để đảm bảo tính nhất quán ***
                var transaction = await _db.BeginTransactionAsync();
                try
                {
                    // Tạo booking
                    var parameters = new
                    {
                        booking.MaKhachHang,
                        booking.MaPhong,
                        booking.GioCheckIn,
                        booking.GioCheckOut,
                        booking.TrangThai,
                        booking.NgayDat,
                        booking.TongTien,
                        booking.LoaiTinhTien
                    };

                    Console.WriteLine($"Tạo booking với MaPhong: {booking.MaPhong}");
                    var maDatPhong = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>(
                        "sp_Booking_Create", parameters);

                    if (maDatPhong <= 0)
                    {
                        _db.RollbackTransaction();
                        Console.WriteLine("Tạo đặt phòng thất bại");
                        return ApiResponse<int>.ErrorResponse("Tạo đặt phòng thất bại");
                    }

                    // Cập nhật trạng thái phòng thành "Đã đặt"
                    var updateRoomResult = await UpdateRoomStatusInTransaction(
                        booking.MaPhong, "Đã đặt", null);

                    if (!updateRoomResult)
                    {
                        _db.RollbackTransaction();
                        Console.WriteLine("Cập nhật trạng thái phòng thất bại");
                        return ApiResponse<int>.ErrorResponse("Cập nhật trạng thái phòng thất bại");
                    }

                    _db.CommitTransaction();

                    Console.WriteLine($"Đặt phòng thành công: MaDatPhong = {maDatPhong}, Phòng {room.SoPhong} đã chuyển sang trạng thái 'Đã đặt'");
                    return ApiResponse<int>.SuccessResponse(maDatPhong, "Đặt phòng thành công");
                }
                catch (Exception ex)
                {
                    _db.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo đặt phòng: {ex.Message}");
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo đặt phòng: {ex.Message}");
            }
        }

        // Method để cập nhật trạng thái phòng trong transaction
        public async Task<bool> UpdateRoomStatusInTransaction(int maPhong, string trangThai, string? tinhTrang)
        {
            try
            {
                var parameters = new
                {
                    MaPhong = maPhong,
                    TrangThai = trangThai,
                    TinhTrang = tinhTrang
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Room_UpdateStatus", parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật trạng thái phòng: {ex.Message}");
                return false;
            }
        }

        // Method để xử lý khi thanh toán hoặc hủy booking
        public async Task<ApiResponse<bool>> CompleteOrCancelBookingAsync(int maDatPhong, string action)
        {
            try
            {
                // Lấy thông tin booking
                var booking = await _db.QueryFirstOrDefaultStoredProcedureAsync<Booking>(
                    "sp_Booking_GetById", new { MaDatPhong = maDatPhong });

                if (booking == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Không tìm thấy đặt phòng");
                }

                var transaction = await _db.BeginTransactionAsync();
                try
                {
                    // Cập nhật trạng thái booking
                    string newBookingStatus = action == "complete" ? "Completed" : "Cancelled";
                    await _db.ExecuteStoredProcedureAsync("sp_Booking_UpdateStatus",
                        new { MaDatPhong = maDatPhong, TrangThai = newBookingStatus });

                    // Cập nhật trạng thái phòng về "Trống" và "Chưa dọn dẹp"
                    var updateRoomResult = await UpdateRoomStatusInTransaction(
                        booking.MaPhong, "Trống", "Chưa dọn dẹp");

                    if (!updateRoomResult)
                    {
                        _db.RollbackTransaction();
                        return ApiResponse<bool>.ErrorResponse("Cập nhật trạng thái phòng thất bại");
                    }

                    _db.CommitTransaction();

                    string actionText = action == "complete" ? "hoàn thành" : "hủy";
                    Console.WriteLine($"Đã {actionText} booking {maDatPhong}, phòng {booking.MaPhong} về trạng thái 'Trống - Chưa dọn dẹp'");
                    return ApiResponse<bool>.SuccessResponse(true, $"Đã {actionText} đặt phòng thành công");
                }
                catch (Exception ex)
                {
                    _db.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi {action} booking: {ex.Message}");
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xử lý đặt phòng: {ex.Message}");
            }
        }
        private decimal CalculateTotalPrice(Booking booking, decimal giaPhong, string loaiTinhTien)
        {
            double hours = (booking.GioCheckOut - booking.GioCheckIn).TotalHours;
            if (hours <= 0)
                return 0;

            if (loaiTinhTien == "Nightly" || hours > 12)
            {
                // Tính theo đêm
                int nights = (int)Math.Ceiling(hours / 24);
                return giaPhong * nights;
            }
            else
            {
                // Tính theo giờ
                decimal firstHourPrice = giaPhong * 0.2m; // 20% giá phòng
                decimal subsequentHourPrice = giaPhong * 0.1m; // 10% giá phòng/giờ
                int totalHours = (int)Math.Ceiling(hours);
                if (totalHours == 1)
                    return firstHourPrice;
                return firstHourPrice + (totalHours - 1) * subsequentHourPrice;
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int maDatPhong, BookingUpdateRequest bookingUpdateRequest)
        {
            try
            {
                if (bookingUpdateRequest == null || maDatPhong <= 0)
                {
                    Console.WriteLine("Dữ liệu đặt phòng không hợp lệ");
                    return ApiResponse<bool>.ErrorResponse("Dữ liệu đặt phòng không hợp lệ");
                }

                var parameters = new
                {
                    MaDatPhong = maDatPhong,
                    bookingUpdateRequest.MaPhong,
                    bookingUpdateRequest.GioCheckIn,
                    bookingUpdateRequest.GioCheckOut,
                    bookingUpdateRequest.LoaiTinhTien
                };

                Console.WriteLine($"Gọi sp_Booking_Update với MaDatPhong: {maDatPhong}");
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Booking_Update", parameters);

                if (rowsAffected <= 0)
                {
                    Console.WriteLine("Cập nhật đặt phòng thất bại");
                    return ApiResponse<bool>.ErrorResponse("Cập nhật đặt phòng thất bại");
                }

                Console.WriteLine($"Cập nhật đặt phòng thành công: MaDatPhong = {maDatPhong}");
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