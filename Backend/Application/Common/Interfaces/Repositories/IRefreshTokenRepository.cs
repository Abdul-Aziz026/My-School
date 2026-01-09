using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task AddAsync(UserRefreshToken token);
    Task UpdateAsync(UserRefreshToken token);
    Task DeleteManyAsync(IEnumerable<UserRefreshToken> tokens);
}
