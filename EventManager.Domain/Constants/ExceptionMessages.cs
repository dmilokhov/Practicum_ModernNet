namespace EventManager.Domain.Constants;

public static class ExceptionMessages
{
    public const string NoAvailableSeatsExceptionMsg = "No available seats for this event";
    public const string TryChangeCancelledBookingExceptionMsg = "It is not possible to change cancelled booking";
    public const string TryBookStartedEventExceptionMsg = "It is not possible to book for started event";
    public const string BookingLimitOverflowExceptionMsg = "It is not allowed to book more than 10 events for 1 user";
    public const string UserCanCancelOnlyHisBookingsMsg = "User with role \"User\" can cancel only his/her own bookings";  
}
