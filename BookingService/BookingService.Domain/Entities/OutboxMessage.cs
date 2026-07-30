namespace BookingService.Domain.Entities;

public class OutboxMessage
{
    public Guid Id { get; init; }
    public string Topic { get; init; } = default!;
    public string Key { get; init; } = default!;
    public string Type { get; init; } = default!;
    public string Payload { get; init; } = default!;
    public DateTime CreatedAtUtc { get; init; }
    public bool IsProcessed { get; set; }

    public OutboxMessage() {}

    public OutboxMessage(
        Guid id,
        string topic,
        string key,
        string type,
        string payload,
        DateTime createdAtUtc,
        bool isProcessed)
    {
        Id = id;
        Topic = topic;
        Key = key;
        Type = type;
        Payload = payload;
        CreatedAtUtc = createdAtUtc;
        IsProcessed = isProcessed;
    }
}
