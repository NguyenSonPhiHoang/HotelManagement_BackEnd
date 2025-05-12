using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _repository;

        public PaymentController(IPaymentRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Payment payment)
        {
            try
            {
                var maThanhToan = await _repository.CreateAsync(payment);
                return CreatedAtAction(nameof(GetById), new { maThanhToan }, new { MaThanhToan = maThanhToan });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{maThanhToan}")]
        public async Task<IActionResult> Update(int maThanhToan, [FromBody] Payment payment)
        {
            try
            {
                payment.MaThanhToan = maThanhToan;
                await _repository.UpdateAsync(payment);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{maThanhToan}")]
        public async Task<IActionResult> Delete(int maThanhToan)
        {
            try
            {
                await _repository.DeleteAsync(maThanhToan);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{maThanhToan}")]
        public async Task<IActionResult> GetById(int maThanhToan)
        {
            try
            {
                var payment = await _repository.GetByIdAsync(maThanhToan);
                if (payment == null)
                    return NotFound();
                return Ok(payment);
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