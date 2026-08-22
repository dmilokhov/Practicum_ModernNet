namespace EventService.Application.Interfaces.Cache;

public interface IEventCacheInvalidator
{
    Task InvalidateAsync(Guid eventId);
}
