namespace HotelManagement.Model
{
    public class Account
    {
        public int MaTaiKhoan { get; set; }
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string TenHienThi { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }
    }

    public class AddAccount
    {
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string TenHienThi { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int MaVaiTro { get; set; }
    }
    public class ModifyAccount
    {
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string TenHienThi { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int MaVaiTro { get; set; }
    }
    public class AccountModel
    {
        public int MaTaiKhoan { get; set; }
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string TenHienThi { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int MaVaiTro { get; set; }
        public bool IsActivated { get; set; }
        public int MaKhachHang { get; set; }
        public string HoTenKhachHang { get; set; }
    }


}