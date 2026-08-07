namespace BookingService.Domain.Constants;

public static class Limitations
{
    public const int MaxUserBookingAmount = 10;
    public const int OutboxMessagesBatchCount = 20;
}
