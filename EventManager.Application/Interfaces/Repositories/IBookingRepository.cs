using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Booking bookingModel, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
