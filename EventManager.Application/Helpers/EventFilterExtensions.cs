using EventManager.Application.Model.Filters;
using EventManager.Domain.Entities;

namespace EventManager.Application.Helpers;

public static class EventFilterExtensions
{
    public static IQueryable<Event> ApplyFilter(this IQueryable<Event> query, EventFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            query = query.Where(e => e.Title.ToLower().Contains(filter.Title.ToLower()));
        }

        if (filter.From.HasValue)
        {
            query = query.Where(e => e.StartAt >= filter.From.Value);
        }

        if (filter.To.HasValue)
        {
            query = query.Where(e => e.EndAt <= filter.To.Value);
        }

        return query;
    }
}
