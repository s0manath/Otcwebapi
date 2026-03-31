using OTC.Api.Models;

namespace OTC.Api.Services;

public interface IDashboardService
{
    Task<DashboardData> GetDashboardDataAsync(string date, string username);
}
