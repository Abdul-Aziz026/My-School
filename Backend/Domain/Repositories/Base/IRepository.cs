using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Repositories.Base;

public interface IRepository
{
    Task<bool> AddAsync<T>(T entity) where T : BaseEntity;
    Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity;
    Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity;
    //Task<int> CountAsync(ISpecification<T> spec);
    Task<IReadOnlyList<T>> GetAllAsync<T>() where T : class;
    Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity;
    Task<IReadOnlyList<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity;
}
