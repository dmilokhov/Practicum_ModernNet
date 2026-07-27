using EventManager.Application.Interfaces.Factories;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;

namespace EventManager.Application.Model.Factories;

public class BookingFactory : IBookingFactory
{
    public Booking Create(Guid eventId, Guid userId) =>
        new(Guid.NewGuid(), eventId, userId, BookingStatuses.Pending, DateTime.UtcNow);
}
