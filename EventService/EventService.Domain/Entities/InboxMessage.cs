namespace EventService.Domain.Entities;

public class InboxMessage
{
    public Guid Id { get; init; }
    public DateTime ReceivedAtUtc { get; init; }
    public InboxMessage() {}
}

