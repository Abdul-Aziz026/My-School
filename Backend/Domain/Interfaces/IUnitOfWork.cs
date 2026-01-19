
namespace Domain.Interfaces;

// Generic Unit of Work interface
public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    void Dispose();
}
