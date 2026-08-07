namespace EventService.Application.Interfaces.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SendAsync<T>(string key, T value, TimeSpan ttl);
    Task<bool> RemoveAsync(string key);
}
