using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetRoomsByFilter(
            [FromQuery] string trangThai = "Tất cả phòng",
            [FromQuery] string loaiPhong = "Tất cả loại phòng",
            [FromQuery] string tinhTrang = "Tất cả")
        {
            var result = await _roomRepository.GetRoomsByFilter(trangThai, loaiPhong, tinhTrang);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRoomBySoPhong([FromQuery] string soPhong)
        {
            if (string.IsNullOrEmpty(soPhong))
            {
                return BadRequest(ApiResponse<Room>.ErrorResponse("Số phòng không được để trống"));
            }

            var response = await _roomRepository.GetRoomBySoPhong(soPhong);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? trangThai = null,
            [FromQuery] int? loaiPhong = null,
            [FromQuery] string? tinhTrang = null,
            [FromQuery] string? sortBy = "MaPhong",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _roomRepository.GetAllAsync(pageNumber, pageSize, searchTerm, trangThai, loaiPhong, tinhTrang, sortBy, sortOrder);
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

        [HttpGet("{maPhong}")]
        public async Task<IActionResult> GetById(int maPhong)
        {
            var response = await _roomRepository.GetByIdAsync(maPhong);
            if (!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Room room)
        {
            var response = await _roomRepository.CreateAsync(room);
            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetById), new { maPhong = response.Data }, response);
        }

        [HttpPut("{maPhong}")]
        public async Task<IActionResult> Update(int maPhong, [FromBody] Room room)
        {
            room.MaPhong = maPhong;
            var response = await _roomRepository.UpdateAsync(room);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpDelete("{maPhong}")]
        public async Task<IActionResult> Delete(int maPhong)
        {
            var response = await _roomRepository.DeleteAsync(maPhong);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }
    }
}