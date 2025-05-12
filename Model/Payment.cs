namespace HotelManagement.Model
{
    public class Payment
    {
        public int MaThanhToan { get; set; }
        public int MaHoaDon { get; set; }
        public string PhuongThucThanhToan { get; set; } = null!; 
        public int SoDiemSuDung { get; set; }
        public decimal SoTienGiam { get; set; }
        public decimal ThanhTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string? HoTenKhachHang { get; set; } 
    }
}