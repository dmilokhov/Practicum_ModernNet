using BookingService.Application.Commands;
using BookingService.Application.Interfaces.Services;
using BookingService.Domain.Constants;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Domain.Exceptions;
using BookingService.Infrastructure.Persistence;
using EventManager.Common.Core.Enums;
using EventManager.Common.Core.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.UnitTests.BookingServiceTests;

public class CancelBookingTests : BookingServiceTestsBase
{
    [Fact]
    public async Task CancelBooking_ReleasesSeat()
    {
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const int seatsAmount = 1;

        var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, userId, seatsAmount));

        await bookingService.CancelBookingAsync(new CancelBookingCommand(booking.Id, userId, Roles.User));

        var cancelledBooking = await dbContext.Bookings.FindAsync(booking.Id);
        cancelledBooking!.Status.Should().Be(BookingStatuses.Cancelled);
    }

    [Fact]
    public async Task CancelBooking_UserCannotCancelOthersBooking()
    {
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var eventId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var someUserId = Guid.NewGuid();
        const int seatsAmount = 1;

        var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, ownerId, seatsAmount));

        var action = async () => await bookingService.CancelBookingAsync(
            new CancelBookingCommand(booking.Id, someUserId, Roles.User));

        await action.Should().ThrowAsync<OperationNotAllowedException>()
            .WithMessage(ExceptionMessages.UserCanCancelOnlyHisBookingsMsg);
    }

    [Fact]
    public async Task CancelBooking_AdminCanCancelAnyBooking()
    {
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var eventId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var someUserId = Guid.NewGuid();
        const int seatsAmount = 1;

        var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, ownerId, seatsAmount));

        var action = async() => await bookingService.CancelBookingAsync(new CancelBookingCommand(booking.Id, someUserId, Roles.Admin));

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CancelBooking_AlreadyCancelled_Throws()
    {
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const int seatsAmount = 1;

        var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, userId, seatsAmount));
        await bookingService.CancelBookingAsync(new CancelBookingCommand(booking.Id, userId, Roles.User));

        var action = async () => await bookingService.CancelBookingAsync(
            new CancelBookingCommand(booking.Id, userId, Roles.User));

        await action.Should().ThrowAsync<TryChangeWrongBookingException>()
            .WithMessage(ExceptionMessages.NotPossibleToChangeBookingExceptionMsg(BookingStatuses.Cancelled));
    }

    [Fact]
    public async Task CancelBooking_AlreadyRejected_Throws()
    {
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const int seatsAmount = 1;

        var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, userId, seatsAmount));
        await bookingService.RejectBooking(booking.Id);

        var action = async () => await bookingService.CancelBookingAsync(
            new CancelBookingCommand(booking.Id, userId, Roles.User));

        await action.Should().ThrowAsync<TryChangeWrongBookingException>()
            .WithMessage(ExceptionMessages.NotPossibleToChangeBookingExceptionMsg(BookingStatuses.Cancelled));
    }

    [Fact]
    public async Task CancelBooking_NotFound_Throws()
    {
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var missingBookingId = Guid.NewGuid();
        var action = async () => await bookingService.CancelBookingAsync(
            new CancelBookingCommand(missingBookingId, Guid.NewGuid(), Roles.User));

        await action.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"{nameof(Booking)} {missingBookingId} is not found");
    }
}
