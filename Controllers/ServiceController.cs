using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var response = await _serviceRepository.GetAllAsync();
            if (!response.Success)
                return StatusCode(500, new { success = false, message = response.Message, data = (IEnumerable<Service>)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var response = await _serviceRepository.GetByIdAsync(id);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (Service)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] AddService service)
        {
            var response = await _serviceRepository.CreateAsync(service);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (Service)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateService service)
        {
            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (!existingService.Success)
                return StatusCode(404, new { success = false, message = "Không tìm thấy dịch vụ", data = (Service)null });

            var response = await _serviceRepository.UpdateAsync(id, service);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (Service)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (!existingService.Success)
                return StatusCode(404, new { success = false, message = "Không tìm thấy dịch vụ", data = false });

            var response = await _serviceRepository.DeleteAsync(id);
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