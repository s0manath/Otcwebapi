using OTC.Api.Models;

namespace OTC.Api.Services;

public class LoginMasterService : ILoginMasterService
{
    private static readonly List<LoginMasterListItem> _mockLogins = new()
    {
        new LoginMasterListItem { Username = "admin", UserType = "Admin", Role = "Super Admin", Locked = false },
        new LoginMasterListItem { Username = "user1", UserType = "User", Role = "Manager", Locked = true },
    };

    private static readonly List<HierarchyItem> _mockHierarchy = new()
    {
        new HierarchyItem { Id = "R1", Name = "North Region", ParentId = null },
        new HierarchyItem { Id = "R2", Name = "South Region", ParentId = null },
        new HierarchyItem { Id = "S1", Name = "Delhi", ParentId = "R1" },
        new HierarchyItem { Id = "S2", Name = "Punjab", ParentId = "R1" },
        new HierarchyItem { Id = "S3", Name = "Tamil Nadu", ParentId = "R2" },
        new HierarchyItem { Id = "S4", Name = "Kerala", ParentId = "R2" },
        new HierarchyItem { Id = "D1", Name = "New Delhi", ParentId = "S1" },
        new HierarchyItem { Id = "D2", Name = "Ludhiana", ParentId = "S2" },
        new HierarchyItem { Id = "D3", Name = "Chennai", ParentId = "S3" },
        new HierarchyItem { Id = "F1", Name = "Franchise 1", ParentId = "D1" },
        new HierarchyItem { Id = "F2", Name = "Franchise 2", ParentId = "D3" },
    };

    public Task<List<LoginMasterListItem>> SearchLoginsAsync(LoginMasterSearchRequest request)
    {
        var result = _mockLogins.AsQueryable();

        if (!string.IsNullOrEmpty(request.Field) && !string.IsNullOrEmpty(request.StartWith))
        {
            if (request.Field == "Uname")
                result = result.Where(x => x.Username.StartsWith(request.StartWith, StringComparison.OrdinalIgnoreCase));
            else if (request.Field == "Utype")
                result = result.Where(x => x.UserType.StartsWith(request.StartWith, StringComparison.OrdinalIgnoreCase));
        }

        if (request.LockedUser)
            result = result.Where(x => x.Locked);

        return Task.FromResult(result.ToList());
    }

    public Task<LoginMasterRequest?> GetLoginByIdAsync(string username)
    {
        var login = _mockLogins.FirstOrDefault(x => x.Username == username);
        if (login == null) return Task.FromResult<LoginMasterRequest?>(null);

        return Task.FromResult<LoginMasterRequest?>(new LoginMasterRequest
        {
            Username = login.Username,
            UserType = login.UserType,
            Role = login.Role,
            FullName = "Mock Full Name",
            Email = "mock@example.com"
        });
    }

    public Task<bool> SaveLoginAsync(LoginMasterRequest request)
    {
        // For now, just return true as it's a mock
        return Task.FromResult(true);
    }

    public Task<List<HierarchyItem>> GetHierarchyAsync(string type, string? parentId = null)
    {
        IEnumerable<HierarchyItem> result = _mockHierarchy;

        switch (type.ToLower())
        {
            case "region":
                result = result.Where(x => x.ParentId == null);
                break;
            case "state":
                if (!string.IsNullOrEmpty(parentId))
                {
                    var parentIds = parentId.Split(',');
                    result = result.Where(x => parentIds.Contains(x.ParentId));
                }
                break;
            case "district":
                if (!string.IsNullOrEmpty(parentId))
                {
                    var parentIds = parentId.Split(',');
                    result = result.Where(x => parentIds.Contains(x.ParentId));
                }
                break;
            case "franchise":
                if (!string.IsNullOrEmpty(parentId))
                {
                    var parentIds = parentId.Split(',');
                    result = result.Where(x => parentIds.Contains(x.ParentId));
                }
                break;
        }

        return Task.FromResult(result.ToList());
    }
}
