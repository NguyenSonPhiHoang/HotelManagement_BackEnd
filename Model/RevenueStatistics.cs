namespace HotelManagement.Model
{
    public class RevenueStatistics
    {
        public decimal TongDoanhThuPhong { get; set; }
        public decimal TongDoanhThuDichVu { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoLuongHoaDon { get; set; }
        public List<DailyRevenue> DanhSachDoanhThuNgay { get; set; } = new List<DailyRevenue>();
    }

    public class DailyRevenue
    {
        public DateTime NgayDoanhThu { get; set; }
        public decimal DoanhThuPhong { get; set; }
        public decimal DoanhThuDichVu { get; set; }
        public decimal DoanhThuNgay { get; set; }
        public int SoHoaDon { get; set; }
    }
}