using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task DeleteManyAsync(IEnumerable<RefreshToken> tokens);
}
