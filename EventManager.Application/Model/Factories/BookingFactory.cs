using EventManager.Application.Interfaces;
using EventManager.Domain.Entities;

namespace EventManager.Application.Model.Factories;

public class BookingFactory : IBookingFactory
{
    public Booking Create(Guid eventId) =>
        new(Guid.NewGuid(), eventId, BookingStatus.Pending, DateTime.UtcNow);
}
