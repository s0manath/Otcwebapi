using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteMasterController : ControllerBase
    {

        private readonly IRouteMasterService _routeMasterService;

        public RouteMasterController(IRouteMasterService routeMasterService)
        {
            _routeMasterService = routeMasterService;
        }

        [HttpPost("routemaster-list")]

        public async Task<IActionResult> GetRouteMasterList()
        {
            try
            {
                var result = await _routeMasterService.GetRouteMasterList();
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("routemaster-details")]

        public async Task<IActionResult> GetRouteMasterDetails(StringIdRequest request)
        {
            try
            {
                var result = await _routeMasterService.GetRouteMasterDetails(request);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("routemaster-save")]

        public async Task <IActionResult> UpsertRoutemaster(RouteMaster request)
        {
            try
            {
                var result = await _routeMasterService.UpsertRoutemaster(request);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
