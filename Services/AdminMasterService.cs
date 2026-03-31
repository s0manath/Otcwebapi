using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public class AdminMasterService : IAdminMasterService
    {
        // Mock data for initial development
        private static readonly List<LocationMaster> _locations = new()
        {
            new LocationMaster { Id = 1, LocationName = "Downtown", RegionCode = "REG001", RegionName = "North Region" },
            new LocationMaster { Id = 2, LocationName = "Uptown", RegionCode = "REG002", RegionName = "South Region" }
        };

        private static readonly List<RegionMaster> _regions = new()
        {
            new RegionMaster { Id = 1, RegionCode = "REG001", RegionName = "North Region" },
            new RegionMaster { Id = 2, RegionCode = "REG002", RegionName = "South Region" }
        };

        private static readonly List<PendingLoginRequest> _pendingRequests = new()
        {
            new PendingLoginRequest { Id = 1, Username = "cust_user1", CustodianOrZomName = "John Doe", RequestDate = DateTime.Now.AddDays(-1), RequestFor = "Login", MobileInfo = "Android 13, Samsung S22", Comments = "New device registration", Status = "Pending" },
            new PendingLoginRequest { Id = 2, Username = "zom_user1", CustodianOrZomName = "Jane Smith", RequestDate = DateTime.Now.AddHours(-5), RequestFor = "Password Reset", MobileInfo = "iOS 16, iPhone 14", Comments = "Forgot password", Status = "Pending" }
        };

        private static readonly List<KeyInventoryMaster> _keys = new();
        private static readonly List<OneLineMaster> _oneLines = new();
        private static readonly List<SiteAccessMaster> _siteAccess = new();
        private static readonly List<RouteMasterAdmin> _routeMasters = new();
        private static readonly List<CustodianLoginMapping> _custodianMappings = new();
        private static readonly List<ZomLoginMapping> _zomMappings = new();

        public async Task<List<LocationMaster>> GetLocationsAsync(AdminMasterSearchRequest request)
        {
            await Task.Delay(100);
            return _locations.Where(l => string.IsNullOrEmpty(request.Query) || l.LocationName.Contains(request.Query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<bool> SaveLocationAsync(LocationMaster location)
        {
            if (location.Id > 0)
            {
                var existing = _locations.FirstOrDefault(l => l.Id == location.Id);
                if (existing != null)
                {
                    existing.LocationName = location.LocationName;
                    existing.RegionCode = location.RegionCode;
                    existing.RegionName = location.RegionName;
                    existing.IsActive = location.IsActive;
                }
            }
            else
            {
                location.Id = _locations.Count > 0 ? _locations.Max(l => l.Id) + 1 : 1;
                _locations.Add(location);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var item = _locations.FirstOrDefault(l => l.Id == id);
            if (item != null) _locations.Remove(item);
            return await Task.FromResult(true);
        }

        public async Task<List<RegionMaster>> GetRegionsAsync(AdminMasterSearchRequest request)
        {
            return await Task.FromResult(_regions);
        }

        public async Task<bool> SaveRegionAsync(RegionMaster region)
        {
            if (region.Id > 0)
            {
                var existing = _regions.FirstOrDefault(r => r.Id == region.Id);
                if (existing != null)
                {
                    existing.RegionCode = region.RegionCode;
                    existing.RegionName = region.RegionName;
                    existing.IsActive = region.IsActive;
                }
            }
            else
            {
                region.Id = _regions.Count > 0 ? _regions.Max(r => r.Id) + 1 : 1;
                _regions.Add(region);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteRegionAsync(int id)
        {
            var item = _regions.FirstOrDefault(r => r.Id == id);
            if (item != null) _regions.Remove(item);
            return await Task.FromResult(true);
        }

        public async Task<List<KeyInventoryMaster>> GetKeyInventoryAsync(AdminMasterSearchRequest request) => await Task.FromResult(_keys);
        public async Task<bool> SaveKeyInventoryAsync(KeyInventoryMaster key)
        {
             if (key.Id > 0) { /* Update logic */ }
             else { key.Id = _keys.Count + 1; _keys.Add(key); }
             return await Task.FromResult(true);
        }
        public async Task<bool> DeleteKeyInventoryAsync(int id) => await Task.FromResult(true);

        public async Task<List<OneLineMaster>> GetOneLineMastersAsync(AdminMasterSearchRequest request) => await Task.FromResult(_oneLines);
        public async Task<bool> SaveOneLineMasterAsync(OneLineMaster master) => await Task.FromResult(true);
        public async Task<bool> DeleteOneLineMasterAsync(int id) => await Task.FromResult(true);

        public async Task<List<SiteAccessMaster>> GetSiteAccessMastersAsync(AdminMasterSearchRequest request) => await Task.FromResult(_siteAccess);
        public async Task<bool> SaveSiteAccessMasterAsync(SiteAccessMaster master) => await Task.FromResult(true);
        public async Task<bool> DeleteSiteAccessMasterAsync(int id) => await Task.FromResult(true);

        public async Task<List<RouteMasterAdmin>> GetRouteMastersAdminAsync(AdminMasterSearchRequest request) => await Task.FromResult(_routeMasters);
        public async Task<bool> SaveRouteMasterAdminAsync(RouteMasterAdmin route) => await Task.FromResult(true);
        public async Task<bool> DeleteRouteMasterAdminAsync(int id) => await Task.FromResult(true);

        public async Task<List<CustodianLoginMapping>> GetCustodianLoginMappingsAsync() => await Task.FromResult(_custodianMappings);
        public async Task<bool> SaveCustodianLoginMappingAsync(CustodianLoginMapping mapping) 
        {
            _custodianMappings.Add(mapping);
            return await Task.FromResult(true);
        }

        public async Task<List<ZomLoginMapping>> GetZomLoginMappingsAsync() => await Task.FromResult(_zomMappings);
        public async Task<bool> SaveZomLoginMappingAsync(ZomLoginMapping mapping)
        {
            _zomMappings.Add(mapping);
            return await Task.FromResult(true);
        }

        public async Task<List<PendingLoginRequest>> GetPendingLoginRequestsAsync() => await Task.FromResult(_pendingRequests);
        public async Task<bool> ProcessLoginRequestAsync(MappingApprovalRequest request)
        {
            var pending = _pendingRequests.FirstOrDefault(p => p.Id == request.RequestId);
            if (pending != null)
            {
                pending.Status = request.IsApproved ? "Approved" : "Rejected";
                pending.Comments = request.Comments;
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> BulkUpdateRouteKeysAsync(List<string> atmIds, string routeKey)
        {
            // Simulate bulk update
            return await Task.FromResult(true);
        }
    }
}
