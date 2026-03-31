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

        [HttpPost("locations-list")]
        public async Task<IActionResult> GetLocations([FromBody] AdminMasterSearchRequest request) => Ok(await _adminService.GetLocationsAsync(request));

        [HttpPost("locations")]
        public async Task<IActionResult> SaveLocation([FromBody] LocationMaster location) => Ok(await _adminService.SaveLocationAsync(location));

        [HttpPost("locations-delete")]
        public async Task<IActionResult> DeleteLocation([FromBody] IdRequest request) => Ok(await _adminService.DeleteLocationAsync(request.Id));

        [HttpPost("regions-list")]
        public async Task<IActionResult> GetRegions([FromBody] AdminMasterSearchRequest request) => Ok(await _adminService.GetRegionsAsync(request));

        [HttpPost("regions")]
        public async Task<IActionResult> SaveRegion([FromBody] RegionMaster region) => Ok(await _adminService.SaveRegionAsync(region));

        [HttpPost("key-inventory-list")]
        public async Task<IActionResult> GetKeyInventory([FromBody] AdminMasterSearchRequest request) => Ok(await _adminService.GetKeyInventoryAsync(request));

        [HttpPost("key-inventory")]
        public async Task<IActionResult> SaveKeyInventory([FromBody] KeyInventoryMaster key) => Ok(await _adminService.SaveKeyInventoryAsync(key));

        [HttpPost("one-lines-list")]
        public async Task<IActionResult> GetOneLines([FromBody] AdminMasterSearchRequest request) => Ok(await _adminService.GetOneLineMastersAsync(request));

        [HttpPost("one-lines")]
        public async Task<IActionResult> SaveOneLine([FromBody] OneLineMaster master) => Ok(await _adminService.SaveOneLineMasterAsync(master));

        [HttpPost("site-access-list")]
        public async Task<IActionResult> GetSiteAccess([FromBody] AdminMasterSearchRequest request) => Ok(await _adminService.GetSiteAccessMastersAsync(request));

        [HttpPost("site-access")]
        public async Task<IActionResult> SaveSiteAccess([FromBody] SiteAccessMaster master) => Ok(await _adminService.SaveSiteAccessMasterAsync(master));

        [HttpPost("route-masters-list")]
        public async Task<IActionResult> GetRouteMasters([FromBody] AdminMasterSearchRequest request) => Ok(await _adminService.GetRouteMastersAdminAsync(request));

        [HttpPost("route-masters")]
        public async Task<IActionResult> SaveRouteMaster([FromBody] RouteMasterAdmin route) => Ok(await _adminService.SaveRouteMasterAdminAsync(route));

        #endregion

        #region Mappings & Requests

        [HttpPost("mappings/custodian-list")]
        public async Task<IActionResult> GetCustodianMappings() => Ok(await _adminService.GetCustodianLoginMappingsAsync());

        [HttpPost("mappings/custodian")]
        public async Task<IActionResult> SaveCustodianMapping([FromBody] CustodianLoginMapping mapping) => Ok(await _adminService.SaveCustodianLoginMappingAsync(mapping));

        [HttpPost("mappings/zom-list")]
        public async Task<IActionResult> GetZomMappings() => Ok(await _adminService.GetZomLoginMappingsAsync());

        [HttpPost("mappings/zom")]
        public async Task<IActionResult> SaveZomMapping([FromBody] ZomLoginMapping mapping) => Ok(await _adminService.SaveZomLoginMappingAsync(mapping));

        [HttpPost("pending-requests-list")]
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
