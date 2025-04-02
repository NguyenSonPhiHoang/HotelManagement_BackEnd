using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowReactApp")] 

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.TenTaiKhoan) || string.IsNullOrEmpty(request.MatKhau))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tên đăng nhập và mật khẩu không được để trống"));
            }

            var result = _authService.Login(request);

            if (result == null || result.MaTaiKhoan <= 0)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng"));
            }

            return Ok(ApiResponse<TokenResponse>.SuccessResponse(result, "Đăng nhập thành công"));
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var result = _authService.Register(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}