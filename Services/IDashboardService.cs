using OTC.Api.Models;

namespace OTC.Api.Services;

public interface IDashboardService
{
    Task<SummaryMetrics> GetSummaryMetricsAsync(string date, string username);
    Task<IEnumerable<ChartData>> GetChartDataAsync(string date, string username);
    Task<IEnumerable<DistrictReportItem>> GetDistrictReportAsync(string date, string username);
}
