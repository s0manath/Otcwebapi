using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface ILoginPendingService
    {
        Task<LoginPendingModels> GetLoginPendingDeatils(StringIdRequest request);
        Task<IEnumerable<LoginPendingModels>> GetLoginPendingList();
    }
}
