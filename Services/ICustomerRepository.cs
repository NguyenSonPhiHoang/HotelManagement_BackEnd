using HotelManagement.DataReader;
using HotelManagement.Model;
using HotelManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Services
{
    public interface ICustomerRepository
    {
        Task<(ApiResponse<IEnumerable<Customer>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaKhachHang", string? sortOrder = "ASC");
        Task<ApiResponse<Customer>> GetByIdAsync(string id);
        Task<ApiResponse<string>> CreateAsync(AddCustomer customer);
        Task<ApiResponse<bool>> UpdateAsync(Customer customer);
        Task<ApiResponse<bool>> DeleteAsync(string id);
        Task<ApiResponse<bool>> IsEmailExistsAsync(string email);
        Task<ApiResponse<bool>> IsPhoneExistsAsync(string phone);
        Task<ApiResponse<bool>> AddPointsAsync(string customerId, int points);
        Task<ApiResponse<bool>> AccumulatePointsAsync(string maKhachHang, decimal thanhTien);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseDapper _db;

        public CustomerRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public async Task<(ApiResponse<IEnumerable<Customer>> Items, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm = null, string? sortBy = "MaKhachHang", string? sortOrder = "ASC")
        {
            try
            {
                using var reader = await _db.QueryMultipleAsync("sp_Customer_GetAll",
                    new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        SortBy = sortBy,
                        SortOrder = sortOrder
                    });

                var items = (await reader.ReadAsync<Customer>()).ToList();
                var totalCount = await reader.ReadSingleAsync<int>();

                return (ApiResponse<IEnumerable<Customer>>.SuccessResponse(items, "Lấy danh sách khách hàng thành công"), totalCount);
            }
            catch (Exception ex)
            {
                return (ApiResponse<IEnumerable<Customer>>.ErrorResponse($"Lỗi khi lấy danh sách khách hàng: {ex.Message}"), 0);
            }
        }

        public async Task<ApiResponse<Customer>> GetByIdAsync(string id)
        {
            try
            {
                var customer = await _db.QueryFirstOrDefaultStoredProcedureAsync<Customer>("sp_Customer_GetById", new { MaKhachHang = id });
                if (customer == null)
                    return ApiResponse<Customer>.ErrorResponse("Không tìm thấy khách hàng");

                return ApiResponse<Customer>.SuccessResponse(customer, "Lấy thông tin khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Customer>.ErrorResponse($"Lỗi khi lấy thông tin khách hàng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> CreateAsync(AddCustomer addCustomer)
        {
            try
            {
                var parameters = new
                {
                    addCustomer.HoTenKhachHang,
                    addCustomer.Email,
                    addCustomer.DienThoai,
                    addCustomer.MaCT
                };

                var newId = await _db.QueryFirstOrDefaultStoredProcedureAsync<string>("sp_Customer_Insert", parameters);
                return ApiResponse<string>.SuccessResponse(newId, "Tạo khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Lỗi khi tạo khách hàng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Customer customer)
        {
            try
            {
                var parameters = new
                {
                    customer.MaKhachHang,
                    customer.HoTenKhachHang,
                    customer.Email,
                    customer.DienThoai,
                    customer.MaCT
                };

                int rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Customer_Update", parameters);
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật khách hàng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi cập nhật khách hàng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            try
            {
                int rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Customer_Delete", new { MaKhachHang = id });
                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa khách hàng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi xóa khách hàng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> IsEmailExistsAsync(string email)
        {
            try
            {
                var customer = await _db.QueryFirstOrDefaultAsync<Customer>(
                    "SELECT TOP 1 MaKhachHang FROM Customer WHERE Email = @Email",
                    new { Email = email });

                return ApiResponse<bool>.SuccessResponse(customer != null, customer != null ? "Email đã tồn tại" : "Email hợp lệ");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi kiểm tra email: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> IsPhoneExistsAsync(string phone)
        {
            try
            {
                var customer = await _db.QueryFirstOrDefaultAsync<Customer>(
                    "SELECT TOP 1 MaKhachHang FROM Customer WHERE DienThoai = @DienThoai",
                    new { DienThoai = phone });

                return ApiResponse<bool>.SuccessResponse(customer != null, customer != null ? "Số điện thoại đã tồn tại" : "Số điện thoại hợp lệ");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi kiểm tra số điện thoại: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AddPointsAsync(string customerId, int points)
        {
            try
            {
                int rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Customer_AddPoints",
                    new { MaKhachHang = customerId, SoDiemThem = points });

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Thêm điểm thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Thêm điểm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi thêm điểm: {ex.Message}");
            }
        }
        public async Task<ApiResponse<bool>> AccumulatePointsAsync(string maKhachHang, decimal thanhTien)
        {
            try
            {
                // Chuyển đổi maKhachHang sang int
                if (!int.TryParse(maKhachHang, out int maKhachHangInt))
                {
                    Console.WriteLine($"MaKhachHang không hợp lệ: {maKhachHang}");
                    return ApiResponse<bool>.ErrorResponse("Mã khách hàng không hợp lệ");
                }

                // Lấy thông tin khách hàng
                var customer = await _db.QueryFirstOrDefaultStoredProcedureAsync<Customer>("sp_Customer_GetById", new { MaKhachHang = maKhachHangInt });
                if (customer == null)
                {
                    Console.WriteLine($"Không tìm thấy khách hàng với MaKhachHang: {maKhachHangInt}");
                    return ApiResponse<bool>.ErrorResponse("Không tìm thấy khách hàng");
                }
                Console.WriteLine($"Tìm thấy khách hàng: MaKhachHang = {maKhachHangInt}, MaCT = {customer.MaCT}");

                // Lấy chương trình điểm
                var pointProgram = await _db.QueryFirstOrDefaultStoredProcedureAsync<PointProgram>("sp_PointProgram_GetById", new { MaCT = customer.MaCT });
                if (pointProgram == null)
                {
                    Console.WriteLine($"Không tìm thấy chương trình điểm với MaCT: {customer.MaCT}");
                    return ApiResponse<bool>.ErrorResponse("Không tìm thấy chương trình điểm");
                }
                Console.WriteLine($"Tìm thấy chương trình điểm: MaCT = {customer.MaCT}, TyLeTichDiem = {pointProgram.TyLeTichDiem}");

                // Kiểm tra TyLeTichDiem
                decimal tyLeTichDiem = pointProgram.TyLeTichDiem;
                if (tyLeTichDiem <= 0)
                {
                    Console.WriteLine($"TyLeTichDiem không hợp lệ: {tyLeTichDiem}");
                    return ApiResponse<bool>.ErrorResponse("Tỷ lệ tích điểm không hợp lệ");
                }

                // Tính điểm
                int soDiem = (int)(thanhTien * tyLeTichDiem);
                Console.WriteLine($"Tính điểm: ThanhTien = {thanhTien}, TyLeTichDiem = {tyLeTichDiem}, SoDiem = {soDiem}");

                // Kiểm tra soDiem
                if (soDiem <= 0)
                {
                    Console.WriteLine($"SoDiem không hợp lệ: {soDiem}");
                    return ApiResponse<bool>.ErrorResponse("Số điểm tính được không hợp lệ");
                }

                // Cập nhật tổng điểm
                int rowsAffected = await _db.ExecuteStoredProcedureAsync("sp_Customer_AddPoints",
                    new { MaKhachHang = maKhachHangInt, SoDiemThem = soDiem });
                if (rowsAffected <= 0)
                {
                    Console.WriteLine($"Cập nhật TongDiem thất bại cho MaKhachHang: {maKhachHangInt}, RowsAffected = {rowsAffected}");
                    return ApiResponse<bool>.ErrorResponse("Tích điểm thất bại");
                }
                Console.WriteLine($"Cập nhật TongDiem thành công: MaKhachHang = {maKhachHangInt}, RowsAffected = {rowsAffected}");

                // Ghi lịch sử tích điểm
                await _db.ExecuteStoredProcedureAsync("sp_PointHistory_Create",
                    new
                    {
                        MaKhachHang = maKhachHangInt,
                        SoDiem = soDiem,
                        NgayGiaoDich = DateTime.Now,
                        LoaiGiaoDich = "Earn"
                    });
                Console.WriteLine($"Ghi PointHistory thành công: MaKhachHang = {maKhachHangInt}, SoDiem = {soDiem}");

                return ApiResponse<bool>.SuccessResponse(true, "Tích điểm thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tích điểm cho MaKhachHang {maKhachHang}: {ex.Message}");
                return ApiResponse<bool>.ErrorResponse($"Lỗi khi tích điểm: {ex.Message}");
            }
        }
    }
}