using Microsoft.AspNetCore.Mvc;
using HotelManagement.Model;
using HotelManagement.Services;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllRoles()
        {
            var response = _roleService.GetAllRoles();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetById{maVaiTro}")]
        public IActionResult GetRoleById(string maVaiTro)
        {
            var response = _roleService.GetRoleById(maVaiTro);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("AddRole")]
        public IActionResult CreateRole([FromBody] Role role)
        {
            var response = _roleService.CreateRole(role);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("ModifyRole{maVaiTro}")]
        public IActionResult UpdateRole(string maVaiTro, [FromBody] Role role)
        {
            if (maVaiTro != role.MaVaiTro)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Mã vai trò không khớp"));
            }

            var response = _roleService.UpdateRole(role);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("Delete{maVaiTro}")]
        public IActionResult DeleteRole(string maVaiTro)
        {
            var response = _roleService.DeleteRole(maVaiTro);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}