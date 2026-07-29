using EventManager.Common.Core.Contracts;

namespace BookingService.Application.Interfaces.Messaging;

public interface IBookingEventsPublisher
{
    Task PublishBookingConfirmedAsync(BookingConfirmedMsg msg, CancellationToken ct = default);
}
