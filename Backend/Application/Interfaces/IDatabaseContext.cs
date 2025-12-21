using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IDatabaseContext 
{
    Task<string> Hello<T>();
    Task<List<T>> GetAllAsync<T>() where T : BaseEntity;
    Task<bool> AddAsync<T>(T entity) where T : BaseEntity;
    Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity;
    Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity;
    Task<int> DeleteManyAsync<T>(IEnumerable<T> entities) where T : BaseEntity;
    Task<bool> SoftDeleteAsync<T>(T entity) where T : BaseEntity;
    Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity;
    Task<List<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity;
}
