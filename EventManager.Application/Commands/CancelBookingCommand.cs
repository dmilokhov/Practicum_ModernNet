using EventManager.Domain.Enums;

namespace EventManager.Application.Commands;

public record CancelBookingCommand (Guid BookingId, Guid UserId, Roles UserRole);
