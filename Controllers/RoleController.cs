using HotelManagement.Model;
using HotelManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelManagement.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleRepository.GetAllAsync();
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (IEnumerable<Role>?)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpGet("{maVaiTro}")]
        public async Task<IActionResult> GetRoleById(string maVaiTro)
        {
            var response = await _roleRepository.GetByIdAsync(maVaiTro);
            if (!response.Success)
                return StatusCode(404, new { success = false, message = response.Message, data = (Role?)null });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] AddRole addRole)
        {
            if (addRole == null || string.IsNullOrWhiteSpace(addRole.TenVaiTro))
                return StatusCode(400, new { success = false, message = "Tên vai trò không được để trống", data = (int?)null });

            var response = await _roleRepository.CreateAsync(addRole);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = (int?)null });

            return StatusCode(201, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }
        [HttpPut("{maVaiTro}")]
        public async Task<IActionResult> UpdateRole(string maVaiTro, [FromBody] Role role)
        {
            if (maVaiTro != role.MaVaiTro)
                return StatusCode(400, new { success = false, message = "Mã vai trò không khớp", data = false });

            var response = await _roleRepository.UpdateAsync(role);
            if (!response.Success)
                return StatusCode(400, new { success = false, message = response.Message, data = false });

            return StatusCode(200, new
            {
                success = response.Success,
                message = response.Message,
                data = response.Data
            });
        }

        [HttpDelete("{maVaiTro}")]
        public async Task<IActionResult> DeleteRole(string maVaiTro)
        {
            var response = await _roleRepository.DeleteAsync(maVaiTro);
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