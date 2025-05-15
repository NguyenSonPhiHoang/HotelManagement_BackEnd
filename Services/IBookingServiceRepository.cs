using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IBookingServiceRepository
    {
        Task<ApiResponse<int>> CreateAsync(BookingService bookingService);
        Task<ApiResponse<bool>> UpdateAsync(BookingService bookingService);
        Task<ApiResponse<bool>> DeleteAsync(int maBSD);
        Task<ApiResponse<BookingService>> GetByIdAsync(int maBSD);
        Task<(ApiResponse<IEnumerable<BookingService>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null);
    }

    public class BookingServiceRepository : IBookingServiceRepository
    {
        private readonly DatabaseDapper _db;

        public BookingServiceRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<int>> CreateAsync(BookingService bookingService)
        {
            try
            {
                var parameters = new
                {
                    bookingService.MaDatPhong,
                    bookingService.MaDichVu,
                    bookingService.SoLuong,
                    bookingService.Gia,
                    bookingService.ThanhTien,
                    bookingService.NgaySuDung
                };

                var maBSD = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_BookingService_Create", parameters);
                return ApiResponse<int>.SuccessResponse(maBSD, "Tạo dịch vụ đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo dịch vụ đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(BookingService bookingService)
        {
            try
            {
                var parameters = new
                {
                    bookingService.MaBSD,
                    bookingService.MaDatPhong,
                    bookingService.MaDichVu,
                    bookingService.SoLuong,
                    bookingService.Gia,
                    bookingService.ThanhTien,
                    bookingService.NgaySuDung
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_BookingService_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật dịch vụ đặt phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật dịch vụ đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật dịch vụ đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maBSD)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_BookingService_Delete", new { MaBSD = maBSD });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa dịch vụ đặt phòng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa dịch vụ đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa dịch vụ đặt phòng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<BookingService>> GetByIdAsync(int maBSD)
        {
            try
            {
                var bookingService = await _db.QueryFirstOrDefaultStoredProcedureAsync<BookingService>("sp_BookingService_GetById", new { MaBSD = maBSD });
                if (bookingService == null)
                    return ApiResponse<BookingService>.ErrorResponse("Không tìm thấy dịch vụ đặt phòng");

                return ApiResponse<BookingService>.SuccessResponse(bookingService, "Lấy thông tin dịch vụ đặt phòng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<BookingService>.ErrorResponse($"Lỗi khi lấy thông tin dịch vụ đặt phòng: {ex.Message}");
            }
        }

        public async Task<(ApiResponse<IEnumerable<BookingService>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_BookingService_GetAll",
                    new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

                var items = (await reader.ReadAsync<BookingService>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<BookingService>>.SuccessResponse(items, "Lấy danh sách dịch vụ đặt phòng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<BookingService>>.ErrorResponse($"Lỗi khi lấy danh sách dịch vụ đặt phòng: {ex.Message}"), 0);
            }
        }
    }
}