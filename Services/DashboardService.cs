using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using Microsoft.Extensions.Configuration;

namespace OTC.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly string _connectionString;

    public DashboardService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
    }

    public async Task<SummaryMetrics> GetSummaryMetricsAsync(string date, string username)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var parameters = new DynamicParameters();
            parameters.Add("@Date", string.IsNullOrEmpty(date) ? DateTime.Now.ToString("yyyy-MM-dd") : date);
            parameters.Add("@Username", string.IsNullOrEmpty(username) ? "admin" : username);

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "proc_HomeScreenCard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result != null)
            {
                var dict = (IDictionary<string, object>)result;
                return new SummaryMetrics
                {
                    Total = dict.TryGetValue("Total", out var t) && t != null ? Convert.ToInt32(t) : 0,
                    Completed = dict.TryGetValue("Completed", out var c) && c != null ? Convert.ToInt32(c) : 0,
                    Reset = dict.TryGetValue("Reset", out var r) && r != null ? Convert.ToInt32(r) : 0,
                    Pending = dict.TryGetValue("Skipped", out var s) && s != null ? Convert.ToInt32(s) : 0
                };
            }
            return new SummaryMetrics();
        }
    }

    public async Task<IEnumerable<ChartData>> GetChartDataAsync(string date, string username)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var parameters = new DynamicParameters();
            parameters.Add("@dateval", string.IsNullOrEmpty(date) ? DateTime.Now.ToString("yyyy-MM-dd") : date);
            parameters.Add("@uname", string.IsNullOrEmpty(username) ? "admin" : username);

            return await connection.QueryAsync<ChartData>(
                "Proc_OTCHomeDashboardChart",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }

    public async Task<IEnumerable<DistrictReportItem>> GetDistrictReportAsync(string date, string username)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Date", string.IsNullOrEmpty(date)
                ? DateTime.Now.ToString("yyyy-MM-dd")
                : date);

            parameters.Add("@UserName", string.IsNullOrEmpty(username)
                ? "admin"
                : username);

            var data = await connection.QueryAsync<dynamic>(
                "usp_GetHomeDepartmentReportDistrictWise",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return data.Select(d =>
            {
                var dict = (IDictionary<string, object>)d;

                return new DistrictReportItem
                {
                    DistrictName = dict.TryGetValue("District Name", out var dn)
                        ? dn?.ToString() ?? "Unknown"
                        : "Unknown",

                    Total = dict.TryGetValue("Total", out var t)
                        ? t?.ToString() ?? "0"
                        : "0",

                    Completed = dict.TryGetValue("Completed", out var c)
                        ? c?.ToString() ?? "0"
                        : "0",

                    ResetAndCompleted = dict.TryGetValue("Reset and Completed", out var r)
                        ? r?.ToString() ?? "0"
                        : "0",

                    Pending = dict.TryGetValue("Skipped", out var s)
                        ? s?.ToString() ?? "0"
                        : "0",

                    DistrictCode = dict.TryGetValue("DistrictCode", out var dc)
                        ? dc?.ToString() ?? string.Empty
                        : string.Empty
                };
            }).ToList();
        }
    }
}
