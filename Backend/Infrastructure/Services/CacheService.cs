
using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly TimeSpan _defaultExpiration;
    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _logger = logger;
        _cache = cache;

        _defaultExpiration = TimeSpan.FromMinutes(10);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache miss for key: {Key}", key);
                return default!;
            }

            _logger.LogInformation("Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached data for key: {Key}", key);
            return null;
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _logger.LogInformation("Cache removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache for key: {Key}", key);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
            };

            await _cache.SetStringAsync(key, serializedData, options);
            _logger.LogInformation($"Data cached for key: {key} with expiration: {options.AbsoluteExpirationRelativeToNow}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {Key}", key);
        }
    }
}
