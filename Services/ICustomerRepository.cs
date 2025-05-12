using HotelManagement.DataReader;
using HotelManagement.Model;
using HotelManagement.Utilities;
using System;
using System.Collections.Generic;

namespace HotelManagement.Services
{
    public interface ICustomerRepository
    {
        ApiResponse<IEnumerable<Customer>> GetAllCustomers();
        ApiResponse<Customer> GetCustomerById(string id);
        ApiResponse<string> CreateCustomer(AddCustomer customer);
        ApiResponse<bool> UpdateCustomer(Customer customer);
        ApiResponse<bool> DeleteCustomer(string id);
        ApiResponse<bool> IsEmailExists(string email);
        ApiResponse<bool> IsPhoneExists(string phone);
        ApiResponse<bool> AddPoints(string customerId, int points);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseDapper _db;

        public CustomerRepository(DatabaseDapper db)
        {
            _db = db ;
        }

        public ApiResponse<IEnumerable<Customer>> GetAllCustomers()
        {
            try
            {
                var customers = _db.QueryStoredProcedure<Customer>("sp_Customer_GetAll");
                return ApiResponse<IEnumerable<Customer>>.SuccessResponse(customers, "Lấy danh sách khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Customer>>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<Customer> GetCustomerById(string id)
        {
            try
            {
                var customer = _db.QueryFirstOrDefaultStoredProcedure<Customer>("sp_Customer_GetById",
                    new { MaKhachHang = id });

                if (customer == null)
                    return ApiResponse<Customer>.ErrorResponse("Không tìm thấy khách hàng");

                return ApiResponse<Customer>.SuccessResponse(customer, "Lấy thông tin khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<Customer>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<string> CreateCustomer(AddCustomer addCustomer)
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

                var newId = _db.QueryFirstOrDefaultStoredProcedure<string>("sp_Customer_Insert", parameters);
                return ApiResponse<string>.SuccessResponse(newId, "Tạo khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> UpdateCustomer(Customer customer)
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

                int rowsAffected = _db.ExecuteStoredProcedure("sp_Customer_Update", parameters);

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Cập nhật khách hàng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Cập nhật khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> DeleteCustomer(string id)
        {
            try
            {
                int rowsAffected = _db.ExecuteStoredProcedure("sp_Customer_Delete",
                    new { MaKhachHang = id });

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Xóa khách hàng thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Xóa khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> IsEmailExists(string email)
        {
            try
            {
                var customer = _db.QueryFirstOrDefault<Customer>(
                    "SELECT TOP 1 MaKhachHang FROM Customer WHERE Email = @Email",
                    new { Email = email });

                return ApiResponse<bool>.SuccessResponse(customer != null);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> IsPhoneExists(string phone)
        {
            try
            {
                var customer = _db.QueryFirstOrDefault<Customer>(
                    "SELECT TOP 1 MaKhachHang FROM Customer WHERE DienThoai = @DienThoai",
                    new { DienThoai = phone });

                return ApiResponse<bool>.SuccessResponse(customer != null);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }

        public ApiResponse<bool> AddPoints(string customerId, int points)
        {
            try
            {
                int rowsAffected = _db.ExecuteStoredProcedure("sp_Customer_AddPoints",
                    new { MaKhachHang = customerId, SoDiemThem = points });

                if (rowsAffected <= 0)
                    return ApiResponse<bool>.ErrorResponse("Thêm điểm thất bại");

                return ApiResponse<bool>.SuccessResponse(true, "Thêm điểm thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
            }
        }
    }
}