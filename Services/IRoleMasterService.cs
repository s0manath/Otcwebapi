using System.Collections.Generic;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IRoleMasterService
    {
        Task<List<RoleMaster>> GetRolesAsync();
        Task<RoleMaster?> GetRoleByIdAsync(string name);
        Task<string> SaveRoleAsync(RoleMaster role);
        Task<List<string>> GetModuleListAsync();
    }
}
