using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustodianMasterController : ControllerBase
    {

        public readonly ICustodianMasterService _iCustodianMasterService;

        public CustodianMasterController(ICustodianMasterService custodianMasterService)
        {
            _iCustodianMasterService = custodianMasterService;
        }


        [HttpPost("custodianmaster-list")]

        public async Task<IActionResult> GetCustodianMasterList([FromBody] CustodianMasterRequest request)
        {
            try
            {
                var result = await _iCustodianMasterService.GetCustodianMasterList(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500,new  { Message= ex.Message });
            }
        }

        [HttpPost("custodianmaster-details")]

        public async Task<IActionResult> GetCustodianMasterDetails(StringIdRequest request)
        {
            try
            {
                var result = await _iCustodianMasterService.GetCustodianMasterDetails(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }


            
        }

        [HttpPost("custodianmaster-save")]

        public async Task <IActionResult> UpsertCustodianMaster(CustodianMaster request)
        {
            try
            {
                var result = await _iCustodianMasterService.UpsertCustodianmaster(request);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
