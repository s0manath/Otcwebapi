using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OTC.Api.Models;

namespace OTC.Api.Services;

public class LoginMasterService : ILoginMasterService
{
    private readonly string _connectionString;

    public LoginMasterService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string 'DefaultConnection' not found.");
    }

    public async Task<List<LoginMasterListItem>> SearchLoginsAsync(LoginMasterSearchRequest request)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"select uname as Username, utype as UserType, role_master.role_name as Role, 
                    CAST(ISNULL(logmast.logflg, 0) AS BIT) as Locked 
                    from logmast 
                    inner join role_master with (Nolock) on logmast.rolecode=role_master.sl_no 
                    where 1=1 and (utype='Admin' or utype='Auditor' or utype='Cash Team')";

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            sql += " AND (uname LIKE @Term OR utype LIKE @Term OR role_master.role_name LIKE @Term)";
        }
        sql += " order by logmast.uname";

        var param = new { Term = $"%{request.SearchTerm}%" };
        var result = await connection.QueryAsync<LoginMasterListItem>(sql, param);
        return result.ToList();
    }

    public async Task<LoginMasterRequest?> GetLoginByIdAsync(string username)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "select uname as Username, fullname as FullName, rolecode as Role, email as Email, utype as UserType from logmast where uname = @username";
        var login = await connection.QueryFirstOrDefaultAsync<LoginMasterRequest>(sql, new { username });
        return login;
    }

    public Task<List<LoginMasterListItem>> SearchCustodianLoginsAsync(LoginMasterSearchRequest request, string userName)
    {
        return Task.FromResult(new List<LoginMasterListItem>());
    }

    public async Task<string> SaveLoginAsync(LoginMasterRequest request, string createdBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        
        bool exists = await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM logmast WHERE uname = @Username", new { request.Username });
        string spName = exists ? "Sp_Logmast" : "Proc_InsertUpdateLogmast";

        if (exists)
        {
            parameters.Add("@Username", request.Username);
            parameters.Add("@fullname", request.FullName);
            parameters.Add("@password", request.Password);
            parameters.Add("@Utype", request.UserType);
            parameters.Add("@rolecode", request.Role);
            parameters.Add("@glist", "");
            parameters.Add("@grp", "");
            parameters.Add("@createdby", createdBy);
            parameters.Add("@Regioncode", string.Join(",", request.SelectedRegions));
            parameters.Add("@Locationcode", "");
            parameters.Add("@BankCode", "");
            parameters.Add("@Module", "Update");
            parameters.Add("@IsMobileUser", "0");
            parameters.Add("@Email", request.Email);
            
            try
            {
                await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        else
        {
            parameters.Add("@Username", request.Username);
            parameters.Add("@fullname", request.FullName);
            parameters.Add("@password", request.Password);
            parameters.Add("@Utype", request.UserType);
            parameters.Add("@Ulist", request.Role ?? "");
            parameters.Add("@glist", "");
            parameters.Add("@grp", "");
            parameters.Add("@rolecode", request.Role);
            parameters.Add("@createdby", createdBy);
            parameters.Add("@Regioncode", string.Join(",", request.SelectedRegions));
            parameters.Add("@Locationcode", "");
            parameters.Add("@BankCode", "");
            parameters.Add("@Module", "Insert");
            parameters.Add("@IsMobileUser", "0");
            parameters.Add("@Email", request.Email);
            
            var userIdParam = new SqlParameter("@NewUserId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add("@NewUserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            try
            {
                await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    public async Task<string> LockLoginAsync(string username, string lockedBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "Update LogMast set lastmodifieddate=getdate(),logflg=1,LockedBy=@lockedBy,LockedDate=getdate() where Uname=@username";
        try
        {
            await connection.ExecuteAsync(sql, new { username, lockedBy });
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> UnlockLoginAsync(string username)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "Update LogMast set lastmodifieddate=getdate(),logflg=null,logattempt=null,Locked=null where Uname=@username";
        try
        {
            await connection.ExecuteAsync(sql, new { username });
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<List<HierarchyItem>> GetHierarchyAsync(string type, string? parentId = null)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        
        try
        {
            switch (type.ToLower())
            {
                case "region":
                    var regions = await connection.QueryAsync<dynamic>("select RoCode, RoName from ROMast order by RoName");
                    return regions.Select(x => new HierarchyItem { Id = x.RoCode?.ToString() ?? "", Name = x.RoName?.ToString() ?? "" }).ToList();
                
                case "state":
                    parameters.Add("@regionIDs", parentId ?? "");
                    var states = await connection.QueryAsync<dynamic>("Proc_GetStatesByRegion", parameters, commandType: CommandType.StoredProcedure);
                    return states.Select(x => new HierarchyItem { Id = x.slno?.ToString() ?? "", Name = x.state_name?.ToString() ?? "" }).ToList();
                
                case "district":
                    parameters.Add("@State", parentId ?? "");
                    var districts = await connection.QueryAsync<dynamic>("Proc_GetDistrictsByState", parameters, commandType: CommandType.StoredProcedure);
                    return districts.Select(x => new HierarchyItem { Id = x.district_id?.ToString() ?? "", Name = x.district_name?.ToString() ?? "" }).ToList();
                
                case "franchise":
                    parameters.Add("@district", parentId ?? "");
                    var franchises = await connection.QueryAsync<dynamic>("Proc_GetFranchiseByDistrict", parameters, commandType: CommandType.StoredProcedure);
                    return franchises.Select(x => new HierarchyItem { Id = x.FranchiseCode?.ToString() ?? "", Name = x.FranchiseName?.ToString() ?? "" }).ToList();
            }
        }
        catch
        {
            // Fallback for missing SPs
        }

        return new List<HierarchyItem>();
    }
}
