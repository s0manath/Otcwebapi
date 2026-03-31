using System.Collections.Generic;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IAdminMasterService
    {
        // Location Master
        Task<List<LocationMaster>> GetLocationsAsync(AdminMasterSearchRequest request);
        Task<bool> SaveLocationAsync(LocationMaster location);
        Task<bool> DeleteLocationAsync(int id);

        // Region Master
        Task<List<RegionMaster>> GetRegionsAsync(AdminMasterSearchRequest request);
        Task<bool> SaveRegionAsync(RegionMaster region);
        Task<bool> DeleteRegionAsync(int id);

        // Key Inventory
        Task<List<KeyInventoryMaster>> GetKeyInventoryAsync(AdminMasterSearchRequest request);
        Task<bool> SaveKeyInventoryAsync(KeyInventoryMaster key);
        Task<bool> DeleteKeyInventoryAsync(int id);

        // OneLine Master
        Task<List<OneLineMaster>> GetOneLineMastersAsync(AdminMasterSearchRequest request);
        Task<bool> SaveOneLineMasterAsync(OneLineMaster master);
        Task<bool> DeleteOneLineMasterAsync(int id);

        // Site Access Master
        Task<List<SiteAccessMaster>> GetSiteAccessMastersAsync(AdminMasterSearchRequest request);
        Task<bool> SaveSiteAccessMasterAsync(SiteAccessMaster master);
        Task<bool> DeleteSiteAccessMasterAsync(int id);

        // Route Master (Admin)
        Task<List<RouteMasterAdmin>> GetRouteMastersAdminAsync(AdminMasterSearchRequest request);
        Task<bool> SaveRouteMasterAdminAsync(RouteMasterAdmin route);
        Task<bool> DeleteRouteMasterAdminAsync(int id);

        // Login Mappings
        Task<List<CustodianLoginMapping>> GetCustodianLoginMappingsAsync();
        Task<bool> SaveCustodianLoginMappingAsync(CustodianLoginMapping mapping);
        
        Task<List<ZomLoginMapping>> GetZomLoginMappingsAsync();
        Task<bool> SaveZomLoginMappingAsync(ZomLoginMapping mapping);

        // Pending Requests
        Task<List<PendingLoginRequest>> GetPendingLoginRequestsAsync();
        Task<bool> ProcessLoginRequestAsync(MappingApprovalRequest request);

        // Utilities
        Task<bool> BulkUpdateRouteKeysAsync(List<string> atmIds, string routeKey);
    }
}
