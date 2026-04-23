using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OTC.Api.Models;
using OTC.Api.Utility;
using System.Data;
using System.Text.Json;

namespace OTC.Api.Services;

public class LoginService : ILoginService
{
    private readonly string _connectionString;
    private readonly string _appName;

    public LoginService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new Exception("Connection string not found");
        _appName = configuration["AppName"] ?? "OTC_APPLICATION";
    }

    public async Task<LoginResponse> ValidateLoginAsync(EncryptedLoginRequest request, string ipAddress)
    {
        try
        {
            // 1. Decrypt the payload from UI
            var decryptedJson = PayloadEncryption.Decrypt(request.EncryptedPayload);
            var loginData = JsonSerializer.Deserialize<LoginRequest>(decryptedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (loginData == null || string.IsNullOrEmpty(loginData.Username) || string.IsNullOrEmpty(loginData.Password))
            {
                return new LoginResponse { Success = false, Message = "Invalid login payload" };
            }

            // 2. Encrypt password using legacy PRI key
            string legacyEncryptedPassword = AesEncryption.Encrypt(loginData.Password);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // 3. Get App ID (Legacy logic)
            var appParams = new DynamicParameters();
            appParams.Add("@appName", _appName);
            appParams.Add("@appId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("TempGetAppID", appParams, commandType: CommandType.StoredProcedure);
            int appIdValue = appParams.Get<int>("@appId");
            string appIdHex = appIdValue.ToString("X");

            // 4. Validate Login via Sp_chklogin_new
            string sessionLogId = Guid.NewGuid().ToString().Substring(0, 8) + appIdHex; // Simplified session ID generation

            var loginParams = new DynamicParameters();
            loginParams.Add("@uname", loginData.Username);
            loginParams.Add("@pass", legacyEncryptedPassword);
            loginParams.Add("@Sessionlogid", sessionLogId);
            loginParams.Add("@ipAddress", ipAddress);

            loginParams.Add("@chk", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            loginParams.Add("@ulist", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            loginParams.Add("@utype", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            loginParams.Add("@userfname", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            loginParams.Add("@PID", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            loginParams.Add("@rolecode", dbType: DbType.String, size: 3, direction: ParameterDirection.Output);
            loginParams.Add("@logid", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            loginParams.Add("@invalid_cnt", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("Sp_chklogin_new", loginParams, commandType: CommandType.StoredProcedure);

            string chkResult = loginParams.Get<string>("@chk")?.ToLower().Trim() ?? "";
            int invalidCount = loginParams.Get<int>("@invalid_cnt");

            if (chkResult == "available")
            {
                string pid = loginParams.Get<string>("@PID") ?? "";
                string logId = loginParams.Get<string>("@logid") ?? "";
                string roleCode = loginParams.Get<string>("@rolecode") ?? "";
                string fullName = loginParams.Get<string>("@userfname") ?? "";

                if (string.IsNullOrEmpty(pid))
                {
                    // 5. Add Login Record if no active PID
                    var addLoginParams = new DynamicParameters();
                    addLoginParams.Add("@ipadd", ipAddress);
                    addLoginParams.Add("@logid", logId);
                    addLoginParams.Add("@Sessionlogid", sessionLogId);
                    addLoginParams.Add("@username", loginData.Username);
                    addLoginParams.Add("@uRights", roleCode);
                    addLoginParams.Add("@dup", "No");

                    await connection.ExecuteAsync("addlogin", addLoginParams, commandType: CommandType.StoredProcedure);
                }

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login Successful",
                    Token = "dummy-jwt-for-now", // Implement proper JWT if required
                    Username = loginData.Username,
                    FullName = fullName,
                    RoleCode = roleCode,
                    LogId = logId,
                    SessionLogId = sessionLogId,
                    PID = pid
                };
            }
            else
            {
                string errorMessage = chkResult switch
                {
                    "blocked" => "User Blocked. Please contact Administrator.",
                    "invalidpass" => "Invalid Username or Password.",
                    "expired" => "Password Expired.",
                    "create new" => "Please create a new password.",
                    "invalidlogin" => "Invalid Login.",
                    _ => "Invalid Username or Password."
                };

                return new LoginResponse
                {
                    Success = false,
                    Message = errorMessage,
                    InvalidCount = invalidCount
                };
            }
        }
        catch (Exception ex)
        {
            return new LoginResponse { Success = false, Message = "Login Error: " + ex.Message };
        }
    }
}
