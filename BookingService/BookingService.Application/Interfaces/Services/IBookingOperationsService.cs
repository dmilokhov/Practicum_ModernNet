using BookingService.Application.Commands;
using BookingService.Application.Responses;

namespace BookingService.Application.Interfaces.Services;
public interface IBookingOperationsService
{
    Task<BookingResponse> SubmitBookingAsync(SubmitBookingCommand command, CancellationToken ct = default);
    Task CancelBookingAsync(CancelBookingCommand command, CancellationToken ct = default);
    Task<BookingResponse> GetBookingByIdAsync(GetBookingByIdCommand command, CancellationToken ct = default);
    Task ProcessBookingAsync(Guid bookingId, CancellationToken ct = default);
    Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default);
}
