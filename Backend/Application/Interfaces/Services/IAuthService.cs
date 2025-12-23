using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<RefreshTokenResponse> RegisterAsync(RegisterDto registerUser);
    Task<RefreshTokenResponse?> LoginAsync(LoginDto loginUser);
    Task<RefreshTokenResponse> RefreshTokenAsync(string token);
    Task<UserInfo?> GetUserInfoAsync(string userId);
    Task RevokeRefreshTokenAsync(string refreshToken);
}