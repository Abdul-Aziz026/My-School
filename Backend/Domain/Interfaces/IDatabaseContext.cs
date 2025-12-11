namespace Domain.Interfaces;

public interface IDatabaseContext
{
    Task<List<T>> GetAllAsync<T>();
    Task<bool> AddAsync<T>(T entity);
    Task<bool> UpdateAsync<T>(T entity);
    Task<bool> DeleteAsync<T>(T entity);
}
