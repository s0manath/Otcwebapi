using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IRegionMasterService
    {
        Task<RegionMaster> GetRegionMasterDetails(StringIdRequest request);
        Task<IEnumerable<RegionMaster>> GetRegionMasterList();
        Task<string> UpsertRegionMaster(RegionMaster request);
    }
}
