using EventService.Application.Interfaces.Cache;
using EventService.Domain.Constants;

namespace EventService.Infrastructure.Cache;

public sealed class EventCacheInvalidator(ICacheService cacheService) : IEventCacheInvalidator
{
    public async Task InvalidateAsync(Guid eventId)
    {
        await cacheService.RemoveAsync(CacheConstants.EventKey(eventId));
    }
}
