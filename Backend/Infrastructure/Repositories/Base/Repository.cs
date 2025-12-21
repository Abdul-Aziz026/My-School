
using Domain.Entities;
using Domain.Repositories.Base;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Base;

public class Repository : IRepository
{
    public Task<bool> AddAsync<T>(T entity) where T : BaseEntity
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<T>> GetAllAsync<T>() where T : class
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity
    {
        throw new NotImplementedException();
    }
}
