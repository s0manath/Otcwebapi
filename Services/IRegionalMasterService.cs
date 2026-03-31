using System.Collections.Generic;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IRegionalMasterService
    {
        // States
        Task<List<StateMaster>> GetStatesAsync(RegionalSearchRequest request);
        Task<StateMaster?> GetStateByIdAsync(int id);
        Task<bool> SaveStateAsync(StateMaster state);

        // Districts
        Task<List<DistrictMaster>> GetDistrictsAsync(RegionalSearchRequest request);
        Task<DistrictMaster?> GetDistrictByIdAsync(int id);
        Task<bool> SaveDistrictAsync(DistrictMaster district);

        // ZOMs
        Task<List<ZomMaster>> GetZomsAsync(RegionalSearchRequest request);
        Task<ZomMaster?> GetZomByIdAsync(int id);
        Task<bool> SaveZomAsync(ZomMaster zom);
    }
}
