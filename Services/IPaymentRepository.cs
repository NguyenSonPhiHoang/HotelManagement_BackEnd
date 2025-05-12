using HotelManagement.Model;
using HotelManagement.DataReader;

namespace HotelManagement.Services
{
    public interface IPaymentRepository
    {
        Task<int> CreateAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(int maThanhToan);
        Task<Payment> GetByIdAsync(int maThanhToan);
        Task<(IEnumerable<Payment> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null);
    }
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseDapper _db;

        public PaymentRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Payment payment)
        {
            var parameters = new
            {
                payment.MaHoaDon,
                payment.PhuongThucThanhToan,
                payment.SoDiemSuDung,
                payment.SoTienGiam,
                payment.ThanhTien,
                payment.NgayThanhToan
            };

            return await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_Payment_Create", parameters);
        }

        public async Task UpdateAsync(Payment payment)
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

            await _db.ExecuteStoredProcedureAsync("sp_Payment_Update", parameters);
        }

        public async Task DeleteAsync(int maThanhToan)
        {
            await _db.ExecuteStoredProcedureAsync("sp_Payment_Delete", new { MaThanhToan = maThanhToan });
        }

        public async Task<Payment> GetByIdAsync(int maThanhToan)
        {
            return await _db.QueryFirstOrDefaultStoredProcedureAsync<Payment>("sp_Payment_GetById", new { MaThanhToan = maThanhToan });
        }

        public async Task<(IEnumerable<Payment> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            using var reader = await _db.QueryMultipleAsync("sp_Payment_GetAll",
                new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

            var items = await reader.ReadAsync<Payment>();
            var totalCount = await reader.ReadSingleAsync<int>();

            return (items, totalCount);
        }
    }
}