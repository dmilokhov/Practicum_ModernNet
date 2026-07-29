using BookingService.Application.Interfaces.Messaging;
using Confluent.Kafka;
using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookingService.Infrastructure.Messaging;

public sealed class BookingEventsPublisher(
    ILogger<BookingEventsPublisher> logger,
    IProducer<string, string> producer) 
    : IBookingEventsPublisher 
{
    private static readonly Dictionary<Type, string> Topics = new()
    {
        { typeof(BookingConfirmedMsg), TopicNames.BookingConfirmed },
        { typeof(BookingCancelledMsg), TopicNames.BookingCancelled }
    };

    public async Task PublishAsync<TMessage>(string key, TMessage msg, CancellationToken ct = default) where TMessage : class
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = JsonSerializer.Serialize(msg)
        };

        if (!Topics.TryGetValue(typeof(TMessage), out var topic))
        {
            throw new InvalidOperationException(
                $"No Kafka topic configured for event '{typeof(TMessage).Name}'.");
        }

        var result = await producer.ProduceAsync(topic, message, ct);

        logger.LogInformation("Booking Confirmed msg published to {Topic}. Partition={Partition}, Offset={Offset}",
            result.Topic, result.Partition.Value, result.Offset.Value);
    }
}
