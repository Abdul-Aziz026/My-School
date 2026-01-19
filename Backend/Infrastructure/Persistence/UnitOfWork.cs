
using Application.Common.Interfaces.Persistence;
using Application.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private IClientSessionHandle _session;

    public UnitOfWork(IOptions<MongoSettings> options)
    {
        _client = new MongoClient(options.Value.ConnectionString);
        _database = _client.GetDatabase(options.Value.DatabaseName);
    }



    public IClientSessionHandle Session => throw new NotImplementedException();

    public async Task StartTransactionAsync()
    {
        _session = await _client.StartSessionAsync();
        _session.StartTransaction();
    }

    public async Task CommitAsync()
    {
        if (_session != null && _session.IsInTransaction)
        {
            await _session.CommitTransactionAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_session != null && _session.IsInTransaction)
        {
            await _session.AbortTransactionAsync();
        }
    }
    public Task Repository<T>() where T : class
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}
