using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IRouteMasterService
    {
        Task<RouteMaster> GetRouteMasterDetails(StringIdRequest request);
        Task<IEnumerable<RouteMaster>> GetRouteMasterList();
        Task<string> UpsertRoutemaster(RouteMaster request);
    }
}
