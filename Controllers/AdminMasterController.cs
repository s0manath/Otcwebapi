using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminMasterController : ControllerBase
    {
        private readonly IAdminMasterService _adminService;

        public AdminMasterController(IAdminMasterService adminService)
        {
            _adminService = adminService;
        }

        #region Masters

        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations([FromQuery] AdminMasterSearchRequest request) => Ok(await _adminService.GetLocationsAsync(request));

        [HttpPost("locations")]
        public async Task<IActionResult> SaveLocation([FromBody] LocationMaster location) => Ok(await _adminService.SaveLocationAsync(location));

        [HttpDelete("locations/{id}")]
        public async Task<IActionResult> DeleteLocation(int id) => Ok(await _adminService.DeleteLocationAsync(id));

        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions([FromQuery] AdminMasterSearchRequest request) => Ok(await _adminService.GetRegionsAsync(request));

        [HttpPost("regions")]
        public async Task<IActionResult> SaveRegion([FromBody] RegionMaster region) => Ok(await _adminService.SaveRegionAsync(region));

        [HttpGet("key-inventory")]
        public async Task<IActionResult> GetKeyInventory([FromQuery] AdminMasterSearchRequest request) => Ok(await _adminService.GetKeyInventoryAsync(request));

        [HttpPost("key-inventory")]
        public async Task<IActionResult> SaveKeyInventory([FromBody] KeyInventoryMaster key) => Ok(await _adminService.SaveKeyInventoryAsync(key));

        [HttpGet("one-lines")]
        public async Task<IActionResult> GetOneLines([FromQuery] AdminMasterSearchRequest request) => Ok(await _adminService.GetOneLineMastersAsync(request));

        [HttpPost("one-lines")]
        public async Task<IActionResult> SaveOneLine([FromBody] OneLineMaster master) => Ok(await _adminService.SaveOneLineMasterAsync(master));

        [HttpGet("site-access")]
        public async Task<IActionResult> GetSiteAccess([FromQuery] AdminMasterSearchRequest request) => Ok(await _adminService.GetSiteAccessMastersAsync(request));

        [HttpPost("site-access")]
        public async Task<IActionResult> SaveSiteAccess([FromBody] SiteAccessMaster master) => Ok(await _adminService.SaveSiteAccessMasterAsync(master));

        [HttpGet("route-masters")]
        public async Task<IActionResult> GetRouteMasters([FromQuery] AdminMasterSearchRequest request) => Ok(await _adminService.GetRouteMastersAdminAsync(request));

        [HttpPost("route-masters")]
        public async Task<IActionResult> SaveRouteMaster([FromBody] RouteMasterAdmin route) => Ok(await _adminService.SaveRouteMasterAdminAsync(route));

        #endregion

        #region Mappings & Requests

        [HttpGet("mappings/custodian")]
        public async Task<IActionResult> GetCustodianMappings() => Ok(await _adminService.GetCustodianLoginMappingsAsync());

        [HttpPost("mappings/custodian")]
        public async Task<IActionResult> SaveCustodianMapping([FromBody] CustodianLoginMapping mapping) => Ok(await _adminService.SaveCustodianLoginMappingAsync(mapping));

        [HttpGet("mappings/zom")]
        public async Task<IActionResult> GetZomMappings() => Ok(await _adminService.GetZomLoginMappingsAsync());

        [HttpPost("mappings/zom")]
        public async Task<IActionResult> SaveZomMapping([FromBody] ZomLoginMapping mapping) => Ok(await _adminService.SaveZomLoginMappingAsync(mapping));

        [HttpGet("pending-requests")]
        public async Task<IActionResult> GetPendingRequests() => Ok(await _adminService.GetPendingLoginRequestsAsync());

        [HttpPost("process-request")]
        public async Task<IActionResult> ProcessRequest([FromBody] MappingApprovalRequest request) => Ok(await _adminService.ProcessLoginRequestAsync(request));

        #endregion

        #region Utilities

        [HttpPost("bulk-update-route-keys")]
        public async Task<IActionResult> BulkUpdateRouteKeys([FromBody] BulkUpdateRouteKeyRequest request) 
        {
            return Ok(await _adminService.BulkUpdateRouteKeysAsync(request.AtmIds, request.RouteKey));
        }

        #endregion
    }

    public class BulkUpdateRouteKeyRequest
    {
        public List<string> AtmIds { get; set; } = new();
        public string RouteKey { get; set; } = string.Empty;
    }
}
