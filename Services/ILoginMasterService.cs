using OTC.Api.Models;

namespace OTC.Api.Services;

public interface ILoginMasterService
{
    Task<List<LoginMasterListItem>> SearchLoginsAsync(LoginMasterSearchRequest request);
    Task<LoginMasterRequest?> GetLoginByIdAsync(string username);
    Task<bool> SaveLoginAsync(LoginMasterRequest request);
    Task<List<HierarchyItem>> GetHierarchyAsync(string type, string? parentId = null);
}
