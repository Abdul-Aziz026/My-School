using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository,
                       IRefreshTokenRepository refreshTokenRepository,
                       IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }


    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerUser">including user name, email, and password.</param>
    /// <returns>A JWT token as a string if registration is successful; otherwise, null</returns>
    public async Task<RefreshTokenResponse> RegisterAsync(RegisterDto registerUser)
    {
        var user = await _userRepository.GetByEmailAsync(registerUser.Email);
        if (user is not null)
        {
            return default!;
        }
        // Hash the password with BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);
        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = registerUser.UserName,
            Email = registerUser.Email,
            PasswordHash = passwordHash,
            Roles = new List<string> { "User" }, // Default role
            Permissions = new List<string> { "ViewProduct" }, // Default permission
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(newUser);
        return await _jwtTokenService.GenerateTokenResponseAsync(newUser);
    }


    /// <summary>
    /// User login method
    /// </summary>
    /// <param name="loginUser"></param>
    /// <returns>A JWT token as a string if login is successful; otherwise, null</returns>
    public async Task<RefreshTokenResponse?> LoginAsync(LoginDto loginUser)
    {
        //Expression<Func<User, bool>> filter = o => true;
        //filter = filter.And(u => u.Email == loginUser.Email);
        var user = await _userRepository.GetByEmailAsync(loginUser.Email);
        if (user is null)
        {
            return null!;
        }
        var passwordMatches = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.PasswordHash);
        if (!passwordMatches)
        {
            return null;
        }
        return await _jwtTokenService.GenerateTokenResponseAsync(user);
    }

    /// <summary>
    /// Using old refresh token for new access token and refresh token
    /// </summary>
    /// <param name="token">old refresh token</param>
    /// <returns>new Access Token and new Refresh Token</returns>
    /// <exception cref="UnauthorizedAccessException">Invalid, expired or revoked refresh token</exception>
    public async Task<RefreshTokenResponse> RefreshTokenAsync(string token)
    {
        var tokenHash = ComputeTokenHash(token);
        var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
        if (refreshToken is null)
        {
            throw new UnauthorizedAccessException("Invalid or Expired access token...");
        }
        if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or Expired access token...");
        }
        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        // revoke the old refresh token...
        refreshToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
        // Generate new tokens
        return await _jwtTokenService.GenerateTokenResponseAsync(user);
    }

    public async Task<UserInfo?> GetUserInfoAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return null;
        }
        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            Roles = user.Roles,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = ComputeTokenHash(refreshToken);
        var refreshTokenResponse = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (refreshTokenResponse is not null && !refreshTokenResponse.IsRevoked)
        {
            refreshTokenResponse.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(refreshTokenResponse);
        }
    }

    private string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}
