using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _repository;

        public InvoiceController(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Invoice invoice)
        {
            try
            {
                var maHoaDon = await _repository.CreateAsync(invoice);
                return CreatedAtAction(nameof(GetById), new { maHoaDon }, new { MaHoaDon = maHoaDon });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{maHoaDon}")]
        public async Task<IActionResult> Update(int maHoaDon, [FromBody] Invoice invoice)
        {
            try
            {
                invoice.MaHoaDon = maHoaDon;
                await _repository.UpdateAsync(invoice);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maHoaDon}")]
        public async Task<IActionResult> Delete(int maHoaDon)
        {
            try
            {
                await _repository.DeleteAsync(maHoaDon);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{maHoaDon}")]
        public async Task<IActionResult> GetById(int maHoaDon)
        {
            try
            {
                var invoice = await _repository.GetByIdAsync(maHoaDon);
                if (invoice == null)
                    return NotFound();
                return Ok(invoice);
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