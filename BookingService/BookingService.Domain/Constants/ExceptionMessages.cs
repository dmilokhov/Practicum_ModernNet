using BookingService.Domain.Enums;

namespace BookingService.Domain.Constants;

public static class ExceptionMessages
{
    public const string NoAvailableSeatsExceptionMsg = "No available seats for this event";
    public const string TryBookStartedEventExceptionMsg = "It is not possible to book for started/finished event";
    public const string UserCanCancelOnlyHisBookingsMsg = "User with role 'User' can cancel only his/her own bookings";

    public static string BookingLimitOverflowExceptionMsg(int limitValue) => 
        $"It is not allowed to have more than {limitValue} active event bookings for 1 user";

    public static string NotPossibleToChangeBookingExceptionMsg(BookingStatuses status) =>
        $"It is not possible to change {status.ToString()} booking";
}
