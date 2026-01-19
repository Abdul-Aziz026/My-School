
using MongoDB.Driver;

namespace Application.Common.Interfaces.Persistence;

// Generic Unit of Work interface
public interface IUnitOfWork : IDisposable
{
    IClientSessionHandle Session { get; }
    Task StartTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();

    // Generic repository accessor
    Task Repository<T>() where T : class;
}
