using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankMasterController : ControllerBase
    {
        private readonly IBankMasterService _iBankMasterService;

        public BankMasterController(IBankMasterService bankservice)
        {
            _iBankMasterService = bankservice;
        }

        

        [HttpGet("bankmaster-list")]
        public async Task<IActionResult> GetBankMasterList()
        {
            try
            {
                var result = await _iBankMasterService.GetBankMasterList();
                return Ok(result); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("bankmaster-details")]

        public async Task<IActionResult> GetBankMasterDetails([FromBody] StringIdRequest request )
        {
            try
            {
                var result = await _iBankMasterService.GetBankMasterDetails(request);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [HttpPost("bankmaster-save")]
        public async Task<IActionResult> UpsertBankMasterDeatils([FromBody] Bankmaster request)
        {

            try
            {
                var result = await _iBankMasterService.UpsertBankMasterDeatils(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                {
                    return StatusCode(500, new { Message = ex.Message });
                }
            }

        }


    }
}
