namespace OTC.Api.Models;

public class RouteListItem
{
    //public string Id { get; set; } = string.Empty;
    public string ScheduleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string AtmId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string ScheduleDate { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public string FranchiseName { get; set; } = string.Empty;
    public string ZomName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RouteKey { get; set; } = string.Empty;
    public string Custodian1 { get; set; } = string.Empty;
    public string Custodian2 { get; set; } = string.Empty;
    public string CroType { get; set; } = string.Empty;
}

public class RouteSaveRequest
{
    public string ScheduleId { get; set; } = string.Empty;
    public string RouteConfigId { get; set; } = string.Empty;
    public string AtmId { get; set; } = string.Empty;
    public string RouteKey { get; set; } = string.Empty;
    public string Custodian1 { get; set; } = string.Empty;
    public string Custodian2 { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
    public bool UpdateAll { get; set; } = false;
}

public class RouteFilterOptions
{
    public List<FilterItem> Regions { get; set; } = new();
    public List<FilterItem> Districts { get; set; } = new();
    public List<FilterItem> Franchises { get; set; } = new();
    public List<FilterItem> Zoms { get; set; } = new();
    public List<string> ActivityTypes { get; set; } = new();
}

public class FilterItem
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class CustodianListItem
{
    public string CustodianName { get; set; } = string.Empty;
    public string CustodianCode { get; set; } = string.Empty;
}

public class RouteListRequest
{
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string Username { get; set; } = "admin";
    public string? Region { get; set; }
    public string? District { get; set; }
    public string? Franchise { get; set; }
    public string? Zom { get; set; }
    public string? ActivityType { get; set; }
    public string? Status { get; set; }
    public string? ChkConfig { get; set; }
    public string? SearchField { get; set; }
    public string? Criteria { get; set; }
    public string? SearchValue { get; set; }
}
