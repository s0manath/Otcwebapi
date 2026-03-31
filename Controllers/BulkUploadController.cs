using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BulkUploadController : ControllerBase
    {
        private readonly IAtmBulkUploadService _bulkService;

        public BulkUploadController(IAtmBulkUploadService bulkService)
        {
            _bulkService = bulkService;
        }

        [HttpPost("atm")]
        public async Task<IActionResult> UploadAtm([FromBody] BulkUploadRequest request)
        {
            var result = await _bulkService.ProcessAtmBulkUploadAsync(request);
            return Ok(result);
        }

        [HttpPost("route-key")]
        public async Task<IActionResult> UpdateRouteKey([FromBody] BulkUploadRequest request)
        {
            var result = await _bulkService.ProcessRouteKeyUpdateAsync(request);
            return Ok(result);
        }
    }
}
