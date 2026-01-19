
using Application.Common.Interfaces.Persistence;
using Application.Settings;
using Domain.Interfaces;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDatabaseContext _dbContext;
    private readonly string _indexInfo;
    private IDatabaseContext _transactionContext;

    public UnitOfWork(IDatabaseContext context, IOptions<MongoSettings> option)
    {
        _dbContext = context;
        _indexInfo = option.Value.DatabaseName;
    }

    public async Task BeginTransactionAsync()
    {
        _transactionContext = _dbContext.BeginTransaction();
        await Task.CompletedTask;
    }

    public async Task CommitAsync()
    {
        if (_transactionContext != null)
        {
            await _transactionContext.CommitTransactionAsync();
            _transactionContext = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transactionContext != null)
        {
            await _transactionContext.AbortTransactionAsync();
            _transactionContext = null;
        }
    }
    
    public void Dispose()
    {
        _transactionContext?.Dispose();
    }
}
