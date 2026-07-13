using EventManager.Application.Model.DTOs;

namespace EventManager.Application.Interfaces.Services;
public interface IBookingService
{
    Task<BookingDto> SubmitBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default);
    Task<BookingDto> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default);
    Task<BookingDto> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
    Task ProcessBookingAsync(Guid bookingId, CancellationToken ct = default);
    Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default);
}
