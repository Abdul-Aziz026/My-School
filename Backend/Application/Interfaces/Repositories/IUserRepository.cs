
using Domain.Entities;
using Domain.Repositories.Base;

namespace Application.Interfaces.Repositories;

public interface IUserRepository : IRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
