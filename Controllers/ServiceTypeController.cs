using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
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
        public async Task<IActionResult> GetAllServiceTypes()
        {
            var response = await _serviceTypeRepository.GetAllAsync();
            if (!response.Success)
                return StatusCode(500, new { success = false, message = response.Message, data = (IEnumerable<ServiceType>)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
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