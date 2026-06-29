using EventManager.Application.Interfaces;
using EventManager.Application.Model.DTOs;
using EventManager.Domain.Entities;

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
