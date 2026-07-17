using EventManager.Application.Commands;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services.Validation;
using EventManager.Domain.Constants;
using EventManager.Domain.Exceptions;

namespace EventManager.Application.Services.Validation;

public class SubmitBookingValidationService(IEventRepository eventRepository, IUserRepository userRepository) : ISubmitBookingValidationService
{
    public async Task ValidateAsync(SubmitBookingCommand command, CancellationToken ct = default)
    {
        var eventForBooking = await eventRepository.GetAsync(command.EventId, ct);
        var user = await userRepository.GetByIdAsync(command.UserId, ct);

        if (DateTime.UtcNow >= eventForBooking.StartAt)
        {
            throw new TryBookStartedEventException(ExceptionMessages.TryBookStartedEventExceptionMsg);
        }

        var usersActiveBookingCount = user.Bookings.Count(b => b is { IsCancelled: false, IsRejected: false });
        if (usersActiveBookingCount >= Limitations.MaxUserBookingAmount)
        {
            throw new BookingLimitOverflowException(
                ExceptionMessages.BookingLimitOverflowExceptionMsg(Limitations.MaxUserBookingAmount));
        }

        var reserved = eventForBooking.TryReserveSeats();
        if (!reserved)
        {
            throw new NoAvailableSeatsException(ExceptionMessages.NoAvailableSeatsExceptionMsg);
        } 
    }
}
