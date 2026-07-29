using System.Text.Json;
using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using EventService.Application.Interfaces.Handlers;
using EventService.Application.Interfaces.Repositories;
using EventService.Domain.Constants;
using Microsoft.Extensions.Logging;

namespace EventService.Infrastructure.Handlers
{
    public class BookingConfirmedMsgHandler(
        ILogger<BookingConfirmedMsgHandler> logger,
        IEventRepository eventRepository) : IKafkaMessageHandler
    {
        public string Topic => TopicNames.BookingConfirmed;

        public async Task HandleAsync(string payload, CancellationToken ct = default)
        {
            var msg = JsonSerializer.Deserialize<BookingConfirmedMsg>(payload)
                      ?? throw new JsonException("Invalid BookingConfirmedMsg.");

            var eventForBooking = await eventRepository.GetAsync(msg.EventId, ct);

            if (DateTime.UtcNow >= eventForBooking.StartAt)
            {
                logger.LogError($"Booking {msg.BookingId} - {ErrorMessages.TryBookStartedEventErrorMsg}");
                return;
            }

            var reserved = eventForBooking.TryReserveSeats(msg.SeatsAmount);
            if (!reserved)
            {
                logger.LogError($"Booking {msg.BookingId} - {ErrorMessages.NoAvailableSeatsErrorMsg}");
                return;
            }

            await eventRepository.SaveChangesAsync(ct);
        }
    }
}
