// Model/RegisterRequest.cs
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Model
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [MinLength(5, ErrorMessage = "Tên tài khoản phải có ít nhất 5 ký tự")]
        public string TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Tên hiển thị không được để trống")]
        public string TenHienThi { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",
            ErrorMessage = "Email phải có định dạng @gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        public string Phone { get; set; }

        public int MaVaiTro { get; set; } = 4; // Mặc định là Customer

    }
      public class OtpVerificationRequest
        {
            public int MaTaiKhoan { get; set; }
            public string OtpCode { get; set; } = null!;
        }

        public class OtpResendRequest
        {
            public int MaTaiKhoan { get; set; }
            public string Email { get; set; } = null!;
        }

        public class OtpVerification
        {
            public int MaTaiKhoan { get; set; }
            public string OtpCode { get; set; } = null!;
            public string Email { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
}