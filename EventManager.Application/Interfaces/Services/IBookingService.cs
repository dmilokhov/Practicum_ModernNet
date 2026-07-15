using EventManager.Application.Commands;
using EventManager.Application.Model.DTOs;

namespace EventManager.Application.Interfaces.Services;
public interface IBookingService
{
    Task<BookingDto> SubmitBookingAsync(SubmitBookingCommand command, CancellationToken ct = default);
    Task CancelBookingAsync(CancelBookingCommand command, CancellationToken ct = default);
    Task<BookingDto> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task ProcessBookingAsync(Guid bookingId, CancellationToken ct = default);
    Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default);
}
