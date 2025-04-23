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
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams parameters)
        {
            var result = await _serviceService.GetAllServicesAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var service = await _serviceService.GetServiceByIdAsync(id);
                if (service == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Khong tim thay dich vu"));

                 return Ok(ApiResponse<Service>.SuccessResponse(service, "Lay du lieu thanh cong"));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _serviceService.CreateServiceAsync(model);
                return Ok(ApiResponse<Service>.SuccessResponse(result, "Them du lieu thanh cong"));

            }
            catch (ApplicationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceDTO model)
        {


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _serviceService.UpdateServiceAsync(id, model);
                return Ok(ApiResponse<Service>.SuccessResponse(updated, "Cap nhat du lieu thanh cong"));
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ApiResponse<bool>.ErrorResponse(ex.Message));

                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _serviceService.DeleteServiceAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(deleted, "Xoa du lieu thanh cong"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}