namespace EventManager.Common.Core.Contracts;

public record BookingConfirmedMsg(
    Guid Id,
    Guid BookingId, 
    Guid EventId, 
    Guid UserId,
    DateTime ConfirmedAt,
    int SeatsAmount = 1);
