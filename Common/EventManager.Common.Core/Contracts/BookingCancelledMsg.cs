namespace EventManager.Common.Core.Contracts;

public record BookingCancelledMsg(
    Guid BookingId, 
    Guid EventId, 
    Guid UserId,
    int SeatsAmount);
