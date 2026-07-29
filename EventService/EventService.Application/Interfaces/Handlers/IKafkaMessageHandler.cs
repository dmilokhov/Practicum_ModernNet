namespace EventService.Application.Interfaces.Handlers;

public interface IKafkaMessageHandler
{
    string Topic { get; }

    Task HandleAsync(string payload, CancellationToken ct = default);
}
