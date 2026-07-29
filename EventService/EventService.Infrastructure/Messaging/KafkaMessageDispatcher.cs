using EventService.Application.Interfaces.Handlers;
using EventService.Application.Interfaces.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.Infrastructure.Messaging;

public class KafkaMessageDispatcher(IServiceScopeFactory scopeFactory) : IKafkaMessageDispatcher
{
    public async Task DispatchAsync(string topic, string messageRaw, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();

        var handlers = scope.ServiceProvider.GetServices<IKafkaMessageHandler>();

        var handler = handlers.FirstOrDefault(h => h.Topic == topic);

        if (handler is null)
        {
            throw new InvalidOperationException($"No handler registered for topic '{topic}'.");
        }

        await handler.HandleAsync(messageRaw, ct);
    }
}
