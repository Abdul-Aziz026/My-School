// Application/Interfaces/IAuthService.cs
using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<RefreshTokenResponse> RegisterAsync(RegisterDto registerUser);
    Task<RefreshTokenResponse?> LoginAsync(LoginDto loginUser);
    Task<RefreshTokenResponse> RefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token);
    Task<UserInfo?> GetUserInfoAsync(string userId);
}