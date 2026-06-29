using EventManager.Features.Bookings.Interfaces;

namespace EventManager.Application.Model.Factories;

public class BookingFactory : IBookingFactory
{
    public BookingDto CreateBookingDto(Guid eventId)
    {
        return new BookingDto
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}
