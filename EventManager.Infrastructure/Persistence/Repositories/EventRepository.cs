using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Model.Filters;
using EventManager.Domain.Entities;
using EventManager.Domain.Exceptions;
using EventManager.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence.Repositories;
public class EventRepository(AppDbContext context) : IEventRepository
{
    public async Task<(IReadOnlyList<Event>, int totalCount)> GetPagedAsync(
        EventFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new EventFilter();

        var filteredEventsQuery = context.Events.AsNoTracking().ApplyFilter(filter);
        var totalCount = await filteredEventsQuery.CountAsync(cancellationToken: ct);
        var items = await filteredEventsQuery
            .OrderByDescending(e => e.StartAt)
            .ThenBy(e => e.Title)
            .ApplyPagination(filter.Page, filter.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Event> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await TryGetEventAsync(id, ct);
    }

    public async Task AddAsync(Event eventModel, CancellationToken ct = default)
    {
        await context.Events.AddAsync(eventModel, ct);
    }

    public async Task UpdateAsync(Guid eventId, Event data, CancellationToken ct = default)
    {
        var foundEvent = await TryGetEventAsync(eventId, ct);
        foundEvent.Update(data.Title, data.Description, data.StartAt, data.EndAt, data.TotalSeats);
    }

    public async Task DeleteAsync(Guid eventId, CancellationToken ct = default)
    {
        var foundEvent = await TryGetEventAsync(eventId, ct);
        context.Events.Remove(foundEvent);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }

    private async Task<Event> TryGetEventAsync(Guid id, CancellationToken ct = default)
    {
        var foundEvent = await context.Events.FirstOrDefaultAsync(e => e.Id == id, ct);

        if (foundEvent is null)
        {
            throw new EntityNotFoundException(nameof(Event), id);
        }

        return foundEvent;
    }

}
