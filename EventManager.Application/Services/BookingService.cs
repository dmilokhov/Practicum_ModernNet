using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Model.DTOs;
using EventManager.Application.Model.Mapping;
using EventManager.Domain.Constants;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;

namespace EventManager.Application.Services;

public class BookingService(IBookingFactory bookingFactory,
    IBookingRepository bookingRepository,
    IEventRepository eventRepository,
    IEventBookingLockProvider lockProvider,
    ITaskQueue<BookingDto> bookingQueue) : IBookingService
{
    public async Task<BookingDto> SubmitBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default)
    {
        var bookingDto = await CreateBookingAsync(eventId, userId, ct);
        await bookingQueue.EnqueueAsync(bookingDto, ct);
        return bookingDto;
    }

    public async Task<BookingDto> CreateBookingAsync(Guid eventId, Guid userId, CancellationToken ct = default)
    {
        using (await lockProvider.AcquireAsync(eventId, ct))
        {
            var eventForBooking = await eventRepository.GetAsync(eventId, ct);

            var reserved = eventForBooking.TryReserveSeats();
            if (!reserved)
            {
                throw new NoAvailableSeatsException(ExceptionMessages.NoAvailableSeatsExceptionMsg);
            }

            var booking = bookingFactory.Create(eventId, userId);
            await bookingRepository.AddAsync(booking, ct);
            await bookingRepository.SaveChangesAsync(ct);

            return booking.ToDto();
        }
    }

    public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(bookingId, ct);
        return bookingEntity.ToDto();
    }

    public async Task ProcessBookingAsync(Guid bookingId, CancellationToken ct = default)
    {
        var booking = await bookingRepository.GetAsync(bookingId, ct);

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
}
