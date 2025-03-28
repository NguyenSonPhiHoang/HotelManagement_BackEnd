namespace HotelManagement.Model
{
    public class ChangePasswordRequest
    {
        public int MaTaiKhoan { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}