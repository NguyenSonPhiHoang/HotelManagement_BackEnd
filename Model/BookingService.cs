namespace HotelManagement.Model
{
    public class BookingService
    {
        public int MaBSD { get; set; }
        public int MaDatPhong { get; set; }
        public int MaDichVu { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien { get; set; }
        public DateTime NgaySuDung { get; set; }
        public string? TenDichVu { get; set; } 
    }
}