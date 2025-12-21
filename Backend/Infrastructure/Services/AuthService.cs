using Application.DTOs;
using Application.Helper;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IDatabaseContext _databaseContext;
    private readonly int _jwtExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public AuthService(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        _jwtExpirationMinutes = Convert.ToInt32(ConfigurationHelper.GetConfigurationValue("Jwt:Expiration_Minutes"));
        _refreshTokenExpirationDays = Convert.ToInt32(ConfigurationHelper.GetConfigurationValue("Jwt:RefreshTokenValidityInDays"));
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerUser">including user name, email, and password.</param>
    /// <returns>A JWT token as a string if registration is successful; otherwise, null</returns>
    public async Task<RefreshTokenResponse> RegisterAsync(RegisterDto registerUser)
    {
        Expression <Func<User, bool>> condition = u => u.Email == registerUser.Email;
        //var user = await _databaseContext.GetItemByConditionAsync<User>(condition);
        var user = await _databaseContext.GetItemByConditionAsync<User>(u => u.Email == registerUser.Email);

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
            CreatedAt = DateTime.UtcNow
        };

        await _databaseContext.AddAsync(newUser);

        return await GenerateTokenResponse(newUser, TellMe.IpAddress);
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
        var user = await _databaseContext.GetItemByConditionAsync<User>(u => u.Email == loginUser.Email);
        if (user is null)
        {
            return null!;
        }
        var passwordMatches = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.PasswordHash);

        if (!passwordMatches)
        {
            return null;
        }

        return await GenerateTokenResponse(user, TellMe.IpAddress);
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
        var refreshToken = await _databaseContext.GetItemByConditionAsync<RefreshToken>(o => o.TokenHash == tokenHash);
        if (refreshToken is null)
        {
            throw new UnauthorizedAccessException("Invalid or Expired access token...");
        }
        if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or Expired access token...");
        }
        var user = await _databaseContext.GetItemByConditionAsync<User>(u => u.Id == refreshToken.UserId);
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        // revoke the old refresh token...
        refreshToken.IsRevoked = true;
        await _databaseContext.UpdateAsync(refreshToken);
        // Generate new tokens
        return await GenerateTokenResponse(user, TellMe.IpAddress);
    }

    private async Task<RefreshTokenResponse> GenerateTokenResponse(User user, string ipAddress)
    {
        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

        var accessToken = await CreateJwtToken(user);
        var refreshToken = await GenerateAndStoreRefreshToken(user.Id, ipAddress, refreshTokenExpiry);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = accessTokenExpiry,
            RefreshTokenExpiry = refreshTokenExpiry
        };
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the JWT will be created.</param>
    /// <returns>A JWT Token</returns>
    private async Task<string> CreateJwtToken(User user)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Subject
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
            new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ), // Issued At
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.GetConfigurationValue("Jwt:SecretKey")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: ConfigurationHelper.GetConfigurationValue("Jwt:Issuer"),
            audience: ConfigurationHelper.GetConfigurationValue("Jwt:Audience"),
            claims: claims,
            expires: expirationTime,
            signingCredentials: creds
        );

        var base64Token = new JwtSecurityTokenHandler().WriteToken(token);
        return base64Token;
    }

    private async Task<string> GenerateAndStoreRefreshToken(string userId, string ipAddress, DateTime expiresAt)
    {
        // Delete old refresh tokens for this user
        var oldTokens = await _databaseContext.GetItemsByConditionAsync<RefreshToken>(rt => rt.UserId == userId);
        if (oldTokens is not null && oldTokens.Any())
        {
            await _databaseContext.DeleteManyAsync(oldTokens);
        }

        // Generate new token
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenHash = ComputeTokenHash(token);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedByIp = ipAddress,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _databaseContext.AddAsync(refreshToken);

        return token;
    }


    /// <summary>
    /// Revoked the refresh token
    /// </summary>
    /// <param name="refreshToken">old refresh token</param>
    /// <returns></returns>
    private async Task RevokeRefreshTokenAsync(string token)
    {
        var tokenHash = ComputeTokenHash(token);
        var refreshToken = await _databaseContext.GetItemByConditionAsync<RefreshToken>(rt => rt.TokenHash == tokenHash);

        if (refreshToken is not null && !refreshToken.IsRevoked)
        {
            refreshToken.IsRevoked = true;
            await _databaseContext.UpdateAsync<RefreshToken>(refreshToken);
        }
    }

    Task IAuthService.RevokeRefreshTokenAsync(string token)
    {
        return RevokeRefreshTokenAsync(token);
    }

    /// <summary>
    /// Create new Refresh token and Save in Database
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="ipAddress">client device ip address</param>
    /// <returns>refresh token</returns>
    private async Task<string> GenerateRefreshToken(string userId, string ipAddress)
    {
        var response = await _databaseContext.GetItemsByConditionAsync<RefreshToken>(o => o.Id == userId);
        if (response is not null)
        {
            await _databaseContext.DeleteManyAsync<RefreshToken>(response);
        }
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var refreshToken = new RefreshToken
        {
            TokenHash = ComputeTokenHash(token),
            Id = userId,
            CreatedByIp = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        await _databaseContext.AddAsync<RefreshToken>(refreshToken);
        return token;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns>hashed token</returns>
    private string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }

    public async Task<UserInfo?> GetUserInfoAsync(string userId)
    {
        var user = await _databaseContext.GetItemByConditionAsync<User>(u => u.Id == userId);

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

    
}
