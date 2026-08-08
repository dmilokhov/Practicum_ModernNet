using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using EventService.Application.Interfaces.Cache;
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
    public class BookingCancelledMsgHandler(
        ILogger<BookingCancelledMsgHandler> logger,
        IEventRepository eventRepository,
        IInboxMessageRepository inboxRepository,
        IEventCacheInvalidator eventCacheInvalidator) : IKafkaMessageHandler
    {
        public string Topic => TopicNames.BookingCancelled;

        public async Task HandleAsync(string payload, CancellationToken ct = default)
        {
            var msg = JsonSerializer.Deserialize<BookingCancelledMsg>(payload)
                          ?? throw new JsonException("Invalid BookingCancelledMsg.");

            try
            {
                await inboxRepository.AddAsync(new InboxMessage { Id = msg.Id, ReceivedAtUtc = DateTime.UtcNow }, ct);

                var eventForBooking = await eventRepository.GetAsync(msg.EventId, ct);
                eventForBooking.ReleaseSeats(msg.SeatsAmount);
                await eventRepository.SaveChangesAsync(ct);
                await eventCacheInvalidator.InvalidateAsync(msg.EventId);
            }
            catch(DbUpdateException ex) 
                when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                logger.LogWarning(WarningMessages.MessageWasHandledWarningMsg(msg.Id));
            }
        }
    }
}
