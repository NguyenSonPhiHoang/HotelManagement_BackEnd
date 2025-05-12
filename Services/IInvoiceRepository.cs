using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IInvoiceRepository
    {
        Task<int> CreateAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(int maHoaDon);
        Task<Invoice> GetByIdAsync(int maHoaDon);
        Task<(IEnumerable<Invoice> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null);
    }
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DatabaseDapper _db;

        public InvoiceRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Invoice invoice)
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

            return await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Invoice_Create", parameters);
        }

        public async Task UpdateAsync(Invoice invoice)
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

            await _db.ExecuteStoredProcedureAsync("sp_Invoice_Update", parameters);
        }

        public async Task DeleteAsync(int maHoaDon)
        {
            await _db.ExecuteStoredProcedureAsync("sp_Invoice_Delete", new { MaHoaDon = maHoaDon });
        }

        public async Task<Invoice> GetByIdAsync(int maHoaDon)
        {
            return await _db.QueryFirstOrDefaultStoredProcedureAsync<Invoice>("sp_Invoice_GetById", new { MaHoaDon = maHoaDon });
        }

        public async Task<(IEnumerable<Invoice> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            using var reader = await _db.QueryMultipleAsync("sp_Invoice_GetAll",
                new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

            var items = await reader.ReadAsync<Invoice>();
            var totalCount = await reader.ReadSingleAsync<int>();

            return (items, totalCount);
        }
    }
}