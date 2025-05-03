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

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = _roomService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var response = _roomService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddRoom room)
        {
            var response = _roomService.CreateAsync(room);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AddRoom room)
        {
            var response = _roomService.UpdateAsync(id, room);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _roomService.DeleteAsync(id);
            return Ok(response);
        }


    }
}