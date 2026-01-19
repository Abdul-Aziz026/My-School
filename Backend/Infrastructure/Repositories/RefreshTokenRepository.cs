using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository(IDatabaseContext Context) : IRefreshTokenRepository
{
    public async Task AddAsync(UserRefreshToken token)
    {
        await Context.AddAsync<UserRefreshToken>(token);
    }

    public async Task DeleteManyAsync(IEnumerable<UserRefreshToken> tokens)
    {
        await Context.DeleteManyAsync<UserRefreshToken>(tokens);
    }

    public async Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await Context.GetItemByConditionAsync<UserRefreshToken>(rt => rt.RefreshTokenHash == tokenHash);
    }

    public async Task UpdateAsync(UserRefreshToken token)
    {
        await Context.UpdateAsync<UserRefreshToken>(token);
    }
}
