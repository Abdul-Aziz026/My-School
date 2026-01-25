
using MongoDB.Driver;

namespace Infrastructure.DataMigrations.Indexing;

/// <summary>
/// Defines a single index for a MongoDB collection.
/// </summary>
public interface IIndexDefinition
{
    /// <summary>
    /// Collection name (e.g., "student", "enrollment")
    /// </summary>
    string CollectionName { get; }

    /// <summary>
    /// Index key specification (e.g., ascending Email, descending CreatedAt)
    /// </summary>
    IndexKeysDefinition<dynamic> Keys { get; }

    /// <summary>
    /// Index options (name, unique, sparse, background, etc.)
    /// </summary>
    CreateIndexOptions Options { get; }

    /// <summary>
    /// Human-readable description of what this index is for.
    /// </summary>
    string Description { get; }
}
