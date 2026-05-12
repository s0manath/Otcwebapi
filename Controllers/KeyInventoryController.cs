using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyInventoryController : ControllerBase
    {
        private readonly IKeyInventoryService _keyInventoryService;

        public KeyInventoryController(IKeyInventoryService keyInventoryService)
        {
            _keyInventoryService= keyInventoryService;
        }

        [HttpPost("keyinventory-list")]

        public async Task<IActionResult> GetKeyInventoryList()
        {
            try
            {
                var result = await _keyInventoryService.GetKeyInventoryList();
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("keyinventory-details")]
        public async Task<IActionResult> GetKeyInventoryDeatails(StringIdRequest request)
        {
            try
            {
                var result = await _keyInventoryService.GetKeyInventoryDeatails(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
                
            }
        }
        [HttpPost("keyinventory-save")]
        public async Task<IActionResult> UpsertKeyInventory(KeyInventory request)
        {
            try
            {
                var result = await _keyInventoryService.UpserttKeyInventory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
