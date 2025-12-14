
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IDatabaseContext _databaseContext;
    public AuthService(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }


    public Task<string> RegisterAsync(RegisterDto user)
    {
        return _databaseContext.Hello<string>();
    }

    public async Task<AuthResponse>? LoginAsync(LoginDto loginUser)
    {
        User user = await _databaseContext.GetByConditionAsync();
        if (user is null)
        {
            return default!;
        }
        // implement password match logic...
        bool passWordMatch = true;
        if (!passWordMatch)
        {
            return default!;
        }
        var jwtToken = await CreateJwtToken(user);
        return jwtToken;
    }

    public async Task<AuthResponse> CreateJwtToken(User user)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(ConfigurationHelper.GetConfigurationValue("Jwt:Expiration_Minutes")));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Subject
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Issued At
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(ConfigurationHelper.GetConfigurationValue("Jwt:SecretKey")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: ConfigurationHelper.GetConfigurationValue("Jwt:Issuer"),
            audience: ConfigurationHelper.GetConfigurationValue("Jwt:Audience"),
            claims: claims,
            expires: expirationTime,
            signingCredentials: creds
        );

        var base64Token = new JwtSecurityTokenHandler().WriteToken(token);
        var response = new AuthResponse
        {
            Email = user.Email,
            Name = user.FullName,
            Token = base64Token
        };
        return response;
    }
}
