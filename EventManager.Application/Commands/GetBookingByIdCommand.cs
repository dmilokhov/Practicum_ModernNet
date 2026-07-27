using EventManager.Domain.Enums;

namespace EventManager.Application.Commands;

public record GetBookingByIdCommand(Guid BookingId, Guid UserId, Roles UserRole);
