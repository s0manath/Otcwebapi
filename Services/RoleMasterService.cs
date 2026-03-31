using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public class RoleMasterService : IRoleMasterService
    {
        private static readonly List<RoleMaster> _mockRoles = new()
        {
            new RoleMaster 
            { 
                SlNo = 1, 
                RoleName = "Super Admin", 
                RoleDescription = "Full access to all modules", 
                RoleStatus = 1,
                Privileges = new List<ModuleAccess>
                {
                    new ModuleAccess { ModuleName = "ATM Master", View = true, Add = true, Edit = true, Delete = true },
                    new ModuleAccess { ModuleName = "Custodian Master", View = true, Add = true, Edit = true, Delete = true },
                    new ModuleAccess { ModuleName = "Franchise Master", View = true, Add = true, Edit = true, Delete = true },
                    new ModuleAccess { ModuleName = "Bank Master", View = true, Add = true, Edit = true, Delete = true },
                    new ModuleAccess { ModuleName = "Role Master", View = true, Add = true, Edit = true, Delete = true }
                }
            },
            new RoleMaster 
            { 
                SlNo = 2, 
                RoleName = "Regional Manager", 
                RoleDescription = "Manage regional operations", 
                RoleStatus = 1,
                Privileges = new List<ModuleAccess>
                {
                    new ModuleAccess { ModuleName = "ATM Master", View = true, Add = false, Edit = true, Delete = false },
                    new ModuleAccess { ModuleName = "Custodian Master", View = true, Add = true, Edit = true, Delete = false }
                }
            }
        };

        private static readonly List<string> _modules = new()
        {
            "ATM Master",
            "Custodian Master",
            "Franchise Master",
            "Bank Master",
            "Login Master",
            "Role Master",
            "Route Configure",
            "Location Master",
            "Region Master",
            "Key Inventory"
        };

        public Task<List<RoleMaster>> GetRolesAsync(RoleSearchRequest request)
        {
            var result = _mockRoles.AsQueryable();

            if (!string.IsNullOrEmpty(request.RoleName))
            {
                result = result.Where(x => x.RoleName.Contains(request.RoleName, StringComparison.OrdinalIgnoreCase));
            }

            if (request.Status.HasValue && request.Status.Value != -1)
            {
                result = result.Where(x => x.RoleStatus == request.Status.Value);
            }

            return Task.FromResult(result.ToList());
        }

        public Task<RoleMaster?> GetRoleByIdAsync(long id)
        {
            var role = _mockRoles.FirstOrDefault(x => x.SlNo == id);
            return Task.FromResult<RoleMaster?>(role);
        }

        public Task<string> SaveRoleAsync(RoleMaster role)
        {
            if (role.SlNo == 0)
            {
                role.SlNo = _mockRoles.Any() ? _mockRoles.Max(x => x.SlNo) + 1 : 1;
                role.CreatedOn = DateTime.Now;
                _mockRoles.Add(role);
            }
            else
            {
                var existing = _mockRoles.FirstOrDefault(x => x.SlNo == role.SlNo);
                if (existing != null)
                {
                    existing.RoleName = role.RoleName;
                    existing.RoleDescription = role.RoleDescription;
                    existing.RoleStatus = role.RoleStatus;
                    existing.Privileges = role.Privileges;
                    existing.ReportPrivileges = role.ReportPrivileges;
                }
            }
            return Task.FromResult("Saved Successfully");
        }

        public Task<List<string>> GetModuleListAsync()
        {
            return Task.FromResult(_modules);
        }
    }
}
