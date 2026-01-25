using MongoDB.Driver;

namespace Infrastructure.DataMigrations.Indexing;

/// <summary>
/// Base class for declaring indexes in code.
/// Inherit and define indexes for each entity collection.
/// </summary>
public abstract class IndexDefinitionBase : IIndexDefinition
{
    public abstract string CollectionName { get; }
    public abstract IndexKeysDefinition<dynamic> Keys { get; }
    public abstract CreateIndexOptions Options { get; }
    public abstract string Description { get; }
}
