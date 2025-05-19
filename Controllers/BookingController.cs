using Microsoft.AspNetCore.Mvc;
using HotelManagement.Model;
using HotelManagement.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HotelManagement.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (request == null || request.MaKhachHang <= 0 || request.MaPhong <= 0)
                return BadRequest(new { success = false, message = "Dữ liệu đặt phòng không hợp lệ", data = (int?)null });

            var booking = new Booking
            {
                MaKhachHang = request.MaKhachHang,
                MaPhong = request.MaPhong,
                GioCheckIn = request.GioCheckIn,
                GioCheckOut = request.GioCheckOut,
                NgayDat = request.NgayDat,
                TrangThai = "Pending" // Mặc định
            };

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
                return StatusCode(400, new { success = false, message = "Mã đặt phòng không khớp", data = false });

            var response = await _repository.UpdateAsync(booking);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
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
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
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
                return StatusCode(404, new { success = false, message = response.Message, data = (Booking)null });

            return StatusCode(200, new
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
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<Booking>)null });

            return StatusCode(200, new
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
                return StatusCode(400, new { success = false, message = "Mã đặt phòng phải lớn hơn 0", data = (Booking)null });

            var response = await _repository.SearchByMaDatPhongAsync(maDatPhong);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (Booking)null });

            return StatusCode(200, new
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