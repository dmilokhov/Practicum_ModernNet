namespace EventManager.Common.Core.Contracts;

public record BookingCancelledMsg(
    Guid Id,
    Guid BookingId, 
    Guid EventId, 
    Guid UserId,
    DateTime CancelledAt,
    int SeatsAmount);
