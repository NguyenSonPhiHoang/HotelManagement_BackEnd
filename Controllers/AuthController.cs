using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [EnableCors("AllowReactApp")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.TenTaiKhoan) || string.IsNullOrEmpty(request.MatKhau))
                return StatusCode(400, new { success = false, message = "Tên đăng nhập và mật khẩu không được để trống", data = (TokenResponse)null });

            var response = await _authRepository.LoginAsync(request);
            if (!response.Success || response.Data == null || response.Data.MaTaiKhoan <= 0)
                return StatusCode(401, new { success = false, message = response.Message ?? "Tên đăng nhập hoặc mật khẩu không đúng", data = (TokenResponse)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authRepository.RegisterAsync(request);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
    }
}