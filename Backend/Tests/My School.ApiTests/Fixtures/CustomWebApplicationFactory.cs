using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
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
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Remove existing configuration
            config.Sources.Clear();

            // Add test configuration
            var testConfig = new Dictionary<string, string?>
            {
                ["MongoDbSettings:ConnectionString"] = ConnectionString,
                ["MongoDbSettings:DatabaseName"] = "MySchool-TestDb",
                ["Jwt:Key"] = "test-secret-key-for-testing-purposes-minimum-32-chars",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            };

            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices(services =>
        {
            // Additional service configuration if needed
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