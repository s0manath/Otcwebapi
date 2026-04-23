namespace OTC.Api.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string IpAddress {  get; set; } = string.Empty;
}

public class encryptedLoginRequest
{
    public string encryptedLoginReq{  get; set; }= string.Empty;
}
