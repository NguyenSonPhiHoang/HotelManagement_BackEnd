using Microsoft.AspNetCore.Mvc;

using HotelManagement.Model;
using HotelManagement.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<Account>>> GetAllAccounts()
        {
            var response = _accountService.GetAllAccounts();
            return response.Success ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<Account>> GetAccountById(int id)
        {
            var response = _accountService.GetAccountById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("username/{username}")]
        public ActionResult<ApiResponse<Account>> GetAccountByUsername(string username)
        {
            var response = _accountService.GetAccountByUsername(username);
            return response.Success ? Ok(response) : NotFound(response);
        }
        [HttpPost]
        public ActionResult<ApiResponse<int>> CreateAccount([FromBody] AddAccount account)
        {
            var usernameExists = _accountService.IsUsernameExists(account.TenTaiKhoan);
            if (usernameExists.Success && usernameExists.Data)
                return BadRequest(ApiResponse<int>.ErrorResponse("Tên tài khoản đã tồn tại"));

            var emailExists = _accountService.IsEmailExists(account.Email);
            if (emailExists.Success && emailExists.Data)
                return BadRequest(ApiResponse<int>.ErrorResponse("Email đã tồn tại"));

            var response = _accountService.CreateAccount(account);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<bool>> UpdateAccount(int id, [FromBody] Account account)
        {
            if (id != account.MaTaiKhoan)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Mã tài khoản không khớp"));
            }

            // Kiểm tra tài khoản có tồn tại không
            var existingAccount = _accountService.GetAccountById(id);
            if (!existingAccount.Success)
                return NotFound(ApiResponse<bool>.ErrorResponse("Không tìm thấy tài khoản"));

            // Kiểm tra username và email đã tồn tại chưa (nếu có thay đổi)
            if (existingAccount.Data.TenTaiKhoan != account.TenTaiKhoan)
            {
                var usernameExists = _accountService.IsUsernameExists(account.TenTaiKhoan);
                if (usernameExists.Success && usernameExists.Data)
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Tên tài khoản đã tồn tại"));
            }

            if (existingAccount.Data.Email != account.Email)
            {
                var emailExists = _accountService.IsEmailExists(account.Email);
                if (emailExists.Success && emailExists.Data)
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Email đã tồn tại"));
            }

            var response = _accountService.UpdateAccount(account);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<bool>> DeleteAccount(int id)
        {
            // Kiểm tra tài khoản có tồn tại không
            var existingAccount = _accountService.GetAccountById(id);
            if (!existingAccount.Success)
                return NotFound(ApiResponse<bool>.ErrorResponse("Không tìm thấy tài khoản"));

            var response = _accountService.DeleteAccount(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("change-password")]
        public ActionResult<ApiResponse<bool>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = _accountService.ChangePassword(
                request.MaTaiKhoan,
                request.CurrentPassword,
                request.NewPassword
            );

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("validate/username/{username}")]
        public ActionResult<ApiResponse<bool>> CheckUsername(string username)
        {
            var response = _accountService.IsUsernameExists(username);
            return Ok(response);
        }

        [HttpGet("validate/email/{email}")]
        public ActionResult<ApiResponse<bool>> CheckEmail(string email)
        {
            var response = _accountService.IsEmailExists(email);
            return Ok(response);
        }
    }
}