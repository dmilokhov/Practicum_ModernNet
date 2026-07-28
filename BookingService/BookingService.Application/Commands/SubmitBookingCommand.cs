namespace BookingService.Application.Commands;

public record SubmitBookingCommand(Guid EventId, Guid UserId);
