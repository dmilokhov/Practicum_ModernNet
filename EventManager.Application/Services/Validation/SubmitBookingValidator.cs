using EventManager.Application.Commands;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services.Validation;
using EventManager.Domain.Constants;
using EventManager.Domain.Exceptions;

namespace EventManager.Application.Services.Validation;

public class SubmitBookingValidator(IEventRepository eventRepository, IUserRepository userRepository) : ISubmitBookingValidator
{
    public async Task ValidateAsync(SubmitBookingCommand command, CancellationToken ct = default)
    {
        var eventForBooking = await eventRepository.GetAsync(command.EventId, ct);
        var user = await userRepository.GetByIdAsync(command.UserId, ct);

        var reserved = eventForBooking.TryReserveSeats();
        if (!reserved)
        {
            throw new NoAvailableSeatsException(ExceptionMessages.NoAvailableSeatsExceptionMsg);
        }

        if(eventForBooking.StartAt >= DateTime.UtcNow)
        {
            throw new TryBookStartedEventException(ExceptionMessages.TryBookStartedEventExceptionMsg);
        }

        if(user.Bookings.Count >= Limitations.MaxUserBookingAmount)
        {
            throw new BookingLimitOverflowException(
                ExceptionMessages.BookingLimitOverflowExceptionMsg(Limitations.MaxUserBookingAmount));
        }
            
    }
}
