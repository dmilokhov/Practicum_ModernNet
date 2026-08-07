using EventService.Application.Model.DTOs;
using EventService.Application.Model.Filters;
using EventService.Application.Responses;

namespace EventService.Application.Interfaces.Services;

public interface IEventCrudService
{
    Task<PagedResponse<FullEventDto>> GetEventsAsync(EventFilter filter, CancellationToken ct = default);
    Task<IReadOnlyList<FullEventDto>> GetTopTenPopularEventsAsync(CancellationToken ct);
    Task<FullEventDto> GetEventAsync(Guid id, CancellationToken ct = default);
    Task<FullEventDto> AddEventAsync(EventDto eventModel, CancellationToken ct = default);
    Task UpdateEventAsync(Guid eventId, EventDto data, CancellationToken ct = default);
    Task DeleteEventAsync(Guid eventId, CancellationToken ct = default);
}
