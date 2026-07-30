using BookingService.Application.Interfaces.Messaging;
using BookingService.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.Messaging;

public sealed class BookingEventsPublisher(
    ILogger<BookingEventsPublisher> logger,
    IProducer<string, string> producer) 
    : IBookingEventsPublisher 
{
    public async Task PublishAsync(OutboxMessage message, CancellationToken ct = default)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = message.Key,
            Value = message.Payload
        };

        var result = await producer.ProduceAsync(message.Topic, kafkaMessage, ct);

        logger.LogInformation(
            "Message {MessageId} published to {Topic}. Partition={Partition}, Offset={Offset}",
            message.Id,
            result.Topic,
            result.Partition.Value,
            result.Offset.Value);
    }
}
