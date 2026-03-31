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

            parameters.Add("@Region", string.IsNullOrEmpty(region) ? DBNull.Value : region);
            parameters.Add("@Location", string.IsNullOrEmpty(district) ? DBNull.Value : district);
            parameters.Add("@Franchise", string.IsNullOrEmpty(franchise) ? DBNull.Value : franchise);
            parameters.Add("@ZOM", string.IsNullOrEmpty(zom) ? DBNull.Value : zom);
            parameters.Add("@ActivityType", string.IsNullOrEmpty(activityType) ? DBNull.Value : activityType);
            parameters.Add("@FilterStatus", string.IsNullOrEmpty(status) ? DBNull.Value : status);
            parameters.Add("@ChkConfig", string.IsNullOrEmpty(chkConfig) ? DBNull.Value : chkConfig);
            parameters.Add("@Field", string.IsNullOrEmpty(searchField) ? DBNull.Value : searchField);
            parameters.Add("@Criteria", string.IsNullOrEmpty(criteria) ? DBNull.Value : criteria);
            parameters.Add("@Value", string.IsNullOrEmpty(searchValue) ? DBNull.Value : searchValue);
            parameters.Add("@Username", username);

            return await connection.QueryAsync<RouteListItem>(
                "spFillRouteConfig",
                parameters,
                commandType: CommandType.StoredProcedure
            );
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
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string sql = @"
                select CustodianMaster.CustodianName, CustodianMaster.CustodianCode 
                from ATM_Schedule 
                inner join CustodianMapping on CustodianMapping.EquipId = ATM_Schedule.ATMID  
                inner join CustodianMaster on CustodianMaster.CustodianCode = CustodianMapping.CustodianCode  
                INNER JOIN RouteMaster ON RouteMaster.CustodianID = CustodianMaster.CustodianID AND RouteMaster.TouchKeyID = CustodianMaster.TouchKeyID 
                where (lockflg is null or lockflg='' or lockflg=0) and Schedule_Id = @ScheduleId";

            return await connection.QueryAsync<CustodianListItem>(sql, new { ScheduleId = scheduleId });
        }
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
            parameters.Add("@AtmId", request.ATMID);
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
                select atm_schedule.Schedule_Id as ID, ATMID, Activity_Type, rm.Routekey, 
                       c1.CustodianCode as Custodian1, c2.CustodianCode as Custodian2
                from atm_schedule 
                left join RouteConfig on ATM_Schedule.Schedule_Id = RouteConfig.Schedule_Id 
                left join CustodianMaster c1 on c1.CustodianCode = RouteConfig.Custodian1 
                left join RouteMaster rm on C1.CustodianID = rm.CustodianID and C1.TouchKeyID = rm.TouchKeyID
                left join CustodianMaster c2 on c2.CustodianCode = RouteConfig.Custodian2 
                where atm_schedule.Schedule_Id = @ScheduleId";

            return await connection.QueryFirstOrDefaultAsync<RouteListItem>(sql, new { ScheduleId = scheduleId });
        }
    }
}
