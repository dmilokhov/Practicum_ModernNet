using EventService.Application.Model.Filters;
using EventService.Domain.Entities;

namespace EventService.Application.Interfaces.Repositories;

public interface IEventRepository
{
    Task<(IReadOnlyList<Event>, int totalCount)> GetPagedAsync(EventFilter? filter = null, CancellationToken ct = default);
    Task<IReadOnlyList<Event>> GetTopBySalesAsync(int count, CancellationToken ct = default);
    Task<Event> GetAsync(Guid id, CancellationToken ct = default);


    Task AddAsync(Event eventModel, CancellationToken ct = default);
    Task UpdateAsync(Guid eventId, Event data, CancellationToken ct = default);
    Task DeleteAsync(Guid eventId, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
