using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserRepository : Repository, IUserRepository
{
    public UserRepository(DatabaseContext dbContext) : base(dbContext) {}

    public async Task AddAsync(User user)
    {
        
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await DbContext.GetItemByConditionAsync<User>(u => u.Email == email);
    }

    
}
