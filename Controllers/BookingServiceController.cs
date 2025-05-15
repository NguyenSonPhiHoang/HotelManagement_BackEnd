using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Create([FromBody] BookingService bookingService)
        {
            var response = await _repository.CreateAsync(bookingService);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maBSD}")]
        public async Task<IActionResult> Update(int maBSD, [FromBody] BookingService bookingService)
        {
            if (maBSD != bookingService.MaBSD)
                return StatusCode(400, new { success = false, message = "Mã dịch vụ đặt phòng không khớp", data = false });

            var response = await _repository.UpdateAsync(bookingService);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maBSD}")]
        public async Task<IActionResult> Delete(int maBSD)
        {
            var response = await _repository.DeleteAsync(maBSD);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{maBSD}")]
        public async Task<IActionResult> GetById(int maBSD)
        {
            var response = await _repository.GetByIdAsync(maBSD);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (BookingService)null });

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
            [FromQuery] string? searchTerm = null)
        {
            var (result, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, searchTerm);
            if (!result.Success)
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<BookingService>)null });

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
    }
}