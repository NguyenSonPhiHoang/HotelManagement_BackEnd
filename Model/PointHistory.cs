namespace HotelManagement.Model
{
    public class PointHistory
    {
        public int MaLSTD { get; set; }
        public int MaKhachHang { get; set; }
        public int SoDiem { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public string LoaiGiaoDich { get; set; } = null!; 
        public string? HoTenKhachHang { get; set; } 
    }
}