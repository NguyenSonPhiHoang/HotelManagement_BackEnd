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
            this._roomTypeService = roomTypeService;
        }

           [HttpGet]
        public IActionResult GetAll()
        {
            var response = _roomTypeService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var response = _roomTypeService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddRoomType roomType)
        {
            var response = _roomTypeService.CreateAsync(roomType);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AddRoomType roomType)
        {
       
            var response = _roomTypeService.UpdateAsync(id,roomType);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _roomTypeService.DeleteAsync(id);
            return Ok(response);
        }
    }
}