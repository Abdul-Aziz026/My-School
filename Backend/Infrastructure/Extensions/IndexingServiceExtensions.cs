using Application.Common.Interfaces.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class IndexingServiceExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var indexInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseIndexInitializer>();
        await indexInitializer.InitializeIndexesAsync();
    }
}
