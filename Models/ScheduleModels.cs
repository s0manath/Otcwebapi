namespace OTC.Api.Models;

public class ScheduleListItem
{
    public string ScheduleId { get; set; } = string.Empty;
    public string AtmId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string ScheduleDate { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public class ScheduleInsertRequest
{
    public string? AtmId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string ScheduleDate { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
    public string? BulkScheduleInfo { get; set; } // For legacy multi-insert pattern
}

public class ScheduleUpdateRequest
{
    public string ScheduleId { get; set; } = string.Empty;
    public string AtmId { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string ScheduleDate { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
    public string Comment { get; set; } = string.Empty;
}

public class ActivityType
{
    public string Name { get; set; } = string.Empty;
}

public class ScheduleListRequest
{
    public string FromDate { get; set; } = string.Empty;
    public string ToDate { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
    public string? SearchField { get; set; }
    public string? StartWith { get; set; }
}

public class ScheduleDeleteRequest
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
}
