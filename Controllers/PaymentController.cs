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
        private readonly ICustomerRepository _customerRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentController(
            IPaymentRepository repository, 
            ICustomerRepository customerRepository, 
            IInvoiceRepository invoiceRepository)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _invoiceRepository = invoiceRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", data = (int?)null });

            // Lấy thông tin hóa đơn
            var invoiceResponse = await _invoiceRepository.GetByIdAsync(request.MaHoaDon);
            if (!invoiceResponse.Success || invoiceResponse.Data == null)
                return BadRequest(new { success = false, message = "Hóa đơn không tồn tại", data = (int?)null });

            string maKhachHang = invoiceResponse.Data.MaKhachHang.ToString();
            decimal tongThanhTien = invoiceResponse.Data.TongThanhTien;

            // Tính SoTienGiam nếu dùng điểm
            decimal soTienGiam = 0;
            if (request.SoDiemSuDung > 0)
            {
                var customerResponse = await _customerRepository.GetByIdAsync(maKhachHang);
                if (!customerResponse.Success || customerResponse.Data == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy khách hàng", data = (int?)null });

                var pointProgramResponse = await _customerRepository.GetPointProgramByIdAsync(customerResponse.Data.MaCT.ToString());
                if (!pointProgramResponse.Success || pointProgramResponse.Data == null)
                    return BadRequest(new { success = false, message = "Không tìm thấy chương trình điểm", data = (int?)null });

                soTienGiam = request.SoDiemSuDung * pointProgramResponse.Data.MucGiamGia;
            }

            // Điều chỉnh ThanhTien
            decimal thanhTien = tongThanhTien - soTienGiam;
            if (thanhTien < 0)
                return BadRequest(new { success = false, message = "Số tiền thanh toán không hợp lệ", data = (int?)null });

            // Cập nhật request
            request.SoTienGiam = soTienGiam;
            request.ThanhTien = thanhTien;

            // Tạo thanh toán
            var response = await _repository.CreateAsync(request);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = (int?)null });

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
                return BadRequest(new { success = false, message = "Mã thanh toán không khớp", data = false });

            var response = await _repository.UpdateAsync(payment);
            if (!response.Success)
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
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
                return BadRequest(new { success = false, message = response.Message, data = false });

            return Ok(new
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
                return NotFound(new { success = false, message = response.Message, data = (Payment)null });

            return Ok(new
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
                return BadRequest(new { success = false, message = result.Message, data = (IEnumerable<Payment>)null });

            return Ok(new
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