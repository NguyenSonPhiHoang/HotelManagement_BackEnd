using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _repository;

        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateRequest request)
        {
            Console.WriteLine($"Request LoaiTinhTien: {request.LoaiTinhTien}");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState không hợp lệ");
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", data = (int?)null });
            }

            var booking = new Booking
            {
                MaKhachHang = request.MaKhachHang,
                MaPhong = request.MaPhong,
                GioCheckIn = request.GioCheckIn,
                GioCheckOut = request.GioCheckOut,
                NgayDat = request.NgayDat,
                LoaiTinhTien = request.LoaiTinhTien?.Trim(), // Gán và loại bỏ khoảng trắng
                TrangThai = "Pending" // Mặc định
            };

            Console.WriteLine($"Booking LoaiTinhTien sau ánh xạ: {booking.LoaiTinhTien}");
            var response = await _repository.CreateAsync(booking);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maDatPhong}")]
        public async Task<IActionResult> Update(int maDatPhong, [FromBody] Booking booking)
        {
            if (maDatPhong != booking.MaDatPhong)
                return BadRequest(new { success = false, message = "Mã đặt phòng không khớp", data = false });

            var response = await _repository.UpdateAsync(booking);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maDatPhong}")]
        public async Task<IActionResult> Delete(int maDatPhong)
        {
            var response = await _repository.DeleteAsync(maDatPhong);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{maDatPhong}")]
        public async Task<IActionResult> GetById(int maDatPhong)
        {
            var response = await _repository.GetByIdAsync(maDatPhong);
            if (!response.Success)
                return NotFound(new { success = false, message = response.Message, data = (Booking)null });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "MaDatPhong",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message, data = (IEnumerable<Booking>)null });

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data,
                totalCount,
                pageNumber,
                pageSize
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByMaDatPhong([FromQuery] int maDatPhong)
        {
            if (maDatPhong <= 0)
                return BadRequest(new { success = false, message = "Mã đặt phòng phải lớn hơn 0", data = (Booking)null });

            var response = await _repository.SearchByMaDatPhongAsync(maDatPhong);
            if (!response.Success)
                return NotFound(new { success = false, message = response.Message, data = (Booking)null });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maDatPhong}/status")]
        public async Task<IActionResult> UpdateStatus(int maDatPhong, [FromBody] UpdateStatusRequest request)
        {
            if (string.IsNullOrEmpty(request.TrangThai))
                return BadRequest(new { success = false, message = "Trạng thái không hợp lệ", data = false });

            var response = await _repository.UpdateStatusAsync(maDatPhong, request.TrangThai);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
    }
}