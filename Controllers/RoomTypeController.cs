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
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypeController(IRoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        [HttpGet]
        public IActionResult GetAllRoomTypes()
        {
            var result = _roomTypeService.GetAllRoomTypes();
            return Ok(result);
        }
    }

}