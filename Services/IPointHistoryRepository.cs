using HotelManagement.Model;
using HotelManagement.DataReader;

namespace HotelManagement.Services
{
    public interface IPointHistoryRepository
    {
        Task<int> CreateAsync(PointHistory pointHistory);
        Task UpdateAsync(PointHistory pointHistory);
        Task DeleteAsync(int maLSTD);
        Task<PointHistory> GetByIdAsync(int maLSTD);
        Task<(IEnumerable<PointHistory> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null);
    }
    public class PointHistoryRepository : IPointHistoryRepository
    {
        private readonly DatabaseDapper _db;

        public PointHistoryRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(PointHistory pointHistory)
        {
            var parameters = new
            {
                pointHistory.MaKhachHang,
                pointHistory.SoDiem,
                pointHistory.NgayGiaoDich,
                pointHistory.LoaiGiaoDich
            };

            return await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_PointHistory_Create", parameters);
        }

        public async Task UpdateAsync(PointHistory pointHistory)
        {
            var parameters = new
            {
                pointHistory.MaLSTD,
                pointHistory.MaKhachHang,
                pointHistory.SoDiem,
                pointHistory.NgayGiaoDich,
                pointHistory.LoaiGiaoDich
            };

            await _db.ExecuteStoredProcedureAsync("sp_PointHistory_Update", parameters);
        }

        public async Task DeleteAsync(int maLSTD)
        {
            await _db.ExecuteStoredProcedureAsync("sp_PointHistory_Delete", new { MaLSTD = maLSTD });
        }

        public async Task<PointHistory> GetByIdAsync(int maLSTD)
        {
            return await _db.QueryFirstOrDefaultStoredProcedureAsync<PointHistory>("sp_PointHistory_GetById", new { MaLSTD = maLSTD });
        }

        public async Task<(IEnumerable<PointHistory> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            using var reader = await _db.QueryMultipleAsync("sp_PointHistory_GetAll",
                new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

            var items = await reader.ReadAsync<PointHistory>();
            var totalCount = await reader.ReadSingleAsync<int>();

            return (items, totalCount);
        }
    }
}