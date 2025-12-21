using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Infrastructure.Configuration;
using System.Runtime.CompilerServices;

namespace Infrastructure.Persistence;

public class DatabaseContextClient
{
    private static string ConnectionString = "";
    private static string DatabaseName = "";
    private static bool Init = false;

    private static IMongoClient GetMongoClient()
    {
        Initialize();
        return new MongoClient(ConnectionString);
    }

    private static IMongoDatabase GetDataBase()
    {
        var client = GetMongoClient();
        return client.GetDatabase(DatabaseName);
    }

    public static IMongoCollection<T> GetCollection<T>(string collectionName = null!)
    {
        var database = GetDataBase();
        collectionName = collectionName ?? typeof(T).Name.ToLower();
        return database.GetCollection<T>(collectionName.ToLower());
    }

    private static void Initialize()
    {
        if (Init) return;
        ConnectionString = ConfigurationHelper.GetConfigurationValue("DBSettings:ConnectionString");
        DatabaseName = ConfigurationHelper.GetConfigurationValue("DBSettings:DatabaseName");
    }
}
