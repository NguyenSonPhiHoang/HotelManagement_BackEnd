using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IInvoiceRepository
    {
        Task<ApiResponse<int>> CreateAsync(Invoice invoice);
        Task<ApiResponse<bool>> UpdateAsync(Invoice invoice);
        Task<ApiResponse<bool>> DeleteAsync(int maHoaDon);
        Task<ApiResponse<Invoice>> GetByIdAsync(int maHoaDon);
        Task<(ApiResponse<IEnumerable<Invoice>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaHoaDon", string? sortOrder = "ASC");
    }

    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DatabaseDapper _db;

        public InvoiceRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<int>> CreateAsync(Invoice invoice)
        {
            try
            {
                var parameters = new
                {
                    invoice.MaDatPhong,
                    invoice.MaKhachHang,
                    invoice.TongTienPhong,
                    invoice.TongTienDichVu,
                    invoice.TongThanhTien,
                    invoice.TrangThai
                };

                var maHoaDon = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Invoice_Create", parameters);
                return ApiResponse<int>.SuccessResponse(maHoaDon, "Tạo hóa đơn thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo hóa đơn: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Invoice invoice)
        {
            try
            {
                var parameters = new
                {
                    invoice.MaHoaDon,
                    invoice.MaDatPhong,
                    invoice.MaKhachHang,
                    invoice.TongTienPhong,
                    invoice.TongTienDichVu,
                    invoice.TongThanhTien,
                    invoice.TrangThai
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Invoice_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật hóa đơn thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật hóa đơn thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật hóa đơn: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maHoaDon)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Invoice_Delete", new { MaHoaDon = maHoaDon });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa hóa đơn thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa hóa đơn thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa hóa đơn: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Invoice>> GetByIdAsync(int maHoaDon)
        {
            try
            {
                var invoice = await _db.QueryFirstOrDefaultStoredProcedureAsync<Invoice>("sp_Invoice_GetById", new { MaHoaDon = maHoaDon });
                if (invoice == null)
                    return ApiResponse<Invoice>.ErrorResponse("Không tìm thấy hóa đơn");

                return ApiResponse<Invoice>.SuccessResponse(invoice, "Lấy thông tin hóa đơn thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Invoice>.ErrorResponse($"Lỗi khi lấy thông tin hóa đơn: {ex.Message}");
            }
        }

        public async Task<(ApiResponse<IEnumerable<Invoice>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaHoaDon", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_Invoice_GetAll",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<Invoice>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<Invoice>>.SuccessResponse(items, "Lấy danh sách hóa đơn thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<Invoice>>.ErrorResponse($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}"), 0);
            }
        }
    }
}