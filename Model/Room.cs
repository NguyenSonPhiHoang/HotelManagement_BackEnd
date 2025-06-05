namespace HotelManagement.Model
{
    public class Room
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public string TrangThai { get; set; }
        public string LoaiPhong { get; set; }
        public string TinhTrang { get; set; }
    }
    public class CreateRequestRoom
    {
        public string SoPhong { get; set; }
        public string TrangThai { get; set; }
        public int LoaiPhong { get; set; }
        public string TinhTrang { get; set; }
    }
    public class RoomAvailabilityResult
    {
        public bool IsAvailable { get; set; }
        public string Message { get; set; }
        public Room RoomInfo { get; set; }
    }
}