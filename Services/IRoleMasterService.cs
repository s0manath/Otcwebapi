using System.Collections.Generic;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IRoleMasterService
    {
        Task<List<RoleMaster>> GetRolesAsync(RoleSearchRequest request);
        Task<RoleMaster?> GetRoleByIdAsync(long id);
        Task<string> SaveRoleAsync(RoleMaster role);
        Task<List<string>> GetModuleListAsync();
    }
}
