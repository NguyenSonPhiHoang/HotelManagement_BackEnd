using HotelManagement.DataReader;
using HotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface IPointHistoryRepository
    {
        Task<ApiResponse<int>> CreateAsync(PointHistory pointHistory);
        Task<ApiResponse<bool>> UpdateAsync(PointHistory pointHistory);
        Task<ApiResponse<bool>> DeleteAsync(int maLSTD);
        Task<ApiResponse<PointHistory>> GetByIdAsync(int maLSTD);
        Task<(ApiResponse<IEnumerable<PointHistory>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null);
    }

    public class PointHistoryRepository : IPointHistoryRepository
    {
        private readonly DatabaseDapper _db;

        public PointHistoryRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<ApiResponse<int>> CreateAsync(PointHistory pointHistory)
        {
            try
            {
                var parameters = new
                {
                    pointHistory.MaKhachHang,
                    pointHistory.SoDiem,
                    pointHistory.NgayGiaoDich,
                    pointHistory.LoaiGiaoDich
                };

                var maLSTD = await _db.QueryFirstOrDefaultStoredProcedureAsync<int>("sp_PointHistory_Create", parameters);
                return ApiResponse<int>.SuccessResponse(maLSTD, "Tạo lịch sử điểm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Lỗi khi tạo lịch sử điểm: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(PointHistory pointHistory)
        {
            try
            {
                var parameters = new
                {
                    pointHistory.MaLSTD,
                    pointHistory.MaKhachHang,
                    pointHistory.SoDiem,
                    pointHistory.NgayGiaoDich,
                    pointHistory.LoaiGiaoDich
                };

                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_PointHistory_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật lịch sử điểm thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật lịch sử điểm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật lịch sử điểm: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int maLSTD)
        {
            try
            {
                var rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_PointHistory_Delete", new { MaLSTD = maLSTD });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa lịch sử điểm thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa lịch sử điểm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa lịch sử điểm: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PointHistory>> GetByIdAsync(int maLSTD)
        {
            try
            {
                var pointHistory = await _db.QueryFirstOrDefaultStoredProcedureAsync<PointHistory>("sp_PointHistory_GetById", new { MaLSTD = maLSTD });
                if (pointHistory == null)
                    return ApiResponse<PointHistory>.ErrorResponse("Không tìm thấy lịch sử điểm");

                return ApiResponse<PointHistory>.SuccessResponse(pointHistory, "Lấy thông tin lịch sử điểm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<PointHistory>.ErrorResponse($"Lỗi khi lấy thông tin lịch sử điểm: {ex.Message}");
            }
        }

        public async Task<(ApiResponse<IEnumerable<PointHistory>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null)
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_PointHistory_GetAll",
                    new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

                var items = (await reader.ReadAsync<PointHistory>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<PointHistory>>.SuccessResponse(items, "Lấy danh sách lịch sử điểm thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<PointHistory>>.ErrorResponse($"Lỗi khi lấy danh sách lịch sử điểm: {ex.Message}"), 0);
            }
        }
    }
}