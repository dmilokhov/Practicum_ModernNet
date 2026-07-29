namespace EventService.Domain.Constants;

public static class ErrorMessages
{
    public const string TryBookStartedEventErrorMsg = "It is not possible to book for started/finished event";
    public const string NoAvailableSeatsErrorMsg = "No available seats for this event";
}
