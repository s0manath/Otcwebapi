namespace OTC.Api.Models;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string LogId { get; set; } = string.Empty;
    public string SessionLogId { get; set; } = string.Empty;
    public string PID { get; set; } = string.Empty;
    public int InvalidCount { get; set; }
}
