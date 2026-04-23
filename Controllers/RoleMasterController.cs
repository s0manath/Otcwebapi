using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleMasterController : ControllerBase
    {
        private readonly IRoleMasterService _roleService;

        public RoleMasterController(IRoleMasterService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _roleService.GetRolesAsync(null);
            return Ok(result);
        }

        [HttpPost("detail")]
        public async Task<IActionResult> Get([FromBody] IdRequest request)
        {
            var result = await _roleService.GetRoleBySlNoAsync(request.Id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] RoleMaster role)
        {
            var result = await _roleService.SaveRoleAsync(role);
            return Ok(new { message = result });
        }

        [HttpPost("modules")]
        public async Task<IActionResult> GetModules()
        {
            var result = await _roleService.GetModuleListAsync();
            return Ok(result);
        }
    }
}
