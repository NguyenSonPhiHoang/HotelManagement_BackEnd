namespace HotelManagement_BackEnd.Model
{
    public class RoomType
    {
        public string MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }

    public class AddRoomType
    {
        public string TenLoaiPhong { get; set; }
    }
}