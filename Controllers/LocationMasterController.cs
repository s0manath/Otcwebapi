using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationMasterController : ControllerBase
    {

        private readonly ILocationMasterService _locationMasterService;
        public LocationMasterController(ILocationMasterService locationMaster) {
            _locationMasterService = locationMaster;
        }

        [HttpPost("locationmaster-list")]
        public async Task<IActionResult> GetLocationMasterList()
        {
            try
            {
                var result = await _locationMasterService.GetLocationMasterList( );
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500,new {ex.Message});
            }
        }
        [HttpPost("locationmaster-details")]
        public async Task<IActionResult> GetLocationMasterDetails([FromBody] StringIdRequest request)
        {
            try
            {
                var result = await _locationMasterService.GetLocationMasterDetails(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
        //[HttpPost("locationmaster-save")]
        //public async Task<IActionResult> GetLocationMasterUpsert()
        //{
        //    try
        //    {
        //        var result = await _locationMasterService.GetLocationMasterUpsert();
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { ex.Message });
        //    }
        //}
    }
}
