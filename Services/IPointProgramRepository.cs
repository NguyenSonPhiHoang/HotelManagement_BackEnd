using HotelManagement.Model;
using HotelManagement.DataReader;

namespace HotelManagement.Services
{
    public interface IPointProgramRepository
    {
        Task<int> CreateAsync(PointProgram pointProgram);
        Task UpdateAsync(PointProgram pointProgram);
        Task DeleteAsync(int maCT);
        Task<PointProgram> GetByIdAsync(int maCT);
        Task<(IEnumerable<PointProgram> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null);
    }
    public class PointProgramRepository : IPointProgramRepository
    {
        private readonly DatabaseDapper _db;

        public PointProgramRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(PointProgram pointProgram)
        {
            var parameters = new
            {
                pointProgram.TenCT,
                pointProgram.DiemToiThieu,
                pointProgram.MucGiamGia
            };

            return await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_PointProgram_Create", parameters);
        }

        public async Task UpdateAsync(PointProgram pointProgram)
        {
            var parameters = new
            {
                pointProgram.MaCT,
                pointProgram.TenCT,
                pointProgram.DiemToiThieu,
                pointProgram.MucGiamGia
            };

            await _db.ExecuteStoredProcedureAsync("sp_PointProgram_Update", parameters);
        }

        public async Task DeleteAsync(int maCT)
        {
            await _db.ExecuteStoredProcedureAsync("sp_PointProgram_Delete", new { MaCT = maCT });
        }

        public async Task<PointProgram> GetByIdAsync(int maCT)
        {
            return await _db.QueryFirstOrDefaultStoredProcedureAsync<PointProgram>("sp_PointProgram_GetById", new { MaCT = maCT });
        }

        public async Task<(IEnumerable<PointProgram> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            using var reader = await _db.QueryMultipleAsync("sp_PointProgram_GetAll",
                new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

            var items = await reader.ReadAsync<PointProgram>();
            var totalCount = await reader.ReadSingleAsync<int>();

            return (items, totalCount);
        }
    }
}