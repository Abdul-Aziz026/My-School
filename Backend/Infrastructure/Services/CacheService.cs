
using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

public class CacheService : ICacheService
{
    public async Task<bool> ExistsAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        throw new NotImplementedException();
    }
}
