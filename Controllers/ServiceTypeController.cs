using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.Model;
using HotelManagement_BackEnd.Application.DTO;
using HotelManagement_BackEnd.Application.Interfaces;
using HotelManagement_BackEnd.Model;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IServiceTypeService _serviceTypeService;

        public ServiceTypeController(IServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams parameters)
        {
            var result = await _serviceTypeService.GetAllServiceTypesAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var serviceType = await _serviceTypeService.GetServiceTypeByIdAsync(id);
            if (serviceType == null)
                return NotFound();

            return Ok(serviceType);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceTypeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _serviceTypeService.CreateServiceTypeAsync(model);
                return Ok(ApiResponse<ServiceType>.SuccessResponse(result, "Tao du lieu thanh cong"));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceTypeDTO model)
        {


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _serviceTypeService.UpdateServiceTypeAsync(id, model);
                return Ok(ApiResponse<ServiceType>.SuccessResponse(updated, "Cap nhat du lieu thanh cong"));

            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _serviceTypeService.DeleteServiceTypeAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(deleted, "Xoa du lieu thanh cong"));
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);

                return BadRequest(ex.Message);
            }
        }
    }
}