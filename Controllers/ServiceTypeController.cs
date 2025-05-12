using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement_BackEnd.Controllers
{
    [ApiController]
    [Route("api/service-types")]
    [Consumes("application/json")]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IServiceTypeRepository _serviceTypeRepository;

        public ServiceTypeController(IServiceTypeRepository serviceTypeRepository)
        {
            _serviceTypeRepository = serviceTypeRepository;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ServiceType>>> GetAllServiceTypes()
        {
            var response = _serviceTypeRepository.GetAllServiceTypes();
            return response.Success ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<ServiceType>> GetServiceTypeById(int id)
        {
            var response = _serviceTypeRepository.GetServiceTypeById(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public ActionResult<ApiResponse<int>> CreateServiceType([FromBody] AddServiceType serviceType)
        {
            var response = _serviceTypeRepository.CreateServiceType(serviceType);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<int>> UpdateServiceType(int id, [FromBody] UpdateServiceType serviceType)
        {
            var existingServiceType = _serviceTypeRepository.GetServiceTypeById(id);
            if (!existingServiceType.Success)
                return NotFound(ApiResponse<int>.ErrorResponse("Không tìm thấy loại dịch vụ"));

            var response = _serviceTypeRepository.UpdateServiceType(id, serviceType);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<int>> DeleteServiceType(int id)
        {
            var existingServiceType = _serviceTypeRepository.GetServiceTypeById(id);
            if (!existingServiceType.Success)
                return NotFound(ApiResponse<int>.ErrorResponse("Không tìm thấy loại dịch vụ"));

            var response = _serviceTypeRepository.DeleteServiceType(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}