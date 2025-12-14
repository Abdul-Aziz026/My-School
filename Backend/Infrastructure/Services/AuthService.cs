using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Configuration;
using Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IDatabaseContext _databaseContext;
    public AuthService(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerUser">including user name, email, and password.</param>
    /// <returns>A JWT token as a string if registration is successful; otherwise, null</returns>
    public async Task<string> RegisterAsync(RegisterDto registerUser)
    {
        Expression <Func<User, bool>> condition = u => u.Email == registerUser.Email;
        var user = await _databaseContext.GetItemByConditionAsync<User>(condition);
        if (user is not null)
        {
            return default;
        }
        // Hash the password with BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);
        var newUser = new User()
        {
            UserName = registerUser.UserName,
            Email = registerUser.Email,
            PasswordHash = passwordHash,
        };
        await _databaseContext.AddAsync<User>(newUser);
        var token = await CreateJwtToken(newUser);
        return token;
    }

    /// <summary>
    /// User login method
    /// </summary>
    /// <param name="loginUser"></param>
    /// <returns>A JWT token as a string if login is successful; otherwise, null</returns>
    public async Task<string?> LoginAsync(LoginDto loginUser)
    {
        Expression<Func<User, bool>> filter = o => true;
        filter = filter.And(u => u.Email == loginUser.Email);
        var user = await _databaseContext.GetItemByConditionAsync<User>(filter);
        if (user is null)
        {
            return default!;
        }
        // implement password match logic...
        if (user.Email != loginUser.Email)
        {
            return default;
        }
        // Verify password using BCrypt
        var passwordMatches = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.PasswordHash);
        if (!passwordMatches)
        {
            return default!;
        }
        var jwtToken = await CreateJwtToken(user);
        return jwtToken;
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the JWT will be created.</param>
    /// <returns>A JWT Token</returns>
    public async Task<string> CreateJwtToken(User user)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(ConfigurationHelper.GetConfigurationValue("Jwt:Expiration_Minutes")));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Subject
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Issued At
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
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
        return base64Token;
    }
}
