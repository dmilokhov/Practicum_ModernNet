namespace EventManager.Domain.Constants;

public static class ExceptionMessages
{
    public const string NoAvailableSeatsExceptionMsg = "No available seats for this event";
    public const string TryChangeCancelledBookingExceptionMsg = "It is not possible to change cancelled booking";
    public const string TryBookStartedEventExceptionMsg = "It is not possible to book for started/finished event";
    public const string UserCanCancelOnlyHisBookingsMsg = "User with role 'User' can cancel only his/her own bookings";
    public const string InvalidLoginOrPasswordMsg = "Invalid login or password";
    public const string InvalidUserIdMsg = "User ID missing or invalid in JWT token.";
    public const string InvalidUserRoleMsg = "User Role missing or invalid in JWT token.";

    public static string BookingLimitOverflowExceptionMsg(int limitValue) => 
        $"It is not allowed to have more than {limitValue} active event bookings for 1 user";
}
