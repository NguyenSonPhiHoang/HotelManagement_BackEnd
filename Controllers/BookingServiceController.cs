using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Controllers
{
    [Route("api/bookingservice")]
    [ApiController]
    public class BookingServiceController : ControllerBase
    {
        private readonly IBookingServiceRepository _repository;

        public BookingServiceController(IBookingServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Create([FromBody] BookingService bookingService)
        {
            try
            {
                var maBSD = _repository.Create(bookingService);
                return CreatedAtAction(nameof(GetById), new { maBSD }, new { MaBSD = maBSD });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{maBSD}")]
        public IActionResult Update(int maBSD, [FromBody] BookingService bookingService)
        {
            try
            {
                bookingService.MaBSD = maBSD;
                _repository.Update(bookingService);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maBSD}")]
        public IActionResult Delete(int maBSD)
        {
            try
            {
                _repository.Delete(maBSD);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{maBSD}")]
        public IActionResult GetById(int maBSD)
        {
            try
            {
                var bookingService = _repository.GetById(maBSD);
                if (bookingService == null)
                    return NotFound();
                return Ok(bookingService);
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