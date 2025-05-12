using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var response = _roleRepository.GetAllRoles();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{maVaiTro}")]
        public IActionResult GetRoleById(string maVaiTro)
        {
            var response = _roleRepository.GetRoleById(maVaiTro);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public IActionResult CreateRole([FromBody] Role role)
        {
            var response = _roleRepository.CreateRole(role);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{maVaiTro}")]
        public IActionResult UpdateRole(string maVaiTro, [FromBody] Role role)
        {
            if (maVaiTro != role.MaVaiTro)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Mã vai trò không khớp"));
            }

            var response = _roleRepository.UpdateRole(role);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{maVaiTro}")]
        public IActionResult DeleteRole(string maVaiTro)
        {
            var response = _roleRepository.DeleteRole(maVaiTro);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}