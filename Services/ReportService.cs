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

    public async Task<ReportDataResponse> GetReportDataAsync(ReportRequest request)
    {
        string sql = string.Empty;
        var parameters = new DynamicParameters();

        switch (request.ReportType.ToLower())
        {
            case "scheduled":
                sql = GetScheduledDetailSql(request, parameters);
                break;
            case "route-details":
                sql = GetRouteMappedSql(request, parameters);
                break;
            case "otc-checkout":
                sql = GetOTCCheckoutSql(request, parameters);
                break;
            case "otc-activity":
                return await GetStoredProcedureReportAsync("sp_GetOTCActivityDetailReport", request);
            case "audit":
                return await GetStoredProcedureReportAsync("usp_GetAuditLog_Diff_ByDate_OneRow", request);
            default:
                throw new Exception($"Report type '{request.ReportType}' not supported.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var data = await connection.QueryAsync<dynamic>(sql, parameters);
            
            var columns = new List<string>();
            if (data.Any())
            {
                var firstRow = (IDictionary<string, object>)data.First();
                columns = firstRow.Keys.ToList();
            }

            return new ReportDataResponse
            {
                Columns = columns,
                Data = data
            };
        }
    }

    private string GetScheduledDetailSql(ReportRequest request, DynamicParameters parameters)
    {
        string sql = "select ATM_Schedule.Schedule_Id as 'Schedule ID', Purchase.EquipId as 'ATM ID', Bank.Address, Bank.City, Bank.State, Bank.PinCode, FranchiseMaster.FranchiseName, RoMast.ROName as 'Region', " +
                     "Locationmast.LocationName as 'Location', ATM_Schedule.Activity_Type as 'Activity Type', ATM_Schedule.CreatedBy as 'Scheduled By', format(ATM_Schedule.Schedule_Date,'dd/MM/yyyy') as 'Scheduled Date', " +
                     "Purchase.RouteKey as 'Route Key', ATM_Schedule.Comment as 'Remarks' from ATM_Schedule " +
                     "inner join Purchase with (Nolock) on Purchase.EquipId=ATM_Schedule.ATMID " +
                     "inner join Bank with (Nolock) on Bank.SiteID=Purchase.SiteID " +
                     "inner join PurchaseLookup with (Nolock) on Purchase.equipid=purchaselookup.Code " +
                     "left join Locationmast with (Nolock) on Locationmast.LocationCode=PurchaseLookup.Locationcode " +
                     "left join RoMast with (Nolock) on RoMast.ROCode=PurchaseLookup.Rocode " +
                     "left join FranchiseMaster with (Nolock) on FranchiseMaster.FranchiseCode=Purchase.FranchiseCode " +
                     "left join RouteConfig with (Nolock) on RouteConfig.Schedule_Id=ATM_Schedule.Schedule_Id where 1=1 ";

        if (!string.IsNullOrEmpty(request.FromDate) && !string.IsNullOrEmpty(request.ToDate))
        {
            sql += " and ATM_Schedule.Schedule_Date between @FromDate and @ToDate ";
            parameters.Add("@FromDate", $"{request.FromDate} 00:00");
            parameters.Add("@ToDate", $"{request.ToDate} 23:59");
        }

        if (!string.IsNullOrEmpty(request.FilterField) && !string.IsNullOrEmpty(request.FilterValue) && request.FilterField != "None")
        {
            sql += $" and {request.FilterField} like @FilterValue ";
            parameters.Add("@FilterValue", request.FilterValue + "%");
        }

        if (!string.IsNullOrEmpty(request.FranchiseCode))
        {
            sql += " and FranchiseMaster.FranchiseCode = @FranchiseCode ";
            parameters.Add("@FranchiseCode", request.FranchiseCode);
        }

        return sql + " order by Purchase.EquipId asc";
    }

    private string GetRouteMappedSql(ReportRequest request, DynamicParameters parameters)
    {
        string sql = "select RouteConfig.Schedule_Id as 'Schedule ID', RouteConfig_Id as 'Route Config ID', Purchase.EquipId as 'ATM ID', Bank.Address, Bank.City, Bank.State, Bank.PinCode, RoMast.ROName as 'Region', " +
                     "Locationmast.LocationName as 'Location', ATM_Schedule.Activity_Type as 'Activity Type', ATM_Schedule.CreatedBy as 'Scheduled By', format(ATM_Schedule.Schedule_Date,'dd/MM/yyyy') as 'Scheduled Date', " +
                     "RouteConfig.CreatedBy as 'Route Mapped By', RouteConfig.RouteKey as 'Route Key', c1.CustodianName as 'Custodian 1', c1.MobileNo as 'C1 Mobile', c2.CustodianName as 'Custodian 2', c2.MobileNo as 'C2 Mobile', " +
                     "case when statusupdate is null then 'Initiation' else statusupdate end as 'Status' from RouteConfig " +
                     "inner join ATM_Schedule with (Nolock) on RouteConfig.Schedule_Id=ATM_Schedule.Schedule_Id " +
                     "inner join Purchase with (Nolock) on Purchase.EquipId=ATM_Schedule.ATMID " +
                     "inner join Bank with (Nolock) on Bank.SiteID=Purchase.SiteID " +
                     "inner join PurchaseLookup with (Nolock) on Purchase.equipid=purchaselookup.Code " +
                     "left join Locationmast with (Nolock) on Locationmast.LocationCode=PurchaseLookup.Locationcode " +
                     "left join RoMast with (Nolock) on RoMast.ROCode=PurchaseLookup.Rocode " +
                     "inner join CustodianMaster c1 with (Nolock) on c1.CustodianCode=RouteConfig.Custodian1 " +
                     "left join CustodianMaster c2 with (Nolock) on c2.CustodianCode=RouteConfig.Custodian2 " +
                     "inner join FranchiseMaster with (Nolock) on FranchiseMaster.FranchiseCode=c1.FranschiseName where 1=1 ";

        if (!string.IsNullOrEmpty(request.FromDate) && !string.IsNullOrEmpty(request.ToDate))
        {
            sql += " and RouteConfig.CreatedDate between @FromDate and @ToDate ";
            parameters.Add("@FromDate", $"{request.FromDate} 00:00");
            parameters.Add("@ToDate", $"{request.ToDate} 23:59");
        }

        return sql + " order by Purchase.EquipId asc";
    }

    private string GetOTCCheckoutSql(ReportRequest request, DynamicParameters parameters)
    {
        string sql = "select Purchase.EquipId as 'ATM ID', Bank.Address, Bank.City, Bank.State, Bank.PinCode, RoMast.ROName as 'Region', Locationmast.LocationName as 'Location', " +
                     "ATM_Schedule.CreatedBy as 'Scheduled By', format(ATM_Schedule.Schedule_Date,'dd/MM/yyyy') as 'Scheduled Date', c1.CustodianName as 'Custodian 1', c2.CustodianName as 'Custodian 2', " +
                     "OTCGeneratedDate as 'Generated On', RouteConfig.CompletedDate as 'Checkout On', case when statusupdate is null then 'Initiation' else statusupdate end as 'Status' " +
                     "from RouteConfig inner join ATM_Schedule with (Nolock) on ATM_Schedule.Schedule_Id=RouteConfig.Schedule_Id " +
                     "left join Purchase with (Nolock) on Purchase.EquipId=ATM_Schedule.ATMID " +
                     "left join Bank with (Nolock) on Bank.SiteID=Purchase.SiteID " +
                     "left join PurchaseLookup with (Nolock) on Purchase.equipid=purchaselookup.Code " +
                     "left join Locationmast with (Nolock) on Locationmast.LocationCode=PurchaseLookup.Locationcode " +
                     "left join RoMast with (Nolock) on RoMast.ROCode=PurchaseLookup.Rocode " +
                     "left join CustodianMaster c1 with (Nolock) on c1.CustodianCode=RouteConfig.Custodian1 " +
                     "left join CustodianMaster c2 with (Nolock) on c2.CustodianCode=RouteConfig.Custodian2 where 1=1 ";

        if (!string.IsNullOrEmpty(request.FromDate) && !string.IsNullOrEmpty(request.ToDate))
        {
            sql += " and RouteConfig.CreatedDate between @FromDate and @ToDate ";
            parameters.Add("@FromDate", $"{request.FromDate} 00:00");
            parameters.Add("@ToDate", $"{request.ToDate} 23:59");
        }
        return sql;
    }

    private async Task<ReportDataResponse> GetStoredProcedureReportAsync(string spName, ReportRequest request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var parameters = new DynamicParameters();
            
            if (spName == "usp_GetAuditLog_Diff_ByDate_OneRow")
            {
                parameters.Add("@AuditDate", request.FromDate);
                parameters.Add("@SearchText", request.FilterValue ?? "");
            }
            else if (spName == "sp_GetOTCActivityDetailReport")
            {
                parameters.Add("@FromDate", $"{request.FromDate} 00:00");
                parameters.Add("@ToDate", $"{request.ToDate} 23:59");
                parameters.Add("@Field", request.FilterField ?? "");
                parameters.Add("@StartWith", request.FilterValue ?? "");
                parameters.Add("@Franchise", request.FranchiseCode ?? "");
                parameters.Add("@Username", request.Username);
            }

            var data = await connection.QueryAsync<dynamic>(spName, parameters, commandType: CommandType.StoredProcedure);
            var columns = new List<string>();
            if (data.Any())
            {
                var firstRow = (IDictionary<string, object>)data.First();
                columns = firstRow.Keys.ToList();
            }

            return new ReportDataResponse { Columns = columns, Data = data };
        }
    }

    public async Task<IEnumerable<FilterItem>> GetFranchisesAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<FilterItem>(
                "select FranchiseName as Name, FranchiseCode as Code from FranchiseMaster order by FranchiseName asc");
        }
    }
}
