using Application.Common.Interfaces.Persistence;
using Application.Settings;
using Infrastructure.Helper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<MongoSettings> options)
    {
        IMongoClient client = new MongoClient(options.Value.ConnectionString);
        _database = client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name = null!)
    {
        name = name ?? typeof(T).Name.ToLower();
        return _database.GetCollection<T>(name.ToLower());
    }
}
