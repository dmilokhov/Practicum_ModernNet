using EventManager.Common.Core.Enums;

namespace BookingService.Application.Commands;

public record CancelBookingCommand (Guid BookingId, Guid UserId, Roles UserRole);
