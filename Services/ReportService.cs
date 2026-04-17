using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;

namespace OTC.Api.Services;

public class ReportService : IReportService
{
    private readonly string _connectionString;

    public ReportService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new Exception("Connection string 'DefaultConnection' not found.");
    }

    private string GetSpName(string reportType)
    {
        return reportType.ToLower() switch
        {
            "scheduled" => "xxx",
            "route-details" => "xxx",
            "otc-checkout" => "xxx",
            "atm-detail" => "xxx",
            "custodian-wise" => "xxx",
            "otc-reset" => "xxx",
            "otc-activity" => "sp_GetOTCActivityDetailReport",
            "audit" => "usp_GetAuditLog_Diff_ByDate_OneRow",
            _ => throw new Exception($"Report type '{reportType}' not supported.")
        };
    }

    public async Task<ReportDataResponse> GetReportDataAsync(ReportRequest request)
    {
        string spName = GetSpName(request.ReportType);
        
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var parameters = new DynamicParameters();
            
            if (request.ReportType.ToLower() == "audit")
            {
                parameters.Add("@AuditDate", request.FromDate);
                parameters.Add("@SearchText", ""); // Filter value removed
            }
            else
            {
                parameters.Add("@FromDate", $"{request.FromDate} 00:00");
                parameters.Add("@ToDate", $"{request.ToDate} 23:59");
                parameters.Add("@Username", request.Username);
            }

            var data = await connection.QueryAsync<dynamic>(spName, parameters, commandType: CommandType.StoredProcedure);
            int totalCount = data.Count();

            var columns = new List<string>();
            if (data.Any())
            {
                var firstRow = (IDictionary<string, object>)data.First();
                columns = firstRow.Keys.ToList();
            }

            return new ReportDataResponse { Columns = columns, Data = data, TotalCount = totalCount };
        }
    }
}
