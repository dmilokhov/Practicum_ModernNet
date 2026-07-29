namespace EventManager.Common.Core.Contracts;

public record BookingConfirmedMsg(
    Guid BookingId, 
    Guid EventId, 
    Guid UserId,
    DateTime ConfirmedAt,
    int SeatsAmount = 1);
