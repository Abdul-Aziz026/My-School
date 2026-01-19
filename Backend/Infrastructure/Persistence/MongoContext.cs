using Application.Common.Interfaces.Persistence;
using Application.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoClient _client;

    public MongoContext(IOptions<MongoSettings> options)
    {
        // network compression options:
        // reduces the amount of data passed over the network between mongodb & app.
        // var uri = $"{options.Value.ConnectionString}?compressors=snappy,zlib,zstd";
        // var settings = MongoClientSettings.FromConnectionString(uri);

        var settings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);
        settings.UseTls = options.Value.UseTls;
        settings.MaxConnecting = options.Value.MaxConnecting;
        settings.MinConnectionPoolSize = options.Value.MinConnectionPoolSize;
        settings.MaxConnectionPoolSize = options.Value.MaxConnectionPoolSize;
        settings.MaxConnectionLifeTime = TimeSpan.FromMinutes(options.Value.MaxConnectionLifeTime);
        settings.WaitQueueTimeout = TimeSpan.FromSeconds(options.Value.WaitQueTimeout);
        settings.RetryWrites = options.Value.RetryWrites;
        settings.RetryWrites = options.Value.RetryReads;
        settings.WriteConcern = WriteConcern.WMajority;
        settings.ReadConcern = ReadConcern.Majority;

        _client = new MongoClient(settings);
        _database = _client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name = null!)
    {
        name = name ?? typeof(T).Name.ToLower();
        return _database.GetCollection<T>(name.ToLower());
    }

    public Task<IClientSessionHandle> StartSessionAsync()
    {
        return _client.StartSessionAsync();
    }
}
