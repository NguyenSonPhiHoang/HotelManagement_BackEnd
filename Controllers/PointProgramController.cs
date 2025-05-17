using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/point-programs")]
    public class PointProgramController : ControllerBase
    {
        private readonly IPointProgramRepository _repository;

        public PointProgramController(IPointProgramRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PointProgram pointProgram)
        {
            var response = await _repository.CreateAsync(pointProgram);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maCT}")]
        public async Task<IActionResult> Update(int maCT, [FromBody] PointProgram pointProgram)
        {
            if (maCT != pointProgram.MaCT)
                return StatusCode(400, new { success = false, message = "Mã chương trình điểm không khớp", data = false });

            var response = await _repository.UpdateAsync(pointProgram);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maCT}")]
        public async Task<IActionResult> Delete(int maCT)
        {
            var response = await _repository.DeleteAsync(maCT);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{maCT}")]
        public async Task<IActionResult> GetById(int maCT)
        {
            var response = await _repository.GetByIdAsync(maCT);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (PointProgram)null });

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
            [FromQuery] string? sortBy = "MaCT",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            if (!result.Success)
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<PointProgram>)null });

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