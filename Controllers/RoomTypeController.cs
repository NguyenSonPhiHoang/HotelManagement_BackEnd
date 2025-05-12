using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/room-types")]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeController(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            var (result, totalCount) = await _roomTypeRepository.GetAllAsync(pageNumber, pageSize, searchTerm);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                Items = result.Data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{maLoaiPhong}")]
        public async Task<IActionResult> GetById(int maLoaiPhong)
        {
            var response = await _roomTypeRepository.GetByIdAsync(maLoaiPhong);
            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomType roomType)
        {
            var response = await _roomTypeRepository.CreateAsync(roomType);
            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetById), new { maLoaiPhong = response.Data }, response);
        }

        [HttpPut("{maLoaiPhong}")]
        public async Task<IActionResult> Update(int maLoaiPhong, [FromBody] RoomType roomType)
        {
            roomType.MaLoaiPhong = maLoaiPhong;
            var response = await _roomTypeRepository.UpdateAsync(roomType);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpDelete("{maLoaiPhong}")]
        public async Task<IActionResult> Delete(int maLoaiPhong)
        {
            var response = await _roomTypeRepository.DeleteAsync(maLoaiPhong);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }
    }
}