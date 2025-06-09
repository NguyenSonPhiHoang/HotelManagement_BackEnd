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
        private readonly IRoomRepository _roomRepository;

        public BookingController(IBookingRepository repository, IRoomRepository roomRepository)
        {
            _repository = repository;
            _roomRepository = roomRepository;
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
                LoaiTinhTien = request.LoaiTinhTien?.Trim(),
                TrangThai = "Pending"
            };

            Console.WriteLine($"Đang đặt phòng: MaPhong = {booking.MaPhong}");
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

        // Endpoint mới: Xác nhận đặt phòng (Pending -> Confirmed)
        [HttpPut("{maDatPhong}/confirm")]
        public async Task<IActionResult> ConfirmBooking(int maDatPhong)
        {
            var response = await _repository.ConfirmBookingAsync(maDatPhong);

            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        // Endpoint: Hoàn thành đặt phòng (Confirmed -> Completed)
        [HttpPut("{maDatPhong}/complete")]
        public async Task<IActionResult> CompleteBooking(int maDatPhong)
        {
            var response = await _repository.CompleteOrCancelBookingAsync(maDatPhong, "complete");

            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        // Endpoint: Hủy đặt phòng (Pending/Confirmed -> Cancelled)
        [HttpPut("{maDatPhong}/cancel")]
        public async Task<IActionResult> CancelBooking(int maDatPhong)
        {
            var response = await _repository.CompleteOrCancelBookingAsync(maDatPhong, "cancel");

            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maDatPhong}")]
        public async Task<IActionResult> Update(int maDatPhong, [FromBody] BookingUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu cập nhật không hợp lệ", data = false });
            }

            var response = await _repository.UpdateAsync(maDatPhong, request);

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
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 10,
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

        // Endpoint cập nhật trạng thái tổng quát (sử dụng logic flow validation trong repository)
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