using Travel.DTOs;

namespace Travel.Modules.Auth;

public interface IAuthService
{
    Task<(bool Success, string Message, UserResponseDto? Data)> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string Message, UserResponseDto? Data)> LoginAsync(LoginDto dto);
}