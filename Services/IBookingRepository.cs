using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IBookingRepository
    {
        int Create(Booking booking);
        void Update(Booking booking);
        void Delete(int maDatPhong);
        Booking GetById(int maDatPhong);
        (IEnumerable<Booking> Items, int TotalCount) GetAll(int pageNumber, int pageSize, string searchTerm = null);
    }
    public class BookingRepository : IBookingRepository
    {
        private readonly DatabaseDapper _db;

        public BookingRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public int Create(Booking booking)
        {
            var parameters = new
            {
                booking.MaKhachHang,
                booking.MaPhong,
                booking.GioCheckIn,
                booking.GioCheckOut,
                booking.TrangThai,
                booking.NgayDat
            };

            return _db.QueryFirstOrDefaultStoredProcedure<int>("sp_Booking_Create", parameters);
        }

        public void Update(Booking booking)
        {
            var parameters = new
            {
                booking.MaDatPhong,
                booking.MaKhachHang,
                booking.MaPhong,
                booking.GioCheckIn,
                booking.GioCheckOut,
                booking.TrangThai,
                booking.NgayDat
            };

            _db.ExecuteStoredProcedure("sp_Booking_Update", parameters);
        }

        public void Delete(int maDatPhong)
        {
            _db.ExecuteStoredProcedure("sp_Booking_Delete", new { MaDatPhong = maDatPhong });
        }

        public Booking GetById(int maDatPhong)
        {
            return _db.QueryFirstOrDefaultStoredProcedure<Booking>("sp_Booking_GetById", new { MaDatPhong = maDatPhong });
        }

        public (IEnumerable<Booking> Items, int TotalCount) GetAll(int pageNumber, int pageSize, string searchTerm = null)
        {
            using var reader = _db.QueryMultiple("sp_Booking_GetAll",
                new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

            var items = reader.Read<Booking>().ToList();
            var totalCount = reader.ReadSingle<int>();

            return (items, totalCount);
        }
    }
}