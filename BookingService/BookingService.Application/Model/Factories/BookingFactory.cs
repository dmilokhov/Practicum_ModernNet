using BookingService.Application.Interfaces.Factories;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;

namespace BookingService.Application.Model.Factories;

public class BookingFactory : IBookingFactory
{
    public Booking Create(Guid eventId, Guid userId, int seatsAmount) =>
        new(Guid.NewGuid(), eventId, userId, BookingStatuses.Pending, DateTime.UtcNow, seatsAmount);
}
