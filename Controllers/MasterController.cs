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

        [HttpPost("custodians-list")]
        public async Task<IActionResult> GetCustodians([FromBody] CustodianSearchRequest request)
        {
            var userName = User.Identity?.Name ?? "Admin";
            var result = await _masterService.GetCustodiansAsync(request.Field, request.StartsWith, request.ChkLocked, userName);
            return Ok(result);
        }

        public class CustodianSearchRequest
        {
            public string? Field { get; set; }
            public string? StartsWith { get; set; }
            public string? ChkLocked { get; set; }
        }

        [HttpPost("custodians-detail")]
        public async Task<IActionResult> GetCustodian([FromBody] IdRequest request)
        {
            var result = await _masterService.GetCustodianByIdAsync(request.Id);
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

        [HttpPost("franchises-list")]
        public async Task<IActionResult> GetFranchises([FromBody] FranchiseSearchRequest request)
        {
            var result = await _masterService.GetFranchisesAsync(request.FilterField, request.FilterValue);
            return Ok(result);
        }

        public class FranchiseSearchRequest
        {
            public string? FilterField { get; set; }
            public string? FilterValue { get; set; }
        }

        [HttpPost("franchises-detail")]
        public async Task<IActionResult> GetFranchise([FromBody] IdRequest request)
        {
            var result = await _masterService.GetFranchiseByIdAsync(request.Id);
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

        [HttpPost("atms-list")]
        public async Task<IActionResult> GetAtms()
        {
            var result = await _masterService.GetAtmsAsync();
            return Ok(result);
        }

        [HttpPost("atms-detail")]
        public async Task<IActionResult> GetAtm([FromBody] StringIdRequest request)
        {
            var result = await _masterService.GetAtmByIdAsync(request.Id);
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

        [HttpPost("dropdowns/locations")]
        public async Task<IActionResult> GetLocations() => Ok(await _masterService.GetLocationsAsync());

        [HttpPost("dropdowns/zoms")]
        public async Task<IActionResult> GetZoms() => Ok(await _masterService.GetZomsAsync());

        [HttpPost("dropdowns/franchises")]
        public async Task<IActionResult> GetFranchiseDropdown() => Ok(await _masterService.GetFranchiseDropdownAsync());

        [HttpPost("dropdowns/routekeys")]
        public async Task<IActionResult> GetRouteKeys() => Ok(await _masterService.GetRouteKeysAsync());

        [HttpPost("dropdowns/states")]
        public async Task<IActionResult> GetStates() => Ok(await _masterService.GetStatesAsync());

        [HttpPost("dropdowns/districts")]
        public async Task<IActionResult> GetDistricts([FromBody] IdRequest request) => Ok(await _masterService.GetDistrictsByStateAsync(request.Id));

        [HttpPost("dropdowns/custodians")]
        public async Task<IActionResult> GetCustodiansDropdown() => Ok(await _masterService.GetCustodiansDropdownAsync());

        #endregion

        #region State & District Management

        [HttpGet("states/all")]
        public async Task<ActionResult<IEnumerable<State>>> GetStatesSearch([FromQuery] string name = null)
        {
            var result = await _masterService.GetStatesAllAsync(name);
            return Ok(result);
        }

        [HttpGet("states/{id}")]
        public async Task<ActionResult<State>> GetState(int id)
        {
            var result = await _masterService.GetStateByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("states")]
        public async Task<ActionResult<string>> SaveState([FromBody] StateMaster state)
        {
            var result = await _masterService.SaveStateAsync(state, "admin");
            if (string.IsNullOrEmpty(result)) return Ok(new { message = "State saved successfully" });
            return BadRequest(new { error = result });
        }

        [HttpGet("districts/all")]
        public async Task<ActionResult<IEnumerable<DistrictMaster>>> GetDistrictsSearch([FromQuery] string name = null, [FromQuery] int? stateId = null)
        {
            var result = await _masterService.GetDistrictsAllAsync(name, stateId);
            return Ok(result);
        }

        [HttpGet("districts/{id}")]
        public async Task<ActionResult<DistrictMaster>> GetDistrict(int id)
        {
            var result = await _masterService.GetDistrictByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("districts")]
        public async Task<ActionResult<string>> SaveDistrict([FromBody] DistrictMaster district)
        {
            var result = await _masterService.SaveDistrictAsync(district, "admin");
            if (string.IsNullOrEmpty(result)) return Ok(new { message = "District saved successfully" });
            return BadRequest(new { error = result });
        }

        [HttpGet("dropdowns/regions")]
        public async Task<ActionResult<IEnumerable<MasterDropdownItem>>> GetRegions()
        {
            var result = await _masterService.GetRegionsAsync();
            return Ok(result);
        }

        #endregion
    }
}
