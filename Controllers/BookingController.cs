using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public IActionResult Create([FromBody] Booking booking)
        {
            try
            {
                var maDatPhong = _repository.Create(booking);
                return CreatedAtAction(nameof(GetById), new { maDatPhong }, new { MaDatPhong = maDatPhong });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{maDatPhong}")]
        public IActionResult Update(int maDatPhong, [FromBody] Booking booking)
        {
            try
            {
                booking.MaDatPhong = maDatPhong;
                _repository.Update(booking);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maDatPhong}")]
        public IActionResult Delete(int maDatPhong)
        {
            try
            {
                _repository.Delete(maDatPhong);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{maDatPhong}")]
        public IActionResult GetById(int maDatPhong)
        {
            try
            {
                var booking = _repository.GetById(maDatPhong);
                if (booking == null)
                    return NotFound();
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var (items, totalCount) = _repository.GetAll(pageNumber, pageSize, searchTerm);
                return Ok(new
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}