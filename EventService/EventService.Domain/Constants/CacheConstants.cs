namespace EventService.Domain.Constants;

public static class CacheConstants
{
    public const string EventsTop10Key = "events:top10";
    public static string EventKey(Guid id) => $"event:{id.ToString()}";

    public const int CachedEventByIdTtlMinutes = 2;
    public const int CachedTopEventsTtlMinutes = 10;
}
