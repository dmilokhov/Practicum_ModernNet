using EventManager.Domain.Constants;
using EventManager.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Domain.Entities;

public enum BookingStatus { Pending, Confirmed, Rejected }

public class Booking
{
    public Guid Id { get; init; }
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; private set; }
    public Guid EventId { get; init; }
    public Event Event { get; init; } = null!;

    public Booking() {}

    public Booking(Guid id, Guid eventId, BookingStatus status, DateTime createdAt)
    {
        Id = id;
        EventId = eventId;
        Status = status;
        CreatedAt = createdAt;
    }

    public void Update(BookingStatus status, DateTime? processedAt)
    {
        Status = status;

        if (!processedAt.HasValue) return;

        if (processedAt.Value < CreatedAt)
        {
            throw new DomainValidationException(ValidationMessages.ProcessedDateLaterThanCreatedMsg);
        }

        ProcessedAt = processedAt;
    }
}

