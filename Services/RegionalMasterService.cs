using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public class RegionalMasterService : IRegionalMasterService
    {
        private static readonly List<StateMaster> _mockStates = new()
        {
            new StateMaster { Id = 1, StateName = "Maharashtra", RegionId = 1, RegionName = "West", IsActive = true },
            new StateMaster { Id = 2, StateName = "Karnataka", RegionId = 2, RegionName = "South", IsActive = true }
        };

        private static readonly List<DistrictMaster> _mockDistricts = new()
        {
            new DistrictMaster { Id = 1, DistrictName = "Mumbai", StateId = 1, StateName = "Maharashtra", IsActive = true },
            new DistrictMaster { Id = 2, DistrictName = "Bangalore", StateId = 2, StateName = "Karnataka", IsActive = true }
        };

        private static readonly List<ZomMaster> _mockZoms = new()
        {
            new ZomMaster { Id = 1, ZomName = "Mumbai-Zone-1", RegionId = 1, RegionName = "West", LocationId = 1, LocationName = "Mumbai North", IsActive = true },
            new ZomMaster { Id = 2, ZomName = "Bangalore-Zone-1", RegionId = 2, RegionName = "South", LocationId = 2, LocationName = "Electronic City", IsActive = true }
        };

        // States
        public Task<List<StateMaster>> GetStatesAsync(RegionalSearchRequest request)
        {
            var result = _mockStates.AsQueryable();
            if (!string.IsNullOrEmpty(request.Name))
                result = result.Where(x => x.StateName.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(result.ToList());
        }

        public Task<StateMaster?> GetStateByIdAsync(int id) => Task.FromResult<StateMaster?>(_mockStates.FirstOrDefault(x => x.Id == id));

        public Task<bool> SaveStateAsync(StateMaster state)
        {
            if (state.Id == 0)
            {
                state.Id = _mockStates.Any() ? _mockStates.Max(x => x.Id) + 1 : 1;
                _mockStates.Add(state);
            }
            else
            {
                var existing = _mockStates.FirstOrDefault(x => x.Id == state.Id);
                if (existing != null)
                {
                    existing.StateName = state.StateName;
                    existing.RegionId = state.RegionId;
                    existing.IsActive = state.IsActive;
                }
            }
            return Task.FromResult(true);
        }

        // Districts
        public Task<List<DistrictMaster>> GetDistrictsAsync(RegionalSearchRequest request)
        {
            var result = _mockDistricts.AsQueryable();
            if (!string.IsNullOrEmpty(request.Name))
                result = result.Where(x => x.DistrictName.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            if (request.ParentId.HasValue)
                result = result.Where(x => x.StateId == request.ParentId.Value);
            return Task.FromResult(result.ToList());
        }

        public Task<DistrictMaster?> GetDistrictByIdAsync(int id) => Task.FromResult<DistrictMaster?>(_mockDistricts.FirstOrDefault(x => x.Id == id));

        public Task<bool> SaveDistrictAsync(DistrictMaster district)
        {
            if (district.Id == 0)
            {
                district.Id = _mockDistricts.Any() ? _mockDistricts.Max(x => x.Id) + 1 : 1;
                _mockDistricts.Add(district);
            }
            else
            {
                var existing = _mockDistricts.FirstOrDefault(x => x.Id == district.Id);
                if (existing != null)
                {
                    existing.DistrictName = district.DistrictName;
                    existing.StateId = district.StateId;
                    existing.IsActive = district.IsActive;
                }
            }
            return Task.FromResult(true);
        }

        // ZOMs
        public Task<List<ZomMaster>> GetZomsAsync(RegionalSearchRequest request)
        {
            var result = _mockZoms.AsQueryable();
            if (!string.IsNullOrEmpty(request.Name))
                result = result.Where(x => x.ZomName.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(result.ToList());
        }

        public Task<ZomMaster?> GetZomByIdAsync(int id) => Task.FromResult<ZomMaster?>(_mockZoms.FirstOrDefault(x => x.Id == id));

        public Task<bool> SaveZomAsync(ZomMaster zom)
        {
            if (zom.Id == 0)
            {
                zom.Id = _mockZoms.Any() ? _mockZoms.Max(x => x.Id) + 1 : 1;
                _mockZoms.Add(zom);
            }
            else
            {
                var existing = _mockZoms.FirstOrDefault(x => x.Id == zom.Id);
                if (existing != null)
                {
                    existing.ZomName = zom.ZomName;
                    existing.RegionId = zom.RegionId;
                    existing.LocationId = zom.LocationId;
                    existing.IsActive = zom.IsActive;
                }
            }
            return Task.FromResult(true);
        }
    }
}
