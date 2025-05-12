using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            try
            {
                var maLSTD = await _repository.CreateAsync(pointHistory);
                return CreatedAtAction(nameof(GetById), new { maLSTD }, new { MaLSTD = maLSTD });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{maLSTD}")]
        public async Task<IActionResult> Update(int maLSTD, [FromBody] PointHistory pointHistory)
        {
            try
            {
                pointHistory.MaLSTD = maLSTD;
                await _repository.UpdateAsync(pointHistory);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maLSTD}")]
        public async Task<IActionResult> Delete(int maLSTD)
        {
            try
            {
                await _repository.DeleteAsync(maLSTD);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{maLSTD}")]
        public async Task<IActionResult> GetById(int maLSTD)
        {
            try
            {
                var pointHistory = await _repository.GetByIdAsync(maLSTD);
                if (pointHistory == null)
                    return NotFound();
                return Ok(pointHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var (items, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, searchTerm);
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