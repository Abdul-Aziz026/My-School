
using Domain.Entities;
using Domain.Repositories.Base;
using Infrastructure.Persistence;
using SharpCompress.Common;
using System;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Base;

public class Repository : IRepository
{
    protected readonly DatabaseContext DbContext;
    public Repository(DatabaseContext dbContext)
    {
        DbContext = dbContext;
    }
    public async Task<bool> AddAsync<T>(T entity) where T : BaseEntity
    {
        return await DbContext.AddAsync<T>(entity);
    }

    public Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity
    {
        throw new NotImplementedException();
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        return await DbContext.GetAllAsync<T>();
    }

    public async Task<T?> GetByIdAsync<T>(string userId) where T : BaseEntity
    {
        return await DbContext.GetItemByConditionAsync<T>(u => u.Id == userId);
    }

    public Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity
    {
        return await DbContext.UpdateAsync<T>(entity);
    }
}
