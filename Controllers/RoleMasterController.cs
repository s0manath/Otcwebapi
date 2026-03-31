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

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] RoleSearchRequest request)
        {
            var result = await _roleService.GetRolesAsync(request);
            return Ok(result);
        }

        [HttpPost("detail")]
        public async Task<IActionResult> Get([FromBody] LongIdRequest request)
        {
            var result = await _roleService.GetRoleByIdAsync(request.Id);
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
