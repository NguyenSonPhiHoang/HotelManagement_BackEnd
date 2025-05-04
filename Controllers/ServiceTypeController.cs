using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes("application/json")]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IServiceTypeService _serviceTypeService;

        public ServiceTypeController(IServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ServiceType>>> GetAllServiceTypes()
        {
            var response = _serviceTypeService.GetAllServiceTypes();
            return response.Success ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<ServiceType>> GetServiceTypeById(int id)
        {
            var response = _serviceTypeService.GetServiceTypeById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public ActionResult<ApiResponse<int>> CreateServiceType([FromBody] AddServiceType serviceType)
        {
            var response = _serviceTypeService.CreateServiceType(serviceType);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<int>> UpdateServiceType(int id, [FromBody] UpdateServiceType serviceType)
        {
            var existingServiceType = _serviceTypeService.GetServiceTypeById(id);
            if (!existingServiceType.Success)
                return NotFound(ApiResponse<int>.ErrorResponse("Không tìm thấy loại dịch vụ"));

            var response = _serviceTypeService.UpdateServiceType(id, serviceType);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<int>> DeleteServiceType(int id)
        {
            var existingServiceType = _serviceTypeService.GetServiceTypeById(id);
            if (!existingServiceType.Success)
                return NotFound(ApiResponse<int>.ErrorResponse("Không tìm thấy loại dịch vụ"));

            var response = _serviceTypeService.DeleteServiceType(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}