using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;

namespace HotelManagement.Controllers
{
    [Route("api/invoices")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _repository;

        public InvoiceController(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceCreateRequest request)
        {
            var response = await _repository.CreateAsync(request);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost("services")]
        public async Task<IActionResult> AddService([FromBody] InvoiceServiceCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", data = false });

            var response = await _repository.AddServiceAsync(request);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{maHoaDon}")]
        public async Task<IActionResult> Update(int maHoaDon, [FromBody] Invoice invoice)
        {
            if (maHoaDon != invoice.MaHoaDon)
                return StatusCode(400, new { success = false, message = "Mã hóa đơn không khớp", data = false });

            var response = await _repository.UpdateAsync(invoice);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maHoaDon}")]
        public async Task<IActionResult> Delete(int maHoaDon)
        {
            var response = await _repository.DeleteAsync(maHoaDon);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{maHoaDon}")]
        public async Task<IActionResult> GetById(int maHoaDon)
        {
            var response = await _repository.GetByIdAsync(maHoaDon);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (Invoice)null });

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
            [FromQuery] string? sortBy = "MaHoaDon",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            if (!result.Success)
                return StatusCode(400, new { success = false, message = result.Message, data = (IEnumerable<Invoice>)null });

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
        [HttpGet("revenue-statistics")]
        public async Task<IActionResult> GetRevenueStatistics(
            [FromQuery] DateTime? ngayBatDau,
            [FromQuery] DateTime? ngayKetThuc)
        {
            try
            {
                var response = await _repository.GetRevenueStatisticsAsync(ngayBatDau, ngayKetThuc);
                if (!response.Success)
                    return BadRequest(new { success = false, message = response.Message });

                return Ok(new
                {
                    success = response.Success,
                    message = response.Message,
                    data = response.Data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }
    }
}