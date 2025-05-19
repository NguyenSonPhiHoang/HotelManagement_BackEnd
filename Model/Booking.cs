namespace HotelManagement.Model
{
    public class Booking
    {
        public int MaDatPhong { get; set; }
        public int MaKhachHang { get; set; }
        public int MaPhong { get; set; }
        public DateTime GioCheckIn { get; set; }
        public DateTime GioCheckOut { get; set; }
        public string TrangThai { get; set; } // Pending, Confirmed, Cancelled
        public DateTime NgayDat { get; set; }
    }
    public class BookingCreateRequest
    {
        public int MaKhachHang { get; set; }
        public int MaPhong { get; set; }
        public DateTime GioCheckIn { get; set; }
        public DateTime GioCheckOut { get; set; }
        public DateTime NgayDat { get; set; }
    }
}