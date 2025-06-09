using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "MaLoaiPhong",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _roomTypeRepository.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            if (!result.Success)
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<RoomType>)null });

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

        [HttpGet("{maLoaiPhong}")]
        public async Task<IActionResult> GetById(int maLoaiPhong)
        {
            var response = await _roomTypeRepository.GetByIdAsync(maLoaiPhong);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (RoomType)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomType roomType)
        {
            var response = await _roomTypeRepository.CreateAsync(roomType);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maLoaiPhong}")]
        public async Task<IActionResult> Update(int maLoaiPhong, [FromBody] RoomType roomType)
        {
            if (maLoaiPhong != roomType.MaLoaiPhong)
                return StatusCode(400, new { success = false, message = "Mã loại phòng không khớp", data = false });

            var response = await _roomTypeRepository.UpdateAsync(roomType);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maLoaiPhong}")]
        public async Task<IActionResult> Delete(int maLoaiPhong)
        {
            var response = await _roomTypeRepository.DeleteAsync(maLoaiPhong);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
    }
}