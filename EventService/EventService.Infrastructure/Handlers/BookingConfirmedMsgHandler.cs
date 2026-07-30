using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using EventService.Application.Interfaces.Handlers;
using EventService.Application.Interfaces.Repositories;
using EventService.Domain.Constants;
using EventService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Text.Json;

namespace EventService.Infrastructure.Handlers
{
    public class BookingConfirmedMsgHandler(
        ILogger<BookingConfirmedMsgHandler> logger,
        IEventRepository eventRepository,
        IInboxMessageRepository inboxRepository) : IKafkaMessageHandler
    {
        public string Topic => TopicNames.BookingConfirmed;

        public async Task HandleAsync(string payload, CancellationToken ct = default)
        {
            try
            {
                var msg = JsonSerializer.Deserialize<BookingConfirmedMsg>(payload)
                          ?? throw new JsonException("Invalid BookingConfirmedMsg.");

                await inboxRepository.AddAsync(new InboxMessage { Id = msg.Id, ReceivedAtUtc = DateTime.UtcNow }, ct);

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
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
            }

            await eventRepository.SaveChangesAsync(ct);
        }
    }
}
