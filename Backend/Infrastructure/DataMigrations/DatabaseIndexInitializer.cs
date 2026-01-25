
using Application.Common.Interfaces.Persistence;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Infrastructure.DataMigrations;

public class DatabaseIndexInitializer(IDatabaseContext context)
{
    public async Task InitializeIndexesAsync()
    {
        // Implementation for initializing database indexes goes here.
        var indexDefinitions = GetIndexDefinitions();




        // This could involve checking existing indexes and creating new ones as needed.
    }

    private async Task CreateIndexAsync(IndexDefinition indexDefinition)
    {
        try
        {
            var collection = context.GetCollection<BsonDocument>(indexDefinition.CollectionName);
            await collection.Indexes.CreateOneAsync(indexDefinition.IndexModel);
            Console.WriteLine($"✓ Created index '{indexDefinition.IndexName}' on collection '{indexDefinition.CollectionName}' (Version: {indexDef.Version})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Failed to create index '{indexDefinition.IndexName}': {ex.Message}");
            throw;
        }
    }

    private async Task DropIndexAsync(IndexDefinition indexDefinition)
    {
        try
        {
            var collection = context.GetCollection<BsonDocument>(indexDefinition.CollectionName);
            await collection.Indexes.DropOneAsync(indexDefinition.IndexName);

            Console.WriteLine($"✓ Removed index '{indexDefinition.IndexName}' from collection '{indexDefinition.CollectionName}' (Version: {indexDef.Version})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Index '{indexDefinition.IndexName}' not removed. Error message: {ex.Message}");
        }
    }


    private List<IndexDefinition> GetIndexDefinitions()
    {
        // Implementation for retrieving index definitions goes here.
        return new List<IndexDefinition>() {
            new IndexDefinition()
            {
                Version = "1.0.0",
                CollectionName = "Users",
                IndexName = "idx_users_email",
                Action = IndexAction.Create,
                IndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys.Ascending("Email"),
                    new CreateIndexOptions { Name = "idx_users_email", Unique = true }
                )
            },
        // Version 1.1.0 - Add compound index
            new IndexDefinition
            {
                Version = "1.1.0",
                CollectionName = "Orders",
                IndexName = "idx_orders_userId_status",
                Action = IndexAction.Create,
                IndexModel = new CreateIndexModel<BsonDocument>(
                    Builders<BsonDocument>.IndexKeys
                            .Ascending("UserId")
                            .Ascending("Status"),
                            new CreateIndexOptions { 
                                Name = "idx_orders_userId_status" 
                            }
                )
            }
        };
    }
}

// Extension method for easy registration in Startup.cs
public static class MongoIndexInitializerExtensions
{
    public static async Task InitializeMongoIndexesAsync(this IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<IDatabaseContext>();
        var initializer = new DatabaseIndexInitializer(context);
        await initializer.InitializeIndexesAsync();
    }
}
