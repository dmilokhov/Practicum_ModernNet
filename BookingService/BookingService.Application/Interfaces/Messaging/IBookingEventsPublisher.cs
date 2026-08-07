using BookingService.Domain.Entities;

namespace BookingService.Application.Interfaces.Messaging;

public interface IBookingEventsPublisher 
{
    Task PublishAsync(OutboxMessage message, CancellationToken ct = default);
}
