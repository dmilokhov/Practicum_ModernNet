using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using EventService.Application.Interfaces.Handlers;
using EventService.Application.Interfaces.Repositories;
using System.Text.Json;

namespace EventService.Infrastructure.Handlers
{
    public class BookingCancelledMsgHandler(IEventRepository eventRepository) : IKafkaMessageHandler
    {
        public string Topic => TopicNames.BookingCancelled;

        public async Task HandleAsync(string payload, CancellationToken ct = default)
        {
            var msg = JsonSerializer.Deserialize<BookingCancelledMsg>(payload)
                          ?? throw new JsonException("Invalid BookingCancelledMsg.");

            var eventForBooking = await eventRepository.GetAsync(msg.EventId, ct);
            eventForBooking.ReleaseSeats(msg.SeatsAmount);
            await eventRepository.SaveChangesAsync(ct);
        }
    }
}
