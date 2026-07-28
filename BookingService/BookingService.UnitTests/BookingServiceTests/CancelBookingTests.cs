using BookingService.Application.Commands;
using BookingService.Application.Interfaces.Repositories;
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
    //[Fact]
    //public async Task CancelBooking_ReleasesSeat()
    //{
    //    using var scope = CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
    //    var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

    //    var someEvent = CreateFutureEvent(totalSeats: 5);
    //    var user = CreateUser("booking-user");
    //    await dbContext.Events.AddAsync(someEvent);
    //    await dbContext.Users.AddAsync(user);
    //    await dbContext.SaveChangesAsync();

    //    var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(someEvent.Id, user.Id));

    //    var eventAfterBooking = await eventRepository.GetAsync(someEvent.Id);
    //    eventAfterBooking.AvailableSeats.Should().Be(4);

    //    await bookingService.CancelBookingAsync(new CancelBookingCommand(booking.Id, user.Id, Roles.User));

    //    var eventAfterCancel = await eventRepository.GetAsync(someEvent.Id);
    //    eventAfterCancel.AvailableSeats.Should().Be(5);

    //    var cancelledBooking = await dbContext.Bookings.FindAsync(booking.Id);
    //    cancelledBooking!.Status.Should().Be(BookingStatuses.Cancelled);
    //}

    //[Fact]
    //public async Task CancelBooking_UserCannotCancelOthersBooking()
    //{
    //    using var scope = CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var someEvent = CreateFutureEvent(totalSeats: 5);
    //    var owner = CreateUser("owner");
    //    var otherUser = CreateUser("other-user");
    //    await dbContext.Events.AddAsync(someEvent);
    //    await dbContext.Users.AddRangeAsync(owner, otherUser);
    //    await dbContext.SaveChangesAsync();

    //    var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(someEvent.Id, owner.Id));

    //    var action = async () => await bookingService.CancelBookingAsync(
    //        new CancelBookingCommand(booking.Id, otherUser.Id, Roles.User));

    //    await action.Should().ThrowAsync<OperationNotAllowedException>()
    //        .WithMessage(ExceptionMessages.UserCanCancelOnlyHisBookingsMsg);
    //}

    //[Fact]
    //public async Task CancelBooking_AdminCanCancelAnyBooking()
    //{
    //    using var scope = CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
    //    var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

    //    var someEvent = CreateFutureEvent(totalSeats: 3);
    //    var owner = CreateUser("owner");
    //    var admin = CreateUser("admin", Roles.Admin);
    //    await dbContext.Events.AddAsync(someEvent);
    //    await dbContext.Users.AddRangeAsync(owner, admin);
    //    await dbContext.SaveChangesAsync();

    //    var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(someEvent.Id, owner.Id));

    //    await bookingService.CancelBookingAsync(new CancelBookingCommand(booking.Id, admin.Id, Roles.Admin));

    //    var eventAfterCancel = await eventRepository.GetAsync(someEvent.Id);
    //    eventAfterCancel.AvailableSeats.Should().Be(3);
    //}

    //[Fact]
    //public async Task CancelBooking_AlreadyCancelled_Throws()
    //{
    //    using var scope = CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var someEvent = CreateFutureEvent(totalSeats: 3);
    //    var user = CreateUser("booking-user");
    //    await dbContext.Events.AddAsync(someEvent);
    //    await dbContext.Users.AddAsync(user);
    //    await dbContext.SaveChangesAsync();

    //    var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(someEvent.Id, user.Id));
    //    await bookingService.CancelBookingAsync(new CancelBookingCommand(booking.Id, user.Id, Roles.User));

    //    var action = async () => await bookingService.CancelBookingAsync(
    //        new CancelBookingCommand(booking.Id, user.Id, Roles.User));

    //    await action.Should().ThrowAsync<TryChangeWrongBookingException>()
    //        .WithMessage(ExceptionMessages.NotPossibleToChangeBookingExceptionMsg(BookingStatuses.Cancelled));
    //}

    //[Fact]
    //public async Task CancelBooking_AlreadyRejected_Throws()
    //{
    //    using var scope = CreateScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var someEvent = CreateFutureEvent(totalSeats: 3);
    //    var user = CreateUser("booking-user");
    //    await dbContext.Events.AddAsync(someEvent);
    //    await dbContext.Users.AddAsync(user);
    //    await dbContext.SaveChangesAsync();

    //    var booking = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(someEvent.Id, user.Id));
    //    await bookingService.RejectBookingAndReleaseEvent(booking.Id);

    //    var action = async () => await bookingService.CancelBookingAsync(
    //        new CancelBookingCommand(booking.Id, user.Id, Roles.User));

    //    await action.Should().ThrowAsync<TryChangeWrongBookingException>()
    //        .WithMessage(ExceptionMessages.NotPossibleToChangeBookingExceptionMsg(BookingStatuses.Cancelled));
    //}

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
