using EventManager.Application.Model.DTOs;

namespace EventManager.Application.Interfaces.Services;
public interface IBookingService
    {
        Task<BookingDto> CreateBookingAsync(Guid eventId, CancellationToken ct = default);
        Task<BookingDto> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
        Task ConfirmBooking(Guid bookingId, CancellationToken ct = default);
        Task RejectBooking(Guid bookingId, CancellationToken ct = default);
        Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default);
    }
}
