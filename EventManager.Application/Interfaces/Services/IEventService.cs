using EventManager.Application.Helpers;
using EventManager.Application.Model.DTOs;
using EventManager.Application.Model.Filters;

namespace EventManager.Application.Interfaces.Services;

public interface IEventService
{
    Task<PagedResponse<FullEventDto>> GetEventsAsync(EventFilter filter, CancellationToken ct = default);
    Task<FullEventDto> GetEventAsync(Guid id, CancellationToken ct = default);
    Task<FullEventDto> AddEventAsync(EventDto eventModel, CancellationToken ct = default);
    Task UpdateEventAsync(Guid eventId, EventDto data, CancellationToken ct = default);
    Task DeleteEventAsync(Guid eventId, CancellationToken ct = default);
}
