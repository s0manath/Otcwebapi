using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OTC.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OTC.Api.Services
{
    public class RoleMasterService : IRoleMasterService
    {
        private readonly string _connectionString;

        public RoleMasterService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
           ?? throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
        }

        public async Task<List<RoleMaster>> GetRolesAsync(RoleSearchRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            var data = await connection.QueryAsync<RoleMaster>("sp_get_roles_search", new 
            { 
                RoleName = string.IsNullOrEmpty(request?.RoleName) ? (object)DBNull.Value : request.RoleName, 
                RoleStatus = request?.Status == -1 ? (object)DBNull.Value : request?.Status 
            }, commandType: CommandType.StoredProcedure);

            return data.ToList();
        }

        public async Task<RoleMaster?> GetRoleBySlNoAsync(long slNo)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_get_role_by_slno",
                new { SlNo = slNo },
                commandType: CommandType.StoredProcedure);

            if (result == null) return null;

            var role = new RoleMaster
            {
                SlNo = result.sl_no,
                RoleName = result.role_name,
                RoleDescription = result.role_description,
                RoleDepartment = result.role_department,
                RoleStatus = result.role_status,
                CoustodianNoneAvailable = result.CoustodianNoneAvailable,
                CreatedOn = result.created_on,
                CreatedBy = result.created_by,
                UpdatedOn = result.updated_on,
                UpdatedBy = result.updated_by
            };

            var modulesJson = result.role_modules_access as string;
            if (!string.IsNullOrEmpty(modulesJson))
            {
                role.Privileges = JsonSerializer.Deserialize<List<ModuleAccess>>(modulesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ModuleAccess>();
            }

            var reportsJson = result.role_reports_access as string;
            if (!string.IsNullOrEmpty(reportsJson))
            {
                role.ReportPrivileges = JsonSerializer.Deserialize<List<ReportAccess>>(reportsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ReportAccess>();
            }

            return role;
        }

        public async Task<string> SaveRoleAsync(RoleMaster role)
        {
            using var connection = new SqlConnection(_connectionString);

            string modulesJson = role.Privileges != null ? JsonSerializer.Serialize(role.Privileges) : null;
            string reportsJson = role.ReportPrivileges != null ? JsonSerializer.Serialize(role.ReportPrivileges) : null;

            if (role.SlNo == 0)
            {
                // Insert
                await connection.ExecuteAsync("sp_add_role_otc", new
                {
                    RoleName = role.RoleName,
                    RoleDescription = string.IsNullOrEmpty(role.RoleDescription) ? (object)DBNull.Value : role.RoleDescription,
                    RoleDepartment = role.RoleDepartment ?? (object)DBNull.Value,
                    RoleModulesAccess = string.IsNullOrEmpty(modulesJson) ? (object)DBNull.Value : modulesJson,
                    RoleStatus = role.RoleStatus,
                    CustNoneStatus = role.CoustodianNoneAvailable,
                    CreatedBy = role.CreatedBy ?? (object)DBNull.Value,
                    RoleReportAccess = string.IsNullOrEmpty(reportsJson) ? (object)DBNull.Value : reportsJson
                }, commandType: CommandType.StoredProcedure);
                return "Saved Successfully";
            }
            else
            {
                // Update
                await connection.ExecuteAsync("sp_update_role_master", new
                {
                    SlNo = role.SlNo,
                    RoleName = role.RoleName,
                    RoleDescription = string.IsNullOrEmpty(role.RoleDescription) ? (object)DBNull.Value : role.RoleDescription,
                    RoleDepartment = role.RoleDepartment ?? (object)DBNull.Value,
                    RoleModulesAccess = string.IsNullOrEmpty(modulesJson) ? (object)DBNull.Value : modulesJson,
                    RoleStatus = role.RoleStatus,
                    UpdatedBy = role.UpdatedBy ?? (object)DBNull.Value,
                    RoleReportAccess = string.IsNullOrEmpty(reportsJson) ? (object)DBNull.Value : reportsJson,
                    CustNoneStatus = role.CoustodianNoneAvailable
                }, commandType: CommandType.StoredProcedure);
                return "Updated Successfully";
            }
        }

        public async Task<List<string>> GetModuleListAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var data = await connection.QueryAsync<dynamic>("sp_get_module_master", commandType: CommandType.StoredProcedure);
            
            var modules = new List<string>();
            foreach(var item in data)
            {
                // The legacy module datatable usually has "ModuleName" or similar.
                // Assuming it has a column named "ModuleName"
                var dict = item as IDictionary<string, object>;
                if (dict.ContainsKey("ModuleName") && dict["ModuleName"] != null)
                {
                    modules.Add(dict["ModuleName"].ToString());
                }
            }
            return modules;
        }
    }
}
