
using Domain.Entities;
using Domain.Repositories.Base;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(string userId);
}
