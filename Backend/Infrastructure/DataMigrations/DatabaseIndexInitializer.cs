
using Application.Common.Interfaces.Persistence;
using Infrastructure.DataMigrations.IndexDefinitions.Base;
using Infrastructure.DataMigrations.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.DataMigrations;

public class DatabaseIndexInitializer : IDatabaseIndexInitializer
{
    private readonly IDatabaseContext _context;
    private readonly IEnumerable<IIndexDefinitionProvider> _indexDefinitionProviders;
    private readonly IMongoCollection<IndexMigration> indexMigrationsCollection;
    private readonly ILogger<DatabaseIndexInitializer> _logger;
    
    public DatabaseIndexInitializer(
        IDatabaseContext context,
        IEnumerable<IIndexDefinitionProvider> indexDefinitionProviders,
        ILogger<DatabaseIndexInitializer> logger
        )
    {
        _context = context;
        indexMigrationsCollection = _context.GetCollection<IndexMigration>("indexmigration");
        _indexDefinitionProviders = indexDefinitionProviders;
        _logger = logger;
    }
    public async Task InitializeIndexesAsync()
    {
        var skipCount = 0;
        var processedCount = 0;

        foreach (var provider in _indexDefinitionProviders)
        {
            var allIndexDefinitions = _indexDefinitionProviders
                .SelectMany(p => p.GetIndexDefinitions())
                .OrderBy(idx => idx.Version).ToList();
            foreach (var indexDef in allIndexDefinitions)
            {
                var isApplied = await IndexMigrationAppliedAsync(indexDef);
                if (!isApplied)
                {
                    if (indexDef.Action == IndexAction.Create)
                    {
                        await CreateIndexAsync(indexDef);
                    }
                    else if (indexDef.Action == IndexAction.Remove)
                    {
                        await DropIndexAsync(indexDef);
                    }
                    await RecordIndexMigrationAsync(indexDef);
                    processedCount++;
                }
                else {
                    skipCount++;
                }
            }
        }
        _logger.LogInformation($"{processedCount} {skipCount}")
    }

    private async Task RecordIndexMigrationAsync(IndexDefinition indexDef)
    {
        var migrationRecord = new IndexMigration
        {
            Version = indexDef.Version,
            CollectionName = indexDef.CollectionName,
            IndexName = indexDef.IndexName,
            AppliedAt = DateTime.UtcNow
        };
        await indexMigrationsCollection.InsertOneAsync(migrationRecord);
    }

    private async Task CreateIndexAsync(IndexDefinition indexDef)
    {
        try
        {
            var collection = _context.GetCollection<BsonDocument>(indexDef.CollectionName);
            await collection.Indexes.CreateOneAsync(indexDef.IndexModel);
            Console.WriteLine($"✓ Created index '{indexDef.IndexName}' on collection '{indexDef.CollectionName}' (Version: {indexDef.Version})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Failed to create index '{indexDef.IndexName}': {ex.Message}");
            throw;
        }
    }

    private async Task DropIndexAsync(IndexDefinition indexDef)
    {
        try
        {
            var collection = _context.GetCollection<BsonDocument>(indexDef.CollectionName);
            await collection.Indexes.DropOneAsync(indexDef.IndexName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Index '{indexDef.IndexName}' not removed. Error message: {ex.Message}");
        }
    }

    private async Task<bool> IndexMigrationAppliedAsync(IndexDefinition indexDef)
    {
        var filter = Builders<IndexMigration>.Filter.And(
            Builders<IndexMigration>.Filter.Eq(im => im.Version, indexDef.Version),
            Builders<IndexMigration>.Filter.Eq(im => im.CollectionName, indexDef.CollectionName),
            Builders<IndexMigration>.Filter.Eq(im => im.IndexName, indexDef.IndexName)
        );
        var count = await indexMigrationsCollection.CountDocumentsAsync(filter);
        return count > 0;
    }
}
