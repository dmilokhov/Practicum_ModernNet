using EventManager.Common.Core.Contracts;

namespace EventService.Application.Interfaces.Handlers;

public interface IBookingConfirmedMsgHandler
{
    Task HandleAsync(BookingConfirmedMsg msg, CancellationToken ct = default);
}
