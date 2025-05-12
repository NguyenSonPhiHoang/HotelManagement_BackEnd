namespace HotelManagement.Model
{
    public class Invoice
    {
        public int MaHoaDon { get; set; }
        public int MaDatPhong { get; set; }
        public int MaKhachHang { get; set; }
        public decimal TongTienPhong { get; set; }
        public decimal TongTienDichVu { get; set; }
        public decimal TongThanhTien { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? HoTenKhachHang { get; set; } 
        public string? SoPhong { get; set; }
    }
}