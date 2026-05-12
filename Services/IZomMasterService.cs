using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IZomMasterService
    {
        Task <ZomMaster> GetZomMasterDetails(StringIdRequest request);
        Task<IEnumerable<ZomMaster>> GetZomMasterList();
        Task<string> UpsertZomMaster(ZomMaster request);
    }
}
