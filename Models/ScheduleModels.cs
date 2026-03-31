namespace OTC.Api.Models;

public class ScheduleListItem
{
    public string Schedule_Id { get; set; } = string.Empty;
    public string ATMID { get; set; } = string.Empty;
    public string Activity_Type { get; set; } = string.Empty;
    public string Schedule_Date { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public class ScheduleInsertRequest
{
    public string? ATMID { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string ScheduleDate { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
    public string? BulkScheduleInfo { get; set; } // For legacy multi-insert pattern
}

public class ScheduleUpdateRequest
{
    public string ScheduleId { get; set; } = string.Empty;
    public string ATMID { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string ScheduleDate { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
    public string Comment { get; set; } = string.Empty;
}

public class ActivityType
{
    public string Name { get; set; } = string.Empty;
}
