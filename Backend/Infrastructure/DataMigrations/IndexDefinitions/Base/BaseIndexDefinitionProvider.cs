using Infrastructure.DataMigrations.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.DataMigrations.IndexDefinitions.Base;

public abstract class BaseIndexDefinitionProvider : IIndexDefinitionProvider
{
    public abstract IEnumerable<IndexDefinition> GetIndexDefinitions();
    /// <summary>
    /// Creates an ascending index on a single field
    /// </summary>
    protected CreateIndexModel<BsonDocument> CreateAscendingIndex(
            string fieldName,
            string indexName,
            bool isUnique = false)
    {
        var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending(fieldName);
        var indexOptions = new CreateIndexOptions
        {
            Name = indexName,
            Unique = isUnique
        };
        return new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);
    }

    /// <summary>
    /// Creates a descending index on a single field
    /// </summary>
    protected CreateIndexModel<BsonDocument> CreateDescendingIndex(
            string fieldName,
            string indexName,
            bool isUnique = false)
    {
        var indexKeys = Builders<BsonDocument>.IndexKeys.Descending(fieldName);
        var indexOptions = new CreateIndexOptions
        {
            Name = indexName,
            Unique = isUnique
        };
        return new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);
    }

    /// <summary>
    /// Creates a compound index on multiple fields
    /// </summary>
    protected CreateIndexModel<BsonDocument> CreateCompoundIndex(
            Dictionary<string, IndexType> fields,
            string indexName,
            bool isUnique = false)
    {
        var indexKeysBuilder = Builders<BsonDocument>.IndexKeys;
        IndexKeysDefinition<BsonDocument> indexKeys = null;
        foreach (var field in fields)
        {
            var newKey = field.Value == IndexType.Ascending
                    ? indexKeysBuilder.Ascending(field.Key)
                    : indexKeysBuilder.Descending(field.Key);

            indexKeys = (indexKeys == null) ?
                    newKey : indexKeysBuilder.Combine(indexKeys, newKey);
        }
        var indexOptions = new CreateIndexOptions
        {
            Name = indexName,
            Unique = isUnique
        };
        return new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);
    }

    /// <summary>
    /// Creates a TTL (Time To Live) index
    /// </summary>
    protected CreateIndexModel<BsonDocument> CreateTtlIndex(
            string fieldName,
            string indexName,
            TimeSpan expireAfter)
    {
        var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending(fieldName);
        var indexOptions = new CreateIndexOptions
        {
            Name = indexName,
            ExpireAfter = expireAfter
        };
        return new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);
    }

}
