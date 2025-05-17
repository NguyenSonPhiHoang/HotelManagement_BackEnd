using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [Route("api/payments")]
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
            var response = await _repository.CreateAsync(payment);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maThanhToan}")]
        public async Task<IActionResult> Update(int maThanhToan, [FromBody] Payment payment)
        {
            if (maThanhToan != payment.MaThanhToan)
                return StatusCode(400, new { success = false, message = "Mã thanh toán không khớp", data = false });

            var response = await _repository.UpdateAsync(payment);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maThanhToan}")]
        public async Task<IActionResult> Delete(int maThanhToan)
        {
            var response = await _repository.DeleteAsync(maThanhToan);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{maThanhToan}")]
        public async Task<IActionResult> GetById(int maThanhToan)
        {
            var response = await _repository.GetByIdAsync(maThanhToan);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (Payment)null });

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
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "MaThanhToan",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            if (!result.Success)
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<Payment>)null });

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
    }
}