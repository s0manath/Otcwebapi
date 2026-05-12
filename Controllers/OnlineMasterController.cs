using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineMasterController : ControllerBase
    {

        private readonly IOnlineMasterService _onlineMasterService;

        public OnlineMasterController(IOnlineMasterService onlineMasterService)
        {
            _onlineMasterService = onlineMasterService;
        }

        [HttpPost("onlinemaster-list")]

        public async Task<IActionResult> GetOnlineMasterList()
        {
            try
            {
                var result = await _onlineMasterService.GetZomMasterList();

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("onlinemaster-details")]

        public async Task<IActionResult> GetOnlineMasterDetails(StringIdRequest request)
        {
            try
            {
                var result = await _onlineMasterService.GetOnlineMasterDetails(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpPost("onlinemaster-save")]
        public async Task<IActionResult> UpsertOnlineMaster([FromBody]OnlineMaster request)
        {
            try
            {
                var result = await _onlineMasterService.UpsertonlineMaster(request);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

    }
}
