using OTC.Api.Models;

namespace OTC.Api.Services;

public class LoginService : ILoginService
{
    public async Task<LoginResponse> ValidateLoginAsync(LoginRequest request)
    {
        // Mocking login for now, will integrate with legacy logic/db later
        if (request.Username == "admin" && request.Password == "admin123")
        {
            return new LoginResponse
            {
                Success = true,
                Message = "Login Successful",
                Token = "mock-jwt-token",
                Username = request.Username
            };
        }

        return new LoginResponse
        {
            Success = false,
            Message = "Invalid username or password"
        };
    }
}
