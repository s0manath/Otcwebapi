namespace OTC.Api.Models;

public class LoginMasterRequest
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<string> SelectedRegions { get; set; } = new();
    public List<string> SelectedStates { get; set; } = new();
    public List<string> SelectedDistricts { get; set; } = new();
    public List<string> SelectedFranchises { get; set; } = new();
}

public class LoginMasterListItem
{
    public string Username { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Locked { get; set; }
}

public class HierarchyItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ParentId { get; set; }
}

public class LoginMasterSearchRequest
{
    public string? SearchTerm { get; set; }
}

public class HierarchyRequest
{
    public string Type { get; set; } = string.Empty;
    public string? ParentId { get; set; }
}
