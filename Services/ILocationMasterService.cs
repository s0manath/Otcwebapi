using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface ILocationMasterService
    {
         Task<LocationMaster> GetLocationMasterDetails(StringIdRequest request);
         Task<IEnumerable<LocationMaster>> GetLocationMasterList();
         //Task GetLocationMasterUpsert();
    }
}
