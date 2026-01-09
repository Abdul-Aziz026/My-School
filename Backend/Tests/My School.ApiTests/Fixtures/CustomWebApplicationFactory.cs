using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Reflection;
using Testcontainers.MongoDb;

namespace YourApp.ApiTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private MongoDbContainer? _mongoDbContainer;
    public IMongoDatabase? Database { get; private set; }
    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .WithName($"myschool-test-mongo-{Guid.NewGuid()}")
            .WithCleanUp(true)
            .Build();

        await _mongoDbContainer.StartAsync();
        ConnectionString = _mongoDbContainer.GetConnectionString();

        // Initialize Database reference
        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase("MySchool-TestDb");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Remove existing configuration
            config.Sources.Clear();

            // Get the path to the test configuration file
            var fileName = "appsettings.Test.json";

            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var rootPath = Directory.GetParent(basePath)!.Parent!.Parent!.FullName;

            var configPath = Path.Combine(rootPath, "My School.ApiTests", "Configuration", fileName);


            // Add test configuration from JSON file
            config.AddJsonFile(configPath, optional: false, reloadOnChange: false);

            // Override MongoDB connection string with test container
            var overrideConfig = new Dictionary<string, string?>
            {
                ["MongoSettings:ConnectionString"] = ConnectionString,
                ["MongoSettings:DatabaseName"] = "MySchool-TestDb"
            };

            config.AddInMemoryCollection(overrideConfig);
        });

        builder.ConfigureServices(services =>
        {
            // Additional service configuration if needed
            // You might want to mock external services like email, RabbitMQ, or Redis here
        });
    }

    public async Task DisposeAsync()
    {
        if (_mongoDbContainer is not null)
        {
            await _mongoDbContainer.StopAsync();
            await _mongoDbContainer.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}