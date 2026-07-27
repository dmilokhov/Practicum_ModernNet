using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces.Factories;

public interface IBookingFactory
{
    Booking Create(Guid eventId, Guid userId);
}
