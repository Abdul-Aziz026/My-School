using Infrastructure.DataMigrations.Models;
using MongoDB.Bson;
using MongoDB.Driver;

public class IndexDefinition
{
    public string Version { get; set; }
    public string CollectionName { get; set; }
    public string IndexName { get; set; }
    public IndexAction Action { get; set; }
    public CreateIndexModel<BsonDocument> IndexModel { get; set; }
}
