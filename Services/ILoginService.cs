using OTC.Api.Models;

namespace OTC.Api.Services;

public interface ILoginService
{
    Task<LoginResponse> ValidateLoginAsync(EncryptedLoginRequest request, string ipAddress);
}
