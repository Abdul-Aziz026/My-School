using Domain.Entities;

namespace Domain.Interfaces;

public interface IDatabaseContext
{
    Task<List<T>> GetAllAsync<T>() where T : BaseEntity;
    Task<bool> AddAsync<T>(T entity) where T : BaseEntity;
    Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity;
    Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity;
    Task<bool> SoftDeleteAsync<T>(T entity) where T : BaseEntity;
}
