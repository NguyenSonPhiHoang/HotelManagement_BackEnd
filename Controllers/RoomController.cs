using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement_BackEnd.Model;
using HotelManagement_BackEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement_BackEnd.Controllers
{
  [ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("filter")]
    public IActionResult GetRoomsByFilter(
        [FromQuery] string trangThai = "Tất cả phòng",
        [FromQuery] string loaiPhong = "Tất cả loại phòng",
        [FromQuery] string tinhTrang = "Tất cả")
    {
        var result = _roomService.GetRoomsByFilter(trangThai, loaiPhong, tinhTrang);
        return Ok(result);
    }
}

}