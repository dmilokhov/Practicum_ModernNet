namespace EventManager.Application.Commands;

public record SubmitBookingCommand(Guid EventId, Guid UserId);
