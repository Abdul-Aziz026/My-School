using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository(DatabaseContext Context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token)
    {
        await Context.AddAsync<RefreshToken>(token);
    }

    public async Task DeleteManyAsync(IEnumerable<RefreshToken> tokens)
    {
        await Context.DeleteManyAsync<RefreshToken>(tokens);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await Context.GetItemByConditionAsync<RefreshToken>(rt => rt.TokenHash == tokenHash);
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        await Context.UpdateAsync<RefreshToken>(token);
    }
}
