using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.IO.Compression;
using System.Text;

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
            "scheduled" => "India1_sp_GetScheduledDetailReport",
            "route-details" => "India1_sp_GetRouteConfigureDetailsReport",
            "otc-checkout" => "India1_Sp_getotccheckoutreport",
            "atm-detail" => "India1_sp_GetATMDetailReport",
            "custodian-wise" => "India1_sp_GetCustodianWiseReport",
            "otc-reset" => "India1_sp_GetOtcResetReport",
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

    public async Task StreamReportToZipAsync(ReportRequest request, Stream outputStream)
    {
        string spName = GetSpName(request.ReportType);

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(spName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 300; // 5 mins timeout for 1M data

                if (request.ReportType.ToLower() == "audit")
                {
                    command.Parameters.AddWithValue("@AuditDate", request.FromDate);
                    command.Parameters.AddWithValue("@SearchText", "");
                }
                else
                {
                    command.Parameters.AddWithValue("@FromDate", $"{request.FromDate} 00:00");
                    command.Parameters.AddWithValue("@ToDate", $"{request.ToDate} 23:59");
                    command.Parameters.AddWithValue("@Username", request.Username);
                }

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                {
                    using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
                    {
                        var fileName = $"{request.ReportType}_{DateTime.Now:yyyyMMddHHmm}.csv";
                        var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                        {
                            // Write Headers
                            var headerRow = new StringBuilder();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (i > 0) headerRow.Append(",");
                                headerRow.Append("\"").Append(reader.GetName(i).Replace("\"", "\"\"")).Append("\"");
                            }
                            await writer.WriteLineAsync(headerRow.ToString());

                            // Write Data
                            while (await reader.ReadAsync())
                            {
                                var dataRow = new StringBuilder();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (i > 0) dataRow.Append(",");
                                    var val = reader.GetValue(i);
                                    var strVal = val == DBNull.Value ? "" : val.ToString();
                                    dataRow.Append("\"").Append(strVal.Replace("\"", "\"\"")).Append("\"");
                                }
                                await writer.WriteLineAsync(dataRow.ToString());
                            }
                            await writer.FlushAsync();
                        }
                    }
                }
            }
        }
    }
}
