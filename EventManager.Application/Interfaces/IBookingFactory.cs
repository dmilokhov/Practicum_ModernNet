using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces;

public interface IBookingFactory
{
    Booking Create(Guid eventId, Guid userId);
}
