using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegionalMasterController : ControllerBase
    {
        private readonly IRegionalMasterService _regionalService;

        public RegionalMasterController(IRegionalMasterService regionalService)
        {
            _regionalService = regionalService;
        }

        // States
        [HttpPost("states/search")]
        public async Task<IActionResult> SearchStates([FromBody] RegionalSearchRequest request) => Ok(await _regionalService.GetStatesAsync(request));

        [HttpPost("states/detail")]
        public async Task<IActionResult> GetState([FromBody] IdRequest request) => Ok(await _regionalService.GetStateByIdAsync(request.Id));

        [HttpPost("states/save")]
        public async Task<IActionResult> SaveState([FromBody] StateMaster state) => Ok(await _regionalService.SaveStateAsync(state));

        // Districts
        [HttpPost("districts/search")]
        public async Task<IActionResult> SearchDistricts([FromBody] RegionalSearchRequest request) => Ok(await _regionalService.GetDistrictsAsync(request));

        [HttpPost("districts/detail")]
        public async Task<IActionResult> GetDistrict([FromBody] IdRequest request) => Ok(await _regionalService.GetDistrictByIdAsync(request.Id));

        [HttpPost("districts/save")]
        public async Task<IActionResult> SaveDistrict([FromBody] DistrictMaster district) => Ok(await _regionalService.SaveDistrictAsync(district));

        // ZOMs
        [HttpPost("zoms/search")]
        public async Task<IActionResult> SearchZoms([FromBody] RegionalSearchRequest request) => Ok(await _regionalService.GetZomsAsync(request));

        [HttpPost("zoms/detail")]
        public async Task<IActionResult> GetZom([FromBody] IdRequest request) => Ok(await _regionalService.GetZomByIdAsync(request.Id));

        [HttpPost("zoms/save")]
        public async Task<IActionResult> SaveZom([FromBody] ZomMaster zom) => Ok(await _regionalService.SaveZomAsync(zom));
    }
}
