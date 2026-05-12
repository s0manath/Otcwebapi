using OTC.Api.Models;
using System.Collections;

namespace OTC.Api.Services;

public interface ILoginMasterService
{
    Task<List<LoginMasterListItem>> SearchLoginsAsync(LoginMasterSearchRequest request);
    Task<List<LoginMasterListItem>> SearchCustodianLoginsAsync(LoginMasterSearchRequest request, string userName);
    Task<LoginMasterRequest?> GetLoginByIdAsync(string username);
    Task<string> SaveLoginAsync(LoginMasterRequest request, string createdBy);
    Task<string> LockLoginAsync(string username, string lockedBy);
    Task<string> UnlockLoginAsync(string username);
    Task<List<HierarchyItem>> GetHierarchyAsync(string type, string? parentId = null);
    Task <IEnumerable<UserRole>> GetUserRole();
    Task<IEnumerable<UserType>> GetUserType();
}
