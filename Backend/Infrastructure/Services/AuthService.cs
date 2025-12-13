
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Configuration;
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

    public async Task<string>? LoginAsync(LoginDto loginUser)
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

    public async Task<string> CreateJwtToken(User user)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(ConfigurationHelper.GetConfigurationValue("Jwt:Expiration_Minutes")));

        var claims = new Claim[]
        {
            // new Claim(),
        };
        await Task.Delay(100);
        return "hello";
    }
}
