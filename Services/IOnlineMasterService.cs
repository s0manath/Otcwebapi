using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IOnlineMasterService
    {
        Task<OnlineMaster> GetOnlineMasterDetails(StringIdRequest request);
        Task<IEnumerable<OnlineMaster>> GetZomMasterList();
        Task <string>UpsertonlineMaster(OnlineMaster request);
    }
}
