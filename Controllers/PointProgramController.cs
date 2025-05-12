using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            try
            {
                var maCT = await _repository.CreateAsync(pointProgram);
                return CreatedAtAction(nameof(GetById), new { maCT }, new { MaCT = maCT });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{maCT}")]
        public async Task<IActionResult> Update(int maCT, [FromBody] PointProgram pointProgram)
        {
            try
            {
                pointProgram.MaCT = maCT;
                await _repository.UpdateAsync(pointProgram);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maCT}")]
        public async Task<IActionResult> Delete(int maCT)
        {
            try
            {
                await _repository.DeleteAsync(maCT);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{maCT}")]
        public async Task<IActionResult> GetById(int maCT)
        {
            try
            {
                var pointProgram = await _repository.GetByIdAsync(maCT);
                if (pointProgram == null)
                    return NotFound();
                return Ok(pointProgram);
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