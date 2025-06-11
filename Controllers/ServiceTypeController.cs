using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [ApiController]
    [Route("api/service-types")]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IServiceTypeRepository _serviceTypeRepository;

        public ServiceTypeController(IServiceTypeRepository serviceTypeRepository)
        {
            _serviceTypeRepository = serviceTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServiceTypes(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "MaLoaiDV",
            [FromQuery] string? sortOrder = "ASC")
        {
            var (result, totalCount) = await _serviceTypeRepository.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
            if (!result.Success)
                return StatusCode(500, new { success = false, message = result.Message, data = (IEnumerable<ServiceType>)null });

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceTypeById(int id)
        {
            var response = await _serviceTypeRepository.GetByIdAsync(id);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (ServiceType)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceType([FromBody] AddServiceType serviceType)
        {
            var response = await _serviceTypeRepository.CreateAsync(serviceType);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (ServiceType)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceType(int id, [FromBody] UpdateServiceType serviceType)
        {
            var existingServiceType = await _serviceTypeRepository.GetByIdAsync(id);
            if (!existingServiceType.Success)
                return StatusCode(404, new { success = false, message = "Không tìm thấy loại dịch vụ", data = (ServiceType)null });

            var response = await _serviceTypeRepository.UpdateAsync(id, serviceType);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (ServiceType)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceType(int id)
        {
            var existingServiceType = await _serviceTypeRepository.GetByIdAsync(id);
            if (!existingServiceType.Success)
                return StatusCode(404, new { success = false, message = "Không tìm thấy loại dịch vụ", data = false });

            var response = await _serviceTypeRepository.DeleteAsync(id);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
    }
}