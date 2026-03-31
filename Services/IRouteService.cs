using OTC.Api.Models;

namespace OTC.Api.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteListItem>> GetRouteListAsync(
        string? fromDate = null, 
        string? toDate = null, 
        string username = "admin",
        string? region = null,
        string? district = null,
        string? franchise = null,
        string? zom = null,
        string? activityType = null,
        string? status = null,
        string? chkConfig = null,
        string? searchField = null,
        string? criteria = null,
        string? searchValue = null
    );

    Task<RouteFilterOptions> GetFilterOptionsAsync();

    Task<IEnumerable<CustodianListItem>> GetCustodiansAsync(string scheduleId);

    Task<bool> SaveRouteAsync(RouteSaveRequest request);

    Task<RouteListItem?> GetRouteDetailsAsync(string scheduleId);
}
