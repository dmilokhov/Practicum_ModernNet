using BookingService.Application.Interfaces.Repositories;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using EventManager.Common.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Persistence.Repositories;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    public async Task AddAsync(Booking bookingModel, CancellationToken ct = default)
    {
        await context.Bookings.AddAsync(bookingModel, ct);
    }

    public async Task<Booking> GetAsync(Guid id, CancellationToken ct = default)
    {
        var booking = await context.Bookings.FirstOrDefaultAsync(e => e.Id == id, ct);

        if (booking is null)
        {
            throw new EntityNotFoundException(nameof(Booking), id);
        }

        return booking;
    }

    public async Task<int> GetUserActiveBookingsCountAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Bookings.CountAsync(b => b.UserId == userId && 
                                                      b.Status != BookingStatuses.Cancelled &&
                                                      b.Status != BookingStatuses.Rejected, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}
