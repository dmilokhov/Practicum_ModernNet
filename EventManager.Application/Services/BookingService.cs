using EventManager.Application.Commands;
using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Factories;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Interfaces.Services.Validation;
using EventManager.Application.Model.Factories;
using EventManager.Application.Responses;
using EventManager.Domain.Constants;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;

namespace EventManager.Application.Services;

public class BookingService(
    ISubmitBookingValidationService validator,
    IBookingFactory bookingFactory,
    IBookingRepository bookingRepository,
    IEventRepository eventRepository,
    IEventBookingLockProvider lockProvider,
    ITaskQueue<BookingResponse> bookingQueue) : IBookingService
{
    public async Task<BookingResponse> SubmitBookingAsync(SubmitBookingCommand command, CancellationToken ct = default)
    {
        var bookingDto = await CreateBookingAsync(command, ct);
        await bookingQueue.EnqueueAsync(bookingDto, ct);
        return bookingDto;
    }

    public async Task CancelBookingAsync(CancelBookingCommand command, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(command.BookingId, ct);
        CheckCurrentUser(command.UserRole, command.UserId, bookingEntity.UserId);

        bookingEntity.Cancel();

        using (await lockProvider.AcquireAsync(bookingEntity.EventId, ct))
        {
            var eventToUpdate = await eventRepository.GetAsync(bookingEntity.EventId, ct);
            eventToUpdate.ReleaseSeats();
            await bookingRepository.SaveChangesAsync(ct);
        }
    }

    public async Task<BookingResponse> GetBookingByIdAsync(GetBookingByIdCommand command, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(command.BookingId, ct);
        CheckCurrentUser(command.UserRole, command.UserId, bookingEntity.UserId); 

        return BookingResponseCreator.Create(bookingEntity);
    }

    public async Task ProcessBookingAsync(Guid bookingId, CancellationToken ct = default)
    {
        var booking = await bookingRepository.GetAsync(bookingId, ct);

        if(booking.IsCancelled)
        {
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(2), ct);

        try
        {
            await eventRepository.GetAsync(booking.EventId, ct);
        }
        catch (EntityNotFoundException ex) when (ex.EntityName == nameof(Event))
        {
            booking.Reject();
            await bookingRepository.SaveChangesAsync(ct);
            return;
        }

        booking.Confirm();
        await bookingRepository.SaveChangesAsync(ct);
    }

    public async Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(bookingId, ct);

        using (await lockProvider.AcquireAsync(bookingEntity.EventId, ct))
        {
            var eventToUpdate = await eventRepository.GetAsync(bookingEntity.EventId, ct);
            eventToUpdate.ReleaseSeats();
            bookingEntity.Reject();
            await bookingRepository.SaveChangesAsync(ct);
        }
    }

    private async Task<BookingResponse> CreateBookingAsync(SubmitBookingCommand command, CancellationToken ct = default)
    {
        using (await lockProvider.AcquireAsync(command.EventId, ct))
        {
            var eventForBooking = await eventRepository.GetAsync(command.EventId, ct);
            await validator.ValidateAsync(command.UserId, eventForBooking.StartAt, ct);

            var reserved = eventForBooking.TryReserveSeats();
            if (!reserved)
            {
                throw new NoAvailableSeatsException(ExceptionMessages.NoAvailableSeatsExceptionMsg);
            }

            var booking = bookingFactory.Create(command.EventId, command.UserId);
            await bookingRepository.AddAsync(booking, ct);
            await bookingRepository.SaveChangesAsync(ct);

            return BookingResponseCreator.Create(booking);
        }
    }

    private void CheckCurrentUser(Roles requestUserRole, Guid requestUserid, Guid bookingUserId)
    {
        if (requestUserRole == Roles.User && requestUserid != bookingUserId)
        {
            throw new OperationNotAllowedException(ExceptionMessages.UserCanCancelOnlyHisBookingsMsg);
        }
    }
}
