using Microsoft.AspNetCore.Mvc;
using HotelManagement.Model;
using HotelManagement.Services;
using System.Collections.Generic;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<Customer>>> GetAllCustomers()
        {
            var response = _customerService.GetAllCustomers();
            return response.Success ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<Customer>> GetCustomerById(string id)
        {
            var response = _customerService.GetCustomerById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public ActionResult<ApiResponse<string>> CreateCustomer([FromBody] AddCustomer customer)
        {
            // Kiểm tra email và điện thoại đã tồn tại chưa
            var emailExists = _customerService.IsEmailExists(customer.Email);
            if (emailExists.Success && emailExists.Data)
                return BadRequest(ApiResponse<string>.ErrorResponse("Email đã tồn tại"));

            var phoneExists = _customerService.IsPhoneExists(customer.DienThoai);
            if (phoneExists.Success && phoneExists.Data)
                return BadRequest(ApiResponse<string>.ErrorResponse("Số điện thoại đã tồn tại"));

            var response = _customerService.CreateCustomer(customer);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<bool>> UpdateCustomer(string id, [FromBody] Customer customer)
        {
            if (id != customer.MaKhachHang)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Mã khách hàng không khớp"));
            }
            var existingCustomer = _customerService.GetCustomerById(id);
            if (!existingCustomer.Success)
                return NotFound(ApiResponse<bool>.ErrorResponse("Không tìm thấy khách hàng"));

            if (existingCustomer.Data.Email != customer.Email)
            {
                var emailExists = _customerService.IsEmailExists(customer.Email);
                if (emailExists.Success && emailExists.Data)
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Email đã tồn tại"));
            }

            if (existingCustomer.Data.DienThoai != customer.DienThoai)
            {
                var phoneExists = _customerService.IsPhoneExists(customer.DienThoai);
                if (phoneExists.Success && phoneExists.Data)
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Số điện thoại đã tồn tại"));
            }

            var response = _customerService.UpdateCustomer(customer);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<bool>> DeleteCustomer(string id)
        {
            var existingCustomer = _customerService.GetCustomerById(id);
            if (!existingCustomer.Success)
                return NotFound(ApiResponse<bool>.ErrorResponse("Không tìm thấy khách hàng"));

            var response = _customerService.DeleteCustomer(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{id}/point")]
        public ActionResult<ApiResponse<bool>> AddPoints([FromBody] AddPointsRequest request)
        {
            // Kiểm tra khách hàng có tồn tại không
            var existingCustomer = _customerService.GetCustomerById(request.MaKhachHang);
            if (!existingCustomer.Success)
                return NotFound(ApiResponse<bool>.ErrorResponse("Không tìm thấy khách hàng"));

            var response = _customerService.AddPoints(request.MaKhachHang, request.SoDiem);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("validate/email/{email}")]
        public ActionResult<ApiResponse<bool>> CheckEmail(string email)
        {
            var response = _customerService.IsEmailExists(email);
            return Ok(response);
        }

        [HttpGet("validate/phone/{phone}")]
        public ActionResult<ApiResponse<bool>> CheckPhone(string phone)
        {
            var response = _customerService.IsPhoneExists(phone);
            return Ok(response);
        }
    }
}   