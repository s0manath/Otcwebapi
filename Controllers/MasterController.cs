using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _masterService;

        public MasterController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        #region Custodian Master

        [HttpGet("custodians")]
        public async Task<IActionResult> GetCustodians([FromQuery] string field = null, [FromQuery] string startsWith = null, [FromQuery] string chklocked = null)
        {
            var userName = User.Identity?.Name ?? "Admin";
            var result = await _masterService.GetCustodiansAsync(field, startsWith, chklocked, userName);
            return Ok(result);
        }

        [HttpGet("custodians/{id}")]
        public async Task<IActionResult> GetCustodian(int id)
        {
            var result = await _masterService.GetCustodianByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("custodians")]
        public async Task<IActionResult> SaveCustodian([FromBody] CustodianMaster custodian)
        {
            var userName = User.Identity?.Name ?? "Admin";
            var error = await _masterService.SaveCustodianAsync(custodian, userName);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Message = error });
            return Ok(new { Message = "Saved successfully" });
        }

        #endregion

        #region Franchise Master

        [HttpGet("franchises")]
        public async Task<IActionResult> GetFranchises([FromQuery] string filterField = null, [FromQuery] string filterValue = null)
        {
            var result = await _masterService.GetFranchisesAsync(filterField, filterValue);
            return Ok(result);
        }

        [HttpGet("franchises/{id}")]
        public async Task<IActionResult> GetFranchise(int id)
        {
            var result = await _masterService.GetFranchiseByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("franchises")]
        public async Task<IActionResult> SaveFranchise([FromBody] FranchiseMaster franchise)
        {
            var userName = User.Identity?.Name ?? "Admin";
            var error = await _masterService.SaveFranchiseAsync(franchise, userName);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Message = error });
            return Ok(new { Message = "Saved successfully" });
        }

        #endregion

        #region ATM Master

        [HttpGet("atms")]
        public async Task<IActionResult> GetAtms()
        {
            var result = await _masterService.GetAtmsAsync();
            return Ok(result);
        }

        [HttpGet("atms/{id}")]
        public async Task<IActionResult> GetAtm(string id)
        {
            var result = await _masterService.GetAtmByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("atms")]
        public async Task<IActionResult> SaveAtm([FromBody] AtmMaster atm)
        {
            var userName = User.Identity?.Name ?? "Admin";
            var error = await _masterService.SaveAtmAsync(atm, userName);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Message = error });
            return Ok(new { Message = "Saved successfully" });
        }

        #endregion

        #region Dropdowns

        [HttpGet("dropdowns/locations")]
        public async Task<IActionResult> GetLocations() => Ok(await _masterService.GetLocationsAsync());

        [HttpGet("dropdowns/zoms")]
        public async Task<IActionResult> GetZoms() => Ok(await _masterService.GetZomsAsync());

        [HttpGet("dropdowns/franchises")]
        public async Task<IActionResult> GetFranchiseDropdown() => Ok(await _masterService.GetFranchiseDropdownAsync());

        [HttpGet("dropdowns/routekeys")]
        public async Task<IActionResult> GetRouteKeys() => Ok(await _masterService.GetRouteKeysAsync());

        [HttpGet("dropdowns/states")]
        public async Task<IActionResult> GetStates() => Ok(await _masterService.GetStatesAsync());

        [HttpGet("dropdowns/districts/{stateId}")]
        public async Task<IActionResult> GetDistricts(int stateId) => Ok(await _masterService.GetDistrictsByStateAsync(stateId));

        #endregion
    }
}
