using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Persistence;

public class DatabaseContext : IDatabaseContext
{
    private readonly ILogger<DatabaseContext> _logger;
    public DatabaseContext(ILogger<DatabaseContext> logger)
    {
        _logger = logger;
    }

    public async Task<string> Hello<T>()
    {
        var collection = DatabaseContextClient.GetCollection<T>();
        return "Alhamdulillah from DatabaseContext";
    }

    public async Task<List<T>> GetAllAsync<T>() where T : BaseEntity
    {
        try {
            var collection = DatabaseContextClient.GetCollection<T>();
            var response = await collection.Find(_ => true).ToListAsync();
            _logger.LogInformation($"Retrieved all entities of type {typeof(T).FullName}, count: {response.Count}");
            return response;
        }
        catch
        {
            _logger.LogError($"GetAllAsync failed for type {typeof(T).FullName}");
            return default!;
        }
    }

    public async Task<bool> AddAsync<T>(T entity) where T : BaseEntity
    {
        try
        {
            var collection = DatabaseContextClient.GetCollection<T>();
            await collection.InsertOneAsync(entity);
            _logger.LogInformation($"Added entity of type {typeof(T).FullName} with Id {entity.Id}");
            return true;
        }
        catch
        {
            _logger.LogError($"AddAsync failed for type {typeof(T).FullName} with Id {entity.Id}");
            return false;
        }
    }

    public async Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity
    {
        try
        {
            var collection = DatabaseContextClient.GetCollection<T>();
            var result = await collection.ReplaceOneAsync(o => o.Id.Equals(entity.Id), entity);
            var success = result.IsAcknowledged && result.ModifiedCount > 0;
            if (success)
            {
                _logger.LogInformation($"Updated entity of type {typeof(T).FullName} with Id {entity.Id}");
            }
            else
            {
                _logger.LogWarning($"UpdateAsync did not modify any document for type {typeof(T).FullName} with Id {entity.Id}");
            }

            return success;
        }
        catch
        {
            _logger.LogError($"UpdateAsync failed for type {typeof(T).FullName} with Id {entity.Id}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity
    {
        try
        {
            var collection = DatabaseContextClient.GetCollection<T>();
            await collection.DeleteOneAsync(o => o.Id.Equals(entity.Id));
            _logger.LogInformation($"Deleted entity of type {typeof(T).FullName} with Id {entity.Id}");
            return true;
        }
        catch
        {
            _logger.LogError($"DeleteAsync failed for type {typeof(T).FullName} with Id {entity.Id}");
            return false;
        }
    }

    public async Task<int> DeleteManyAsync<T>(IEnumerable<T> entities) where T : BaseEntity
    {
        try
        {
            var collection = DatabaseContextClient.GetCollection<T>();
            var ids = entities.Select(e => e.Id);
            var result = await collection.DeleteManyAsync(
                Builders<T>.Filter.In(x => x.Id, ids)
            );

            _logger.LogInformation(
                $"Deleted {result.DeletedCount} {typeof(T).Name} documents");

            return (int)result.DeletedCount;
        }
        catch
        {
            _logger.LogError($"DeleteManyAsync failed for type {typeof(T).FullName}");
            return 0;
        }
    }

    public async Task<bool> SoftDeleteAsync<T>(T entity) where T : BaseEntity
    {
        try
        {
            var collection = DatabaseContextClient.GetCollection<T>();
            entity.IsDeleted = true;
            var response = await UpdateAsync<T>(entity);
            if (response)
            {
                _logger.LogInformation($"Soft deleted entity of type {typeof(T).FullName} with Id {entity.Id}");
            }
            else
            {
                _logger.LogWarning($"SoftDeleteAsync did not modify any document for type {typeof(T).FullName} with Id {entity.Id}");
            }
            return response;
        }
        catch
        {
            _logger.LogError($"SoftDeleteAsync failed for type {typeof(T).FullName} with Id {entity.Id}");
            return false;
        }
    }

    public async Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        var collection = DatabaseContextClient.GetCollection<T>();
        var filter = Builders<T>.Filter.Where(criteria);
        var response = await collection.Find(filter).FirstOrDefaultAsync();
        return response;
    }

    public async Task<List<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        var collection = DatabaseContextClient.GetCollection<T>();
        var filter = Builders<T>.Filter.Where(criteria);
        var results = await collection
            .Find(filter)
            .ToListAsync();   // fetch all matching documents
        return results;
    }
}
