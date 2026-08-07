namespace EventService.Infrastructure.Persistence.Extensions;

public static class QueryPaginationExtensions
{
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
    {
        var skip = (page - 1) * pageSize;

        return query
            .Skip(skip)
            .Take(pageSize);
    }
}
