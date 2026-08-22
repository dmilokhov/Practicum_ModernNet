using EventManager.Common.Core.Constants;
using EventService.Application.Interfaces.Cache;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace EventService.Infrastructure.Cache;

public sealed class RedisCacheService(IConnectionMultiplexer connection, ILogger<RedisCacheService> logger) : ICacheService
{
    private readonly IDatabase _database = connection.GetDatabase();

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedValue = await _database.StringGetAsync(key);

            return cachedValue.HasValue ? JsonSerializer.Deserialize<T>(cachedValue!, JsonOptions) 
                                        : default;
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, CommonExceptionMessages.RedisActionFailed("GET", key));
            return default;
        }
    }

    public async Task<bool> SendAsync<T>(string key, T value, TimeSpan ttl)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);
            return await _database.StringSetAsync(key, json, ttl);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, CommonExceptionMessages.RedisActionFailed("SET", key));
            return false;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            return await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, CommonExceptionMessages.RedisActionFailed("DELETE", key));
            return false;
        }
    }
}
