using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors; 

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
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.TenTaiKhoan) || string.IsNullOrEmpty(request.MatKhau))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tên đăng nhập và mật khẩu không được để trống"));
            }

            var result = _authRepository.Login(request);

            if (result == null || result.MaTaiKhoan <= 0)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng"));
            }

            return Ok(ApiResponse<TokenResponse>.SuccessResponse(result, "Đăng nhập thành công"));
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var result = _authRepository.Register(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}