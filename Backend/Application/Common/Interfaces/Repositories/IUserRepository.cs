using Domain.Entities;
using Domain.Repositories.Base;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository
{
    Task<List<User>> GetPagedAsync(Expression<Func<User, bool>>? filter = null,
                                          int pageNumber = 1,
                                          int pageSize = 10,
                                          Expression<Func<User, object>>? orderBy = null,
                                          bool ascending = true);
    Task<long> CountAsync(Expression<Func<User, bool>> filter);
    Task<User?> GetByEmailAsync(string email);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
