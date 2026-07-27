using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services.Validation;
using EventManager.Domain.Constants;
using EventManager.Domain.Exceptions;

namespace EventManager.Application.Services.Validation;

public class SubmitBookingValidationService(IUserRepository userRepository) : ISubmitBookingValidationService
{
    public async Task ValidateAsync(Guid userId, DateTime eventStartDate, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);

        if (DateTime.UtcNow >= eventStartDate)
        {
            throw new TryBookStartedEventException(ExceptionMessages.TryBookStartedEventExceptionMsg);
        }

        var usersActiveBookingCount = user.Bookings.Count(b => b is { IsCancelled: false, IsRejected: false });
        if (usersActiveBookingCount >= Limitations.MaxUserBookingAmount)
        {
            throw new BookingLimitOverflowException(
                ExceptionMessages.BookingLimitOverflowExceptionMsg(Limitations.MaxUserBookingAmount));
        }
    }
}
