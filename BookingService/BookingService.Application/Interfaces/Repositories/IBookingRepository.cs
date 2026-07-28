using BookingService.Domain.Entities;

namespace BookingService.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking> GetAsync(Guid id, CancellationToken ct = default);
    Task<int> GetUserActiveBookingsCountAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Booking bookingModel, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
