using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/point-history")]
    public class PointHistoryController : ControllerBase
    {
        private readonly IPointHistoryRepository _repository;

        public PointHistoryController(IPointHistoryRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PointHistory pointHistory)
        {
            var response = await _repository.CreateAsync(pointHistory);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maLSTD}")]
        public async Task<IActionResult> Update(int maLSTD, [FromBody] PointHistory pointHistory)
        {
            if (maLSTD != pointHistory.MaLSTD)
                return StatusCode(400, new { success = false, message = "Mã lịch sử điểm không khớp", data = false });

            var response = await _repository.UpdateAsync(pointHistory);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maLSTD}")]
        public async Task<IActionResult> Delete(int maLSTD)
        {
            var response = await _repository.DeleteAsync(maLSTD);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{maLSTD}")]
        public async Task<IActionResult> GetById(int maLSTD)
        {
            var response = await _repository.GetByIdAsync(maLSTD);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (PointHistory)null });

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
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<PointHistory>)null });

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