
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.DataMigrations;

public class IndexMigration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Version { get; set; }
    public string CollectionName { get; set; }
    public string IndexName { get; set; }
    public DateTime AppliedAt { get; set; }
    public IndexAction Action { get; set; } // "Create" or "Remove"
}

public enum IndexAction
{
    Create,
    Remove
}

// Index definition
public class IndexDefinition
{
    public string Version { get; set; }
    public string CollectionName { get; set; }
    public string IndexName { get; set; }
    public IndexAction Action { get; set; }
    public CreateIndexModel<BsonDocument> IndexModel { get; set; }
}
