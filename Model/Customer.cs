namespace HotelManagement.Model
{
    public class Customer
    {
        public string MaKhachHang { get; set; }
        public string HoTenKhachHang { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public int TongDiem { get; set; }
        public string MaCT { get; set; } // Mã Chương Trình (Point Program)
        public string TenCT { get; set; } // Tên Chương Trình
    }

    public class AddCustomer
    {
        public string HoTenKhachHang { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string MaCT { get; set; } // Mã Chương Trình (Point Program)
    }

    public class UpdateCustomerNameUnifiedRequest
    {
        public int? MaTaiKhoan { get; set; }      // Cho trường hợp có tài khoản
        public string? MaKhachHang { get; set; }   // Cho trường hợp không có tài khoản
        public string HoTenKhachHang { get; set; }
    }
        public class CustomerPointsInfo
    {
        public string MaKhachHang { get; set; }
        public string HoTenKhachHang { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public int TongDiem { get; set; }
        public int DiemDaSuDung { get; set; }
        public int DiemCoTheSuDung { get; set; }
        public string HangThanhVien { get; set; }
        public decimal MucGiamGia { get; set; }
        public decimal TyLeTichDiem { get; set; }
    }
}