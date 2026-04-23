using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using Microsoft.Extensions.Configuration;

namespace OTC.Api.Services;

public class RouteService : IRouteService
{
    private readonly string _connectionString;

    public RouteService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
    }

    public async Task<IEnumerable<RouteListItem>> GetRouteListAsync(
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
        string? searchValue = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var parameters = new DynamicParameters();
            
            if (!string.IsNullOrEmpty(fromDate)) parameters.Add("@DateFrom", DateTime.Parse(fromDate).Date);
            else parameters.Add("@DateFrom", DBNull.Value);

            if (!string.IsNullOrEmpty(toDate)) parameters.Add("@DateTo", DateTime.Parse(toDate).Date.AddDays(1).AddSeconds(-1));
            else parameters.Add("@DateTo", DBNull.Value);
            parameters.Add("@Username", username);

            var data = await connection.QueryAsync<RouteListItem>("India1_USP_FillRouteConfig",parameters,commandType: CommandType.StoredProcedure);
            
            return data.ToList();
        }
    }

    public async Task<RouteFilterOptions> GetFilterOptionsAsync()
    {
        var options = new RouteFilterOptions();
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            options.Regions = (await connection.QueryAsync<FilterItem>("select roname as Name, rocode as Code from romast order by ROName asc")).ToList();
            options.Districts = (await connection.QueryAsync<FilterItem>("select district_name as Name, district_id as Code from DistrictMaster order by district_name asc")).ToList();
            options.Franchises = (await connection.QueryAsync<FilterItem>("select FranchiseName as Name, FranchiseCode as Code from FranchiseMaster order by FranchiseName asc")).ToList();
            options.Zoms = (await connection.QueryAsync<FilterItem>("select ZOMName as Name, ZOMCode as Code from ZOMMaster order by ZOMName asc")).ToList();
            options.ActivityTypes = (await connection.QueryAsync<string>("select SubVal from submaster where [Key]='Indent' order by SubVal asc")).ToList();
        }
        return options;
    }

    public async Task<IEnumerable<CustodianListItem>> GetCustodiansAsync(string scheduleId)
    {

        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<CustodianListItem>("dbo.GetCustodiansByScheduleId",new { ScheduleId = scheduleId },commandType: CommandType.StoredProcedure);

    }

    public async Task<bool> SaveRouteAsync(RouteSaveRequest request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            // Check if Route Config already exists
            var existing = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "select RouteConfig_Id, RouteKey from RouteConfig WITH (NOLOCK) where Schedule_Id = @ScheduleId",
                new { ScheduleId = request.ScheduleId }
            );

            string module = existing == null ? "Insert" : "Update";
            string spName = (request.UpdateAll && existing != null && (string)existing.RouteKey == request.RouteKey) 
                ? "sp_RouteConfigBulk" : "sp_RouteConfig";

            var parameters = new DynamicParameters();
            parameters.Add("@AtmId", request.AtmId);
            parameters.Add("@RouteConfig_Id", existing?.RouteConfig_Id ?? string.Empty);
            parameters.Add("@Schedule_Id", request.ScheduleId);
            parameters.Add("@RouteKey", request.RouteKey);
            parameters.Add("@Custodian1", request.Custodian1);
            parameters.Add("@Custodian2", request.Custodian2);
            parameters.Add("@CreatedBy", request.Username);
            parameters.Add("@Module", module);

            await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
    }

    public async Task<RouteListItem?> GetRouteDetailsAsync(string scheduleId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string sql = @"
                select 
                    atm_schedule.Schedule_Id as Id,
                    atm_schedule.Schedule_Id as ScheduleId,
                    atm_schedule.ATMID as AtmId,
                    atm_schedule.Activity_Type as ActivityType,
                    RouteConfig.RouteConfig_Id as RouteId,
                    RouteConfig.RouteKey as RouteKey, 
                    RouteConfig.Custodian1 as Custodian1,
                    RouteConfig.Custodian2 as Custodian2
                from atm_schedule 
                left join RouteConfig with (Nolock) on ATM_Schedule.Schedule_Id = RouteConfig.Schedule_Id 
                where atm_schedule.Schedule_Id = @ScheduleId";

            return await connection.QueryFirstOrDefaultAsync<RouteListItem>(sql, new { ScheduleId = scheduleId });
        }
    }
}
