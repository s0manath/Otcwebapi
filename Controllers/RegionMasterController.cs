using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionMasterController : ControllerBase
    {
        private  readonly IRegionMasterService _regionMasterService;

        public RegionMasterController(IRegionMasterService regionMaster)
        {
            _regionMasterService= regionMaster;
        }


        [HttpPost("regionmaster-list")]

        public async Task<IActionResult> GetRegionMasterList()
        {
            try
            {
                var result = await _regionMasterService.GetRegionMasterList();

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("regionmaster-details")] 

        public async Task<IActionResult> GetRegionMasterDetails(StringIdRequest request)
        {
            try
            {
                var result = await _regionMasterService.GetRegionMasterDetails(request);
                return Ok (result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
                
            }
        }

        [HttpPost("regionmaster-save")]
        public async  Task<IActionResult> UpsertRegionMaster([FromBody] RegionMaster request)
        {
            try
            {
                var result = await _regionMasterService.UpsertRegionMaster(request);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

    }
}
