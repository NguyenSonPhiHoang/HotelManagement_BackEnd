using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement_BackEnd.Application.Interfaces;
using HotelManagement_BackEnd.Model;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingServiceController : ControllerBase
    {
        private readonly IBookingServiceService _bookingServiceService;

        public BookingServiceController(IBookingServiceService bookingServiceService)
        {
            _bookingServiceService = bookingServiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams parameters)
        {
            var result = await _bookingServiceService.GetAllBookingServicesAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bookingService = await _bookingServiceService.GetBookingServiceByIdAsync(id);
            if (bookingService == null)
                return NotFound();

            return Ok(bookingService);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingService model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Tính ThanhTien nếu chưa được tính
                if (model.ThanhTien == 0)
                {
                    model.ThanhTien = model.Gia * model.SoLuong;
                }

                var id = await _bookingServiceService.CreateBookingServiceAsync(model);
                return CreatedAtAction(nameof(GetById), new { id }, model);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingService model)
        {
            if (id != model.MaBSD)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Tính lại ThanhTien
                if (model.SoLuong > 0 && model.Gia > 0)
                {
                    model.ThanhTien = model.Gia * model.SoLuong;
                }

                await _bookingServiceService.UpdateBookingServiceAsync(model);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);
                
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _bookingServiceService.DeleteBookingServiceAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);

                return BadRequest(ex.Message);
            }
        }
    }
}