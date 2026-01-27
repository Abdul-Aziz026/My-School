using Domain.Entities;
using Domain.Repositories.Base;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository
{
    
    Task<User?> GetByEmailAsync(string email);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
