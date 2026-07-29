using BookingService.Domain.Entities;

namespace BookingService.Application.Interfaces.Factories;

public interface IBookingFactory
{
    Booking Create(Guid eventId, Guid userId, int seatsAmount);
}
