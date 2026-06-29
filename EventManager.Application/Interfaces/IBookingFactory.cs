using EventManager.Application.Model.DTOs;

namespace EventManager.Application.Interfaces;

public interface IBookingFactory
{
    BookingDto CreateBookingDto(Guid eventId);
}
