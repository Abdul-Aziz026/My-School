using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UserRepository(DatabaseContext Context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await Context.AddAsync<User>(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Context.GetItemByConditionAsync<User>(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(string userId)
    {
        return await Context.GetItemByConditionAsync<User>(u => u.Id == userId);
    }
}
