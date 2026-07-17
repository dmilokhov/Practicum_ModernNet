using EventManager.Domain.Constants;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;

namespace EventManager.Domain.Entities;

public class Booking
{
    public Guid Id { get; init; }
    public BookingStatuses Status { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; private set; }
    public Guid EventId { get; init; }
    public Event Event { get; init; } = null!;
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;

    public Booking() {}

    public Booking(
        Guid id,
        Guid eventId, 
        Guid userId,
        BookingStatuses status, 
        DateTime createdAt)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        Status = status;
        CreatedAt = createdAt;
    }

    public void Confirm() => Update(BookingStatuses.Confirmed);
    public void Reject() => Update(BookingStatuses.Rejected);
    public void Cancel() => Update(BookingStatuses.Cancelled);

    private void Update(BookingStatuses status)
    {
        if(Status == BookingStatuses.Cancelled)
        {
            throw new TryChangeCancelledBookingException(ExceptionMessages.TryChangeCancelledBookingExceptionMsg);
        }

        Status = status;
        ProcessedAt = DateTime.UtcNow;
    }
}

