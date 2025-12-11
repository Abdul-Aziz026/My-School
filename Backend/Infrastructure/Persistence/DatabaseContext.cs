using Domain.Interfaces;

namespace Infrastructure.Persistence;

public class DatabaseContext : IDatabaseContext
{
    public Task<List<T>> GetAllAsync<T>()
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddAsync<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync<T>(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync<T>(T entity)
    {
        throw new NotImplementedException();
    }
}
