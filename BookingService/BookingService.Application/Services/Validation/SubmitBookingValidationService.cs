using BookingService.Application.Interfaces.Repositories;
using BookingService.Application.Interfaces.Services.Validation;
using BookingService.Domain.Constants;
using BookingService.Domain.Exceptions;

namespace BookingService.Application.Services.Validation;

public class SubmitBookingValidationService(IBookingRepository bookingRepository) : ISubmitBookingValidationService
{
    public async Task ValidateAsync(Guid userId, DateTime eventStartDate, CancellationToken ct = default)
    {
        if (DateTime.UtcNow >= eventStartDate)
        {
            throw new TryBookStartedEventException(ExceptionMessages.TryBookStartedEventExceptionMsg);
        }

        var usersActiveBookingCount = await bookingRepository.GetUserActiveBookingsCountAsync(userId, ct);
        if (usersActiveBookingCount >= Limitations.MaxUserBookingAmount)
        {
            throw new BookingLimitOverflowException(
                ExceptionMessages.BookingLimitOverflowExceptionMsg(Limitations.MaxUserBookingAmount));
        }
    }
}
