using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly int _jwtExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;
    private readonly DatabaseContext _databaseContext;

    public JwtTokenService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        _jwtExpirationMinutes = Convert.ToInt32(ConfigurationHelper.GetConfigurationValue("Jwt:Expiration_Minutes"));
        _refreshTokenExpirationDays = Convert.ToInt32(ConfigurationHelper.GetConfigurationValue("Jwt:RefreshTokenValidityInDays"));
    }

    public async Task<RefreshTokenResponse> GenerateTokenResponseAsync(User user)
    {
        var accessToken = await CreateJwtToken(user);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = ComputeTokenHash(Guid.NewGuid().ToString()),
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };
        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.TokenHash,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            RefreshTokenExpiry = refreshToken.ExpiresAt
        };
    }

    private async Task<string> CreateJwtToken(User user)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),               // USER ID
            new Claim(ClaimTypes.NameIdentifier, user.Id),                 // USER ID (ASP.NET)
            new Claim(JwtRegisteredClaimNames.Email, user.Email),          // EMAIL
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Permissions.Select(permission => new Claim("Permission", permission)));

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

    private string ComputeTokenHash(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}
