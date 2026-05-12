using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZomMasterController : ControllerBase
    {
        private readonly IZomMasterService _zomMasterService;

        public ZomMasterController(IZomMasterService zomMasterService)
        {
            _zomMasterService = zomMasterService;            
        }

        [HttpPost("zommaster-list")]

        public async Task<IActionResult> GetZomMasterList()
        {
            try
            {
                var result = await _zomMasterService.GetZomMasterList();

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("zommaster-details")]

        public async Task<IActionResult> GetZomMasterDetails(StringIdRequest request)
        {
            try
            {
                var result = await _zomMasterService.GetZomMasterDetails(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpPost("zommaster-save")]
        public async Task<IActionResult> UpsertZomMaster([FromBody] ZomMaster request)
        {
            try
            {
                var result = await _zomMasterService.UpsertZomMaster(request);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

    }
}
