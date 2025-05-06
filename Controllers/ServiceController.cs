using HotelManagement.Model;
using HotelManagement.Services;
using HotelManagement.Model;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<Service>>> GetAllServices()
        {
            var response = _serviceService.GetAllServices();
            return response.Success ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<Service>> GetServiceById(int id)
        {
            var response = _serviceService.GetServiceById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public ActionResult<ApiResponse<int>> CreateService([FromBody] AddService service)
        {
            var response = _serviceService.CreateService(service);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<int>> UpdateService(int id, [FromBody] UpdateService service)
        {
            var existingService = _serviceService.GetServiceById(id);
            if (!existingService.Success)
                return NotFound(ApiResponse<int>.ErrorResponse("Không tìm thấy dịch vụ"));

            var response = _serviceService.UpdateService(id, service);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<int>> DeleteService(int id)
        {
            var existingService = _serviceService.GetServiceById(id);
            if (!existingService.Success)
                return NotFound(ApiResponse<int>.ErrorResponse("Không tìm thấy dịch vụ"));

            var response = _serviceService.DeleteService(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}