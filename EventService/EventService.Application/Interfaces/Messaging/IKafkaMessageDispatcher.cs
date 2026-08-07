namespace EventService.Application.Interfaces.Messaging;

public interface IKafkaMessageDispatcher
{
    Task DispatchAsync(string topic, string messageRaw, CancellationToken ct = default);
}
