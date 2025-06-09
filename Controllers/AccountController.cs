using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "MaTaiKhoan",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _accountRepository.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message, data = (IEnumerable<Account>)null });

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data,
                totalCount,
                pageNumber,
                pageSize
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByUsername([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(new { success = false, message = "Tên tài khoản không được để trống", data = (Account)null });

            var response = await _accountRepository.SearchByUsernameAsync(username);
            if (!response.Success)
                return NotFound(new { success = false, message = response.Message, data = (Account)null });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _accountRepository.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(new { success = false, message = response.Message, data = (Account)null });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var response = await _accountRepository.GetByUsernameAsync(username);
            if (!response.Success)
                return NotFound(new { success = false, message = response.Message, data = (Account)null });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddAccount account)
        {
            var usernameExists = await _accountRepository.IsUsernameExistsAsync(account.TenTaiKhoan);
            if (usernameExists.Success && usernameExists.Data)
                return BadRequest(new { success = false, message = "Tên tài khoản đã tồn tại", data = (int?)null });

            var emailExists = await _accountRepository.IsEmailExistsAsync(account.Email);
            if (emailExists.Success && emailExists.Data)
                return BadRequest(new { success = false, message = "Email đã tồn tại", data = (int?)null });

            var response = await _accountRepository.CreateAsync(account);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = (int?)null });

            return CreatedAtAction(nameof(GetById), new { id = response.Data }, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ModifyAccount modifyAccount)
        {
            if (modifyAccount == null)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", data = false });

            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (!existingAccount.Success)
                return NotFound(new { success = false, message = "Không tìm thấy tài khoản", data = false });

            if (existingAccount.Data.TenTaiKhoan != modifyAccount.TenTaiKhoan)
            {
                var usernameExists = await _accountRepository.IsUsernameExistsAsync(modifyAccount.TenTaiKhoan);
                if (usernameExists.Success && usernameExists.Data)
                    return BadRequest(new { success = false, message = "Tên tài khoản đã tồn tại", data = false });
            }

            if (existingAccount.Data.Email != modifyAccount.Email)
            {
                var emailExists = await _accountRepository.IsEmailExistsAsync(modifyAccount.Email);
                if (emailExists.Success && emailExists.Data)
                    return BadRequest(new { success = false, message = "Email đã tồn tại", data = false });
            }

            // Tạo Account từ ModifyAccount và id
            var account = new Account
            {
                MaTaiKhoan = id,
                TenTaiKhoan = modifyAccount.TenTaiKhoan,
                MatKhau = modifyAccount.MatKhau ?? existingAccount.Data.MatKhau, // Giữ nguyên mật khẩu nếu không được cung cấp
                TenHienThi = modifyAccount.TenHienThi,
                Email = modifyAccount.Email,
                Phone = modifyAccount.Phone,
                MaVaiTro = modifyAccount.MaVaiTro
            };

            var response = await _accountRepository.UpdateAsync(account);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (!existingAccount.Success)
                return NotFound(new { success = false, message = "Không tìm thấy tài khoản", data = false });

            var response = await _accountRepository.DeleteAsync(id);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = await _accountRepository.ChangePasswordAsync(request.MaTaiKhoan, request.CurrentPassword, request.NewPassword);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("validate/username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var response = await _accountRepository.IsUsernameExistsAsync(username);
            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("validate/email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var response = await _accountRepository.IsEmailExistsAsync(email);
            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
    }
}