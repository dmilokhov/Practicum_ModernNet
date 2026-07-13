using EventManager.Application.Model.DTOs;
using EventManager.Domain.Entities;

namespace EventManager.Application.Model.Mapping;

public static class BookingMapper
{
    public static BookingDto ToDto(this Booking entity)
    {
        return new BookingDto
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            ProcessedAt = entity.ProcessedAt 
        };
    }
}
