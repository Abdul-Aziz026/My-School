using System.Runtime.InteropServices.Marshalling;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class DatabaseContextClient
{
    private static string ConnectionString = "ConnectionString";
    private static string DatabaseName = "DatabaseName";
    public DatabaseContextClient(IOptions<DBSettings> settings)
    {
        ConnectionString = settings.Value.ConnectionString;
        DatabaseName = settings.Value.DatabaseName;
    }

    //Retry + Timeout Config Version for production use
    //private static IMongoClient GetMongoClient()
    //{
    //    var mongoUrl = new MongoUrl(ConnectionString);
    //    var settings = MongoClientSettings.FromUrl(mongoUrl);

    //    // Retry + Timeout Config
    //    settings.RetryWrites = true;
    //    settings.ConnectTimeout = TimeSpan.FromSeconds(5);
    //    settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);

    //    return new MongoClient(settings);
    //}

    private static IMongoClient GetMongoClient()
    {
        return new MongoClient(ConnectionString);
    }

    private static IMongoDatabase GetDataBase()
    {
        var client = GetMongoClient();
        return client.GetDatabase(DatabaseName);
    }

    public static IMongoCollection<T> GetCollection<T>(string collectionName = null)
    {
        var database = GetDataBase();
        collectionName = collectionName ?? typeof(T).Name.ToLower();
        return database.GetCollection<T>(collectionName.ToLower());
    }
}
