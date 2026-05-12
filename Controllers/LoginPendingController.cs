using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;
using System.Runtime.InteropServices;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginPendingController : ControllerBase
    {
        private readonly ILoginPendingService _loginPendingService;

        public LoginPendingController(ILoginPendingService loginPendingService)
        {
            _loginPendingService=loginPendingService;
        }

        [HttpPost("loginpending-list")]
        public async Task <IActionResult> GetLoginPendingList()
        {
            try
            {
                var result = await _loginPendingService.GetLoginPendingList();
                return Ok (result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("loginpending-deatils")]
        public async Task <IActionResult> GetLoginPendingDetails( StringIdRequest request)
        {
            try
            {
                var result = await _loginPendingService.GetLoginPendingDeatils(request);
                return Ok(result);
            }

            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }




    }

}
