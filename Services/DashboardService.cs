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

    public async Task<DashboardData> GetDashboardDataAsync(string date, string username)
    {
        var result = new DashboardData();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // 1. Get District Performance (using legacy SP pattern logic)
            // Stored Procedure: usp_GetHomeDepartmentReportDistrictWise
            var performanceData = await connection.QueryAsync<dynamic>(
                "usp_GetHomeDepartmentReportDistrictWise",
                new { Date = date, UserName = username },
                commandType: CommandType.StoredProcedure
            );

            result.Performance = performanceData.Select(d => new DistrictPerformance
            {
                DistrictName = d.LocationName ?? d.DistrictName ?? "Unknown",
                DistrictCode = d.LocationCode ?? d.DistrictCode ?? "",
                Total = d.Total is string s ? int.Parse(s) : (int)(d.Total ?? 0),
                Completed = d.Completed is string s1 ? int.Parse(s1) : (int)(d.Completed ?? 0),
                Reset = d.Reset is string s2 ? int.Parse(s2) : (int)(d.Reset ?? 0),
                Pending = d.Pending is string s3 ? int.Parse(s3) : (int)(d.Pending ?? 0)
            }).ToList();

            // 2. Generate Stats based on aggregated performance data
            // In the legacy system, these stats are often derived from the same data or similar queries
            int totalAtms = result.Performance.Sum(p => p.Total);
            int totalCompleted = result.Performance.Sum(p => p.Completed);
            int totalReset = result.Performance.Sum(p => p.Reset);
            int totalPending = result.Performance.Sum(p => p.Pending);

            result.Stats = new List<DashboardStats>
            {
                new DashboardStats { Label = "ATM Schedule", Value = totalAtms.ToString("N0"), Change = "+0%", Color = "bg-[#24b8dd]" },
                new DashboardStats { Label = "OTC Completed", Value = totalCompleted.ToString("N0"), Change = "+0%", Color = "bg-[#1bcf8d]" },
                new DashboardStats { Label = "OTC Reset & Completed", Value = totalReset.ToString("N0"), Change = "+0%", Color = "bg-[#e9494c]" },
                new DashboardStats { Label = "OTC Pending", Value = totalPending.ToString("N0"), Change = "+0%", Color = "bg-[#6258ff]" },
                new DashboardStats { Label = "Total ATMs", Value = totalAtms.ToString("N0"), Change = "+0%", Color = "bg-amber-500" },
                new DashboardStats { Label = "Active Routes", Value = result.Performance.Count.ToString(), Change = "+0", Color = "bg-slate-700" }
            };
        }

        return result;
    }
}
