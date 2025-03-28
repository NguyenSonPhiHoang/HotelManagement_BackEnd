namespace HotelManagement.Model
{
    public class UserToken
    {
        public int MaToken { get; set; }
        public int MaTaiKhoan { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime NgayHetHan { get; set; }
    }
}