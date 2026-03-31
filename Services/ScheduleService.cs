using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using Microsoft.Extensions.Configuration;

namespace OTC.Api.Services;

public class ScheduleService : IScheduleService
{
    private readonly string _connectionString;

    public ScheduleService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
    }

    public async Task<IEnumerable<ScheduleListItem>> GetScheduleListAsync(string fromDate, string toDate, string username, string? searchField = null, string? startWith = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            // Legacy SP: usp_GetScheduleList
            // It expects FromDate, ToDate, UserName
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", DateTime.Parse(fromDate).Date);
            parameters.Add("@ToDate", DateTime.Parse(toDate).Date);
            parameters.Add("@UserName", username);

            var result = await connection.QueryAsync<ScheduleListItem>(
                "usp_GetScheduleList",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Manual filtering for "Search Field" and "Starts With" if not handled by SP
            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(startWith) && searchField != "None")
            {
                // Simple client-side filter for now as legacy code also had custom filter building
                return result.Where(x => {
                    var val = x.GetType().GetProperty(searchField.Split('.').Last())?.GetValue(x, null)?.ToString();
                    return val != null && val.StartsWith(startWith, StringComparison.OrdinalIgnoreCase);
                });
            }

            return result;
        }
    }

    public async Task<bool> InsertScheduleAsync(ScheduleInsertRequest request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            // Generate Schedule ID (Legacy pattern)
            string scheduleId = string.Empty;
            if (string.IsNullOrEmpty(request.BulkScheduleInfo))
            {
                var idResult = await connection.QueryFirstOrDefaultAsync<string>(
                    "select 'S' + RIGHT(REPLICATE(0, 9) + CAST(Schedule_Id AS varchar(7)), 9) as 'ID' from KeyGeneration"
                );
                scheduleId = idResult ?? throw new Exception("Failed to generate Schedule ID");
            }

            // Legacy SP: ScheduleRouteInsertV1
            var parameters = new DynamicParameters();
            parameters.Add("@Schedule_Id", scheduleId);
            parameters.Add("@ATMID", request.ATMID ?? string.Empty);
            parameters.Add("@Activity_Type", request.ActivityType);
            parameters.Add("@Schedule_Date", request.ScheduleDate);
            parameters.Add("@CreatedDate", DateTime.Now.ToString("MM/dd/yyyy hh:mm"));
            parameters.Add("@CreatedBy", request.Username);
            parameters.Add("@LastModifiedDate", string.Empty);
            parameters.Add("@LastModifiedBy", string.Empty);
            parameters.Add("@Module", "Insert");
            parameters.Add("@Comment", string.Empty);

            await connection.ExecuteAsync(
                "ScheduleRouteInsertV1",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }

    public async Task<bool> UpdateScheduleAsync(ScheduleUpdateRequest request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var parameters = new DynamicParameters();
            parameters.Add("@Schedule_Id", request.ScheduleId);
            parameters.Add("@ATMID", request.ATMID);
            parameters.Add("@Activity_Type", request.ActivityType);
            parameters.Add("@Schedule_Date", request.ScheduleDate);
            parameters.Add("@CreatedDate", string.Empty);
            parameters.Add("@CreatedBy", string.Empty);
            parameters.Add("@LastModifiedDate", DateTime.Now.ToString("MM/dd/yyyy hh:mm"));
            parameters.Add("@LastModifiedBy", request.Username);
            parameters.Add("@Module", "Update");
            parameters.Add("@Comment", request.Comment);

            await connection.ExecuteAsync(
                "ScheduleRouteInsertV1",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }

    public async Task<bool> DeleteScheduleAsync(string scheduleId, string username)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            // Check if routed (Legacy logic)
            var status = await connection.QueryFirstOrDefaultAsync<string>(
                "sp_ATM_Schedule_IsRouteConfig",
                new { ScheduleId = scheduleId },
                commandType: CommandType.StoredProcedure
            );

            if (status != "Yes") return false;

            await connection.ExecuteAsync(
                "sp_ATM_Schedule_Delete",
                new { 
                    ScheduleId = scheduleId,
                    DeletedDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm"),
                    DeletedBy = username
                },
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }

    public async Task<IEnumerable<ActivityType>> GetActivityTypesAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<dynamic>(
                "sp_GetIndentList",
                commandType: CommandType.StoredProcedure
            );

            return result.Select(x => new ActivityType { Name = x.SubVal }).ToList();
        }
    }
}
