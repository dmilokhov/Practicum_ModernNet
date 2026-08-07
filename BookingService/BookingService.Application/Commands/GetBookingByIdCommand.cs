using EventManager.Common.Core.Enums;

namespace BookingService.Application.Commands;

public record GetBookingByIdCommand(Guid BookingId, Guid UserId, Roles UserRole);
