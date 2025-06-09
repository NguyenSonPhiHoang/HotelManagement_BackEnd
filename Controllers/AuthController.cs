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
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await _authRepository.RefreshTokenAsync(request);
            if (!response.Success)
                return BadRequest(new { response.Success, response.Message, response.Data });

            return Ok(new { response.Success, response.Message, response.Data });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", data = (int?)null });

            var response = await _authRepository.RegisterAsync(request);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            // Kiểm tra request null trước
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu request không được để trống",
                    data = false
                });
            }

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                    });

                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ",
                    errors = errors,
                    data = false
                });
            }

            try
            {
                var response = await _authRepository.VerifyOtpAsync(request);

                if (!response.Success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = response.Message,
                        data = false
                    });
                }

                return Ok(new
                {
                    success = response.Success,
                    message = response.Message,
                    data = response.Data
                });
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi xử lý yêu cầu",
                    data = false
                });
            }
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] OtpResendRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", data = false });

            var response = await _authRepository.ResendOtpAsync(request);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
    }
}