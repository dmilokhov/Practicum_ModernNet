using BookingService.Application.Commands;
using BookingService.Application.Interfaces;
using BookingService.Application.Interfaces.Factories;
using BookingService.Application.Interfaces.Messaging;
using BookingService.Application.Interfaces.Repositories;
using BookingService.Application.Interfaces.Services;
using BookingService.Application.Model.Factories;
using BookingService.Application.Responses;
using BookingService.Domain.Constants;
using BookingService.Domain.Exceptions;
using EventManager.Common.Core.Contracts;
using EventManager.Common.Core.Enums;
using FluentValidation;

namespace BookingService.Application.Services;

public class BookingOperationsService(
    IValidator<SubmitBookingCommand> submitBookingValidator,
    IBookingFactory bookingFactory,
    IBookingRepository bookingRepository,
    IEventBookingLockProvider lockProvider,
    ITaskQueue<BookingResponse> bookingQueue,
    IBookingEventsPublisher publisher) : IBookingOperationsService
{
    public async Task<BookingResponse> SubmitBookingAsync(SubmitBookingCommand command, CancellationToken ct = default)
    {
        await submitBookingValidator.ValidateAndThrowAsync(command, ct);

        var bookingDto = await CreateBookingAsync(command, ct);
        await bookingQueue.EnqueueAsync(bookingDto, ct);
        return bookingDto;
    }

    public async Task CancelBookingAsync(CancelBookingCommand command, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(command.BookingId, ct);
        CheckCurrentUser(command.UserRole, command.UserId, bookingEntity.UserId);

        bookingEntity.Cancel();
        await bookingRepository.SaveChangesAsync(ct);

        await publisher.PublishAsync(
            bookingEntity.EventId.ToString(),
            new BookingCancelledMsg(
                bookingEntity.Id,
                bookingEntity.EventId,
                bookingEntity.UserId,
                bookingEntity.BookedSeatsAmount),
            ct);
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

        booking.Confirm();
        await bookingRepository.SaveChangesAsync(ct);

        await publisher.PublishAsync(
            booking.EventId.ToString(),
            new BookingConfirmedMsg(
                booking.Id,
                booking.EventId,
                booking.UserId,
                booking.CreatedAt,
                booking.BookedSeatsAmount),
            ct);
    }

    public async Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(bookingId, ct);
        bookingEntity.Reject();
        await bookingRepository.SaveChangesAsync(ct);

        await publisher.PublishAsync(
            bookingEntity.EventId.ToString(),
            new BookingCancelledMsg(
                bookingEntity.Id,
                bookingEntity.EventId,
                bookingEntity.UserId,
                bookingEntity.BookedSeatsAmount),
            ct);
    }

    private async Task<BookingResponse> CreateBookingAsync(SubmitBookingCommand command, CancellationToken ct = default)
    {
        using (await lockProvider.AcquireAsync(command.EventId, ct))
        {
            var usersActiveBookingCount = await bookingRepository.GetUserActiveBookingsCountAsync(command.UserId, ct);
            if (usersActiveBookingCount >= Limitations.MaxUserBookingAmount)
            {
                throw new BookingLimitOverflowException(
                    ExceptionMessages.BookingLimitOverflowExceptionMsg(Limitations.MaxUserBookingAmount));
            }

            var booking = bookingFactory.Create(command.EventId, command.UserId, command.SeatsAmount);
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
