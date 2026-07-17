using EventManager.Application.Commands;
using EventManager.Application.Responses;

namespace EventManager.Application.Interfaces.Services;
public interface IBookingService
{
    Task<BookingResponse> SubmitBookingAsync(SubmitBookingCommand command, CancellationToken ct = default);
    Task CancelBookingAsync(CancelBookingCommand command, CancellationToken ct = default);
    Task<BookingResponse> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task ProcessBookingAsync(Guid bookingId, CancellationToken ct = default);
    Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default);
}
