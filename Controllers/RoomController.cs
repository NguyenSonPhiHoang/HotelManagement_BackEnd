using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
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
            var response = await _roomRepository.GetRoomsByFilterAsync(trangThai, loaiPhong, tinhTrang);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (IEnumerable<Room>)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRoomBySoPhong([FromQuery] string soPhong)
        {
            if (string.IsNullOrEmpty(soPhong))
                return StatusCode(400, new { success = false, message = "Số phòng không được để trống", data = (Room)null });

            var response = await _roomRepository.GetRoomBySoPhongAsync(soPhong);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (Room)null });

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
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? trangThai = null,
            [FromQuery] int? loaiPhong = null,
            [FromQuery] string? tinhTrang = null,
            [FromQuery] string? sortBy = "MaPhong",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _roomRepository.GetAllAsync(pageNumber, pageSize, searchTerm, trangThai, loaiPhong, tinhTrang, sortBy, sortOrder);
            if (!result.Success)
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<Room>)null });

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

        [HttpGet("{maPhong}")]
        public async Task<IActionResult> GetById(int maPhong)
        {
            var response = await _roomRepository.GetByIdAsync(maPhong);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (Room)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequestRoom room)
        {
            var response = await _roomRepository.CreateAsync(room);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maPhong}")]
        public async Task<IActionResult> Update(int maPhong, [FromBody] Room room)
        {
            if (maPhong != room.MaPhong)
                return StatusCode(400, new { success = false, message = "Mã phòng không khớp", data = false });

            var response = await _roomRepository.UpdateAsync(room);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maPhong}")]
        public async Task<IActionResult> Delete(int maPhong)
        {
            var response = await _roomRepository.DeleteAsync(maPhong);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
        [HttpGet("availability/{maPhong}")]
        public async Task<IActionResult> CheckRoomAvailability(int maPhong)
        {
            var response = await _roomRepository.CheckRoomAvailabilityAsync(maPhong);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (object?)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maPhong}/status")]
        public async Task<IActionResult> UpdateRoomStatus(int maPhong, [FromBody] UpdateRoomStatusRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.TrangThai))
                return StatusCode(400, new { success = false, message = "Dữ liệu không hợp lệ", data = false });

            var response = await _roomRepository.UpdateRoomStatusAsync(maPhong, request.TrangThai.Trim(), request.TinhTrang?.Trim());
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms()
        {
            var response = await _roomRepository.GetRoomsByFilterAsync("Trống", "Tất cả loại phòng", "Đã dọn dẹp");
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (IEnumerable<Room>)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
    }
}