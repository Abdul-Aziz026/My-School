using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Repositories;

public class UserRepository : Repository, IUserRepository
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public UserRepository(DatabaseContext dbContext, IRefreshTokenRepository repo) : base(dbContext) 
    {
        _refreshTokenRepository = repo;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await DbContext.GetItemByConditionAsync<User>(u => u.Email == email);
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
