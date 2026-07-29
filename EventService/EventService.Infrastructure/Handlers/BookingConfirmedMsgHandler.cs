using EventManager.Common.Core.Contracts;
using EventService.Application.Interfaces.Handlers;
using EventService.Application.Interfaces.Repositories;
using EventService.Domain.Constants;
using Microsoft.Extensions.Logging;

namespace EventService.Infrastructure.Handlers
{
    public class BookingConfirmedMsgHandler(
        ILogger<BookingConfirmedMsgHandler> logger,
        IEventRepository eventRepository) : IBookingConfirmedMsgHandler
    {
        public async Task HandleAsync(BookingConfirmedMsg msg, CancellationToken ct = default)
        {
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
