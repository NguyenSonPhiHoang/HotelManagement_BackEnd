using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IPaymentRepository
    {
        Task<ApiResponse<int>> CreateAsync(PaymentCreateRequest request);
        Task<ApiResponse<bool>> UpdateAsync(Payment payment);
        Task<ApiResponse<bool>> DeleteAsync(int maThanhToan);
        Task<ApiResponse<Payment>> GetByIdAsync(int maThanhToan);
        Task<(ApiResponse<IEnumerable<Payment>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaThanhToan", string? sortOrder = "ASC");
        Task<ApiResponse<(int MaThanhToan, decimal SoTienGiam, int DiemTichLuy)>> ProcessPaymentAndPointsAsync(string maKhachHang, decimal thanhTien, int soDiemSuDung);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseDapper _db;

        public PaymentRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<int>> CreateAsync(PaymentCreateRequest request)
        {
            try
            {
                var parameters = new
                {
                    request.MaHoaDon,
                    request.PhuongThucThanhToan,
                    request.SoDiemSuDung,
                    request.SoTienGiam,
                    request.ThanhTien,
                    request.NgayThanhToan
                };

                var maThanhToan = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Payment_Create", parameters);
                return ApiResponse<int>.SuccessResponse(maThanhToan, "Tạo thanh toán thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo thanh toán: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Payment payment)
        {
            try
            {
                var parameters = new
                {
                    payment.MaThanhToan,
                    payment.MaHoaDon,
                    payment.PhuongThucThanhToan,
                    payment.SoDiemSuDung,
                    payment.SoTienGiam,
                    payment.ThanhTien,
                    payment.NgayThanhToan
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Payment_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật thanh toán thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật thanh toán thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật thanh toán: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maThanhToan)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Payment_Delete", new { MaThanhToan = maThanhToan });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa thanh toán thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa thanh toán thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa thanh toán: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Payment>> GetByIdAsync(int maThanhToan)
        {
            try
            {
                var payment = await _db.QueryFirstOrDefaultStoredProcedureAsync<Payment>("sp_Payment_GetById", new { MaThanhToan = maThanhToan });
                if (payment == null)
                    return ApiResponse<Payment>.ErrorResponse("Không tìm thấy thanh toán");

                return ApiResponse<Payment>.SuccessResponse(payment, "Lấy thông tin thanh toán thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Payment>.ErrorResponse($"Lỗi khi lấy thông tin thanh toán: {ex.Message}");
            }
        }

        public async Task<(ApiResponse<IEnumerable<Payment>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaThanhToan", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_Payment_GetAll",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<Payment>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<Payment>>.SuccessResponse(items, "Lấy danh sách thanh toán thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<Payment>>.ErrorResponse($"Lỗi khi lấy danh sách thanh toán: {ex.Message}"), 0);
            }
        }
        public async Task<ApiResponse<(int MaThanhToan, decimal SoTienGiam, int DiemTichLuy)>> ProcessPaymentAndPointsAsync(string maKhachHang, decimal thanhTien, int soDiemSuDung)
        {
            try
            {
                if (!int.TryParse(maKhachHang, out int maKhachHangInt))
                    return ApiResponse<(int, decimal, int)>.ErrorResponse("Mã khách hàng không hợp lệ");

                if (thanhTien < 0 || soDiemSuDung < 0)
                    return ApiResponse<(int, decimal, int)>.ErrorResponse("Số tiền hoặc điểm sử dụng không hợp lệ");

                var parameters = new
                {
                    MaKhachHang = maKhachHangInt,
                    ThanhTien = thanhTien,
                    SoDiemSuDung = soDiemSuDung
                };

                var result = await _db.QueryFirstOrDefaultStoredProcedureAsync<(int MaThanhToan, decimal SoTienGiam, int DiemTichLuy)>(
                    "sp_ThanhToanVaTichDiem", parameters);

                return ApiResponse<(int, decimal, int)>.SuccessResponse(result, "Thanh toán và tích điểm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<(int, decimal, int)>.ErrorResponse($"Lỗi khi xử lý thanh toán và điểm: {ex.Message}");
            }
        }
    }
}