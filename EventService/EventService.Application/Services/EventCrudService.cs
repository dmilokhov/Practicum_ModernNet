using EventService.Application.Interfaces;
using EventService.Application.Interfaces.Cache;
using EventService.Application.Interfaces.Repositories;
using EventService.Application.Interfaces.Services;
using EventService.Application.Model.DTOs;
using EventService.Application.Model.Filters;
using EventService.Application.Model.Mapping;
using EventService.Application.Responses;
using EventService.Domain.Constants;

namespace EventService.Application.Services;

public class EventCrudService(
    IEventRepository repository,
    IEventFilterValidator eventFilterValidator,
    ICacheService cacheService) : IEventCrudService
{
    public async Task<PagedResponse<FullEventDto>> GetEventsAsync(EventFilter filter, CancellationToken ct = default)
    {
        eventFilterValidator.Validate(filter);

        var (data, totalItems) = await repository.GetPagedAsync(filter, ct);

        var totalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize);
        var items = data.Select(e => e.ToDto()).ToList();

        return new PagedResponse<FullEventDto>(items, filter.Page, filter.PageSize, totalItems, totalPages);
    }

    public async Task<IReadOnlyList<FullEventDto>> GetTopTenPopularEventsAsync(CancellationToken ct)
    {
        var cachedData = await cacheService.GetAsync<IReadOnlyList<FullEventDto>>(CacheConstants.EventsTop10Key);

        if (cachedData != null)
        {
            return cachedData;
        }

        var dbEvents = await repository.GetTopBySalesAsync(ApplicationConstants.PopularEventsCount, ct);
        var result = dbEvents.Select(e => e.ToDto()).ToList();
        
        await cacheService.SendAsync(
            CacheConstants.EventsTop10Key,
            result,
            TimeSpan.FromMinutes(CacheConstants.CachedTopEventsTtlMinutes));

        return result;
    }

    public async Task<FullEventDto> GetEventAsync(Guid id, CancellationToken ct = default)
    {
        var cachedData = await cacheService.GetAsync<FullEventDto>(CacheConstants.EventKey(id));

        if (cachedData != null) 
        {
            return cachedData;
        }

        var dbEvent  = await repository.GetAsync(id, ct);
        var result = dbEvent.ToDto();

        await cacheService.SendAsync(
            CacheConstants.EventKey(id),
            result,
            TimeSpan.FromMinutes(CacheConstants.CachedEventByIdTtlMinutes));

        return result;
    }

    public async Task<FullEventDto> AddEventAsync(EventDto eventModel, CancellationToken ct = default)
    {
        var eventEntity = eventModel.ToEntity();

        await repository.AddAsync(eventEntity, ct);
        await repository.SaveChangesAsync(ct);

        return eventEntity.ToDto();
    }

    public async Task DeleteEventAsync(Guid eventId, CancellationToken ct = default)
    {
        await repository.DeleteAsync(eventId, ct);
        await repository.SaveChangesAsync(ct);
    }

    public async Task UpdateEventAsync(Guid eventId, EventDto data, CancellationToken ct = default)
    {
        await repository.UpdateAsync(eventId, data.ToEntity(), ct);
        await repository.SaveChangesAsync(ct);
    }

}
