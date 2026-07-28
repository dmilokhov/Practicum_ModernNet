using BookingService.Application.Responses;
using BookingService.Domain.Entities;

namespace BookingService.Application.Model.Factories;

public static class BookingResponseCreator
{
    public static BookingResponse Create(Booking entity)
        => new()
        {
            Id = entity.Id,
            EventId = entity.EventId,
            UserId = entity.UserId,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            ProcessedAt = entity.ProcessedAt
        };
}
