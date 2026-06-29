using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Model.DTOs;
using EventManager.Application.Model.Mapping;
using EventManager.Domain.Constants;
using EventManager.Domain.Entities;
using EventManager.Domain.Exceptions;

namespace EventManager.Application.Services;

public class BookingService(IBookingFactory bookingFactory,
    IBookingRepository bookingRepository,
    IEventRepository eventRepository,
    IEventBookingLockProvider lockProvider) : IBookingService
{
    public async Task<BookingDto> CreateBookingAsync(Guid eventId, CancellationToken ct = default)
    {
        using (await lockProvider.AcquireAsync(eventId, ct))
        {
            var eventForBooking = await eventRepository.GetAsync(eventId, ct);

            var reserved = eventForBooking.TryReserveSeats();
            if (!reserved)
            {
                throw new NoAvailableSeatsException(ExceptionMessages.NoAvailableSeatsExceptionMsg);
            }

            var bookingDto = bookingFactory.CreateBookingDto(eventId);
            await bookingRepository.AddAsync(bookingDto.ToEntity(), ct);
            await bookingRepository.SaveChangesAsync(ct);

            return bookingDto;
        }
    }

    public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(bookingId, ct);
        return bookingEntity.ToDto();
    }

    public async Task ConfirmBooking(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(bookingId, ct);
        bookingEntity.Update(BookingStatus.Confirmed, DateTime.UtcNow);

        await bookingRepository.SaveChangesAsync(ct);
    }

    public async Task RejectBooking(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(bookingId, ct);
        bookingEntity.Update(BookingStatus.Rejected, DateTime.UtcNow);

        await bookingRepository.SaveChangesAsync(ct);
    }

    public async Task RejectBookingAndReleaseEvent(Guid bookingId, CancellationToken ct = default)
    {
        var bookingEntity = await bookingRepository.GetAsync(bookingId, ct);

        using (await lockProvider.AcquireAsync(bookingEntity.EventId, ct))
        {
            var eventToUpdate = await eventRepository.GetAsync(bookingEntity.EventId, ct);
            eventToUpdate.ReleaseSeats();
            bookingEntity.Update(BookingStatus.Rejected, DateTime.UtcNow);
            await bookingRepository.SaveChangesAsync(ct);
        }
    }
}
