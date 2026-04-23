namespace OTC.Api.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string LogId { get; set; } = string.Empty;
    public string SessionLogId { get; set; } = string.Empty;
    public string PID { get; set; } = string.Empty;
    public int InvalidCount { get; set; }
    public string Password { get; set; } = string.Empty;
    public string IpAddress {  get; set; } = string.Empty;
}

public class EncryptedLoginRequest
{
    public string EncryptedPayload { get; set; } = string.Empty;
}
