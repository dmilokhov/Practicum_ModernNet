using BookingService.Application.Interfaces.Messaging;
using Confluent.Kafka;
using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using EventManager.Common.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BookingService.Infrastructure.Messaging;

public sealed class BookingEventsPublisher(
    ILogger<BookingEventsPublisher> logger,
    IProducer<string, string> producer, 
    IOptions<KafkaSettings> settings) 
    : IBookingEventsPublisher
{
    public async Task PublishBookingConfirmedAsync(BookingConfirmedMsg msg, CancellationToken ct = default)
    {
        var message = new Message<string, string>
        {
            Key = msg.EventId.ToString(),
            Value = JsonSerializer.Serialize(msg)
        };

        var result = await producer.ProduceAsync(TopicNames.BookingConfirmed, message, ct);

        logger.LogInformation("Booking Confirmed msg published to {Topic}. Partition={Partition}, Offset={Offset}",
            result.Topic, result.Partition.Value, result.Offset.Value);
    }
}
