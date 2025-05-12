using HotelManagement.DataReader;
using HotelManagement.Model;

namespace HotelManagement.Services
{
    public interface IBookingServiceRepository
    {
        int Create(BookingService bookingService);
        void Update(BookingService bookingService);
        void Delete(int maBSD);
        BookingService GetById(int maBSD);
        (IEnumerable<BookingService> Items, int TotalCount) GetAll(int pageNumber, int pageSize, string searchTerm = null);
    }
    public class BookingServiceRepository : IBookingServiceRepository
    {
        private readonly DatabaseDapper _db;

        public BookingServiceRepository(DatabaseDapper db)
        {
            _db = db;
        }

        public int Create(BookingService bookingService)
        {
            var parameters = new
            {
                bookingService.MaDatPhong,
                bookingService.MaDichVu,
                bookingService.SoLuong,
                bookingService.Gia,
                bookingService.ThanhTien,
                bookingService.NgaySuDung
            };

            return _db.QueryFirstOrDefaultStoredProcedure<int>("sp_BookingService_Create", parameters);
        }

        public void Update(BookingService bookingService)
        {
            var parameters = new
            {
                bookingService.MaBSD,
                bookingService.MaDatPhong,
                bookingService.MaDichVu,
                bookingService.SoLuong,
                bookingService.Gia,
                bookingService.ThanhTien,
                bookingService.NgaySuDung
            };

            _db.ExecuteStoredProcedure("sp_BookingService_Update", parameters);
        }

        public void Delete(int maBSD)
        {
            _db.ExecuteStoredProcedure("sp_BookingService_Delete", new { MaBSD = maBSD });
        }

        public BookingService GetById(int maBSD)
        {
            return _db.QueryFirstOrDefaultStoredProcedure<BookingService>("sp_BookingService_GetById", new { MaBSD = maBSD });
        }

        public (IEnumerable<BookingService> Items, int TotalCount) GetAll(int pageNumber, int pageSize, string searchTerm = null)
        {
            using var reader = _db.QueryMultiple("sp_BookingService_GetAll",
                new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });

            var items = reader.Read<BookingService>().ToList();
            var totalCount = reader.ReadSingle<int>();

            return (items, totalCount);
        }
    }
}