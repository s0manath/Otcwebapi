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
        
        if (login != null)
        {
            try
            {
                int? userId = await connection.ExecuteScalarAsync<int?>("Proc_GetUserIdByUsername", new { Username = username }, commandType: CommandType.StoredProcedure);
                
                if (userId.HasValue)
                {
                    var regions = await connection.QueryAsync<dynamic>("Proc_GetUserRegionsAccessByUser", new { UserId = userId.Value }, commandType: CommandType.StoredProcedure);
                    login.SelectedRegions = regions.Select(r => ((IDictionary<string, object>)r).Values.First()?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

                    var states = await connection.QueryAsync<dynamic>("Proc_GetUserStatesAccessByUser", new { UserId = userId.Value }, commandType: CommandType.StoredProcedure);
                    login.SelectedStates = states.Select(s => ((IDictionary<string, object>)r).Values.First()?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

                    var districts = await connection.QueryAsync<dynamic>("Proc_GetUserDistrictsAccessByUser", new { UserId = userId.Value }, commandType: CommandType.StoredProcedure);
                    login.SelectedDistricts = districts.Select(d => ((IDictionary<string, object>)r).Values.First()?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

                    var franchises = await connection.QueryAsync<dynamic>("Proc_GetUserFranchisesAccessByUser", new { UserId = userId.Value }, commandType: CommandType.StoredProcedure);
                    login.SelectedFranchises = franchises.Select(f => ((IDictionary<string, object>)r).Values.First()?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();
                }
            }
            catch { /* Ignore if SPs fail */ }
        }
        
        return login;
    }

    public Task<List<LoginMasterListItem>> SearchCustodianLoginsAsync(LoginMasterSearchRequest request, string userName)
    {
        return Task.FromResult(new List<LoginMasterListItem>());
    }

    public async Task<string> SaveLoginAsync(LoginMasterRequest request, string createdBy)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            bool exists = await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM logmast WHERE uname = @Username", new { request.Username }, transaction);
            
            var parameters = new DynamicParameters();
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
            parameters.Add("@Module", exists ? "Update" : "Insert");
            parameters.Add("@IsMobileUser", "0");
            parameters.Add("@Email", request.Email);
            
            parameters.Add("@NewUserId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            
            await connection.ExecuteAsync("Proc_InsertUpdateLogmast", parameters, transaction, commandType: CommandType.StoredProcedure);
            
            int newUserId;
            if (exists)
            {
                newUserId = await connection.ExecuteScalarAsync<int>("Proc_GetUserIdByUsername", new { Username = request.Username }, transaction, commandType: CommandType.StoredProcedure);
                
                await connection.ExecuteAsync("DELETE FROM UserRegion_Access WHERE Userid = @userId", new { userId = newUserId }, transaction);
                await connection.ExecuteAsync("DELETE FROM UserState_Access WHERE Userid = @userId", new { userId = newUserId }, transaction);
                await connection.ExecuteAsync("DELETE FROM UserDistrict_Access WHERE Userid = @userId", new { userId = newUserId }, transaction);
                await connection.ExecuteAsync("DELETE FROM UserFranchise_Access WHERE Userid = @userId", new { userId = newUserId }, transaction);
            }
            else
            {
                newUserId = parameters.Get<int>("@NewUserId");
            }
            
            foreach(var region in request.SelectedRegions)
            {
                await connection.ExecuteAsync("Proc_Insert_UserRegion_Access", new { userId = newUserId, roCode = region }, transaction, commandType: CommandType.StoredProcedure);
            }
            foreach(var state in request.SelectedStates)
            {
                await connection.ExecuteAsync("Insert_UserState_Access", new { userId = newUserId, stateId = state }, transaction, commandType: CommandType.StoredProcedure);
            }
            foreach(var district in request.SelectedDistricts)
            {
                await connection.ExecuteAsync("Insert_UserDistrict_Access", new { userId = newUserId, districtId = district }, transaction, commandType: CommandType.StoredProcedure);
            }
            foreach(var franchise in request.SelectedFranchises)
            {
                await connection.ExecuteAsync("Insert_UserFranchise_Access", new { userId = newUserId, franchiseeCode = franchise }, transaction, commandType: CommandType.StoredProcedure);
            }
            
            transaction.Commit();
            return string.Empty;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return ex.Message;
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
