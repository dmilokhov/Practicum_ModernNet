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

public class CreateBookingTests : BookingServiceTestsBase
{
    //[Fact]
    //public async Task CreateBooking_Positive()
    //{
    //    //Arrange
    //    using var scope = CreateScope();

    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
    //    var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
    //    var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

    //    var someEvent = new Event(
    //        "testEvent",
    //        "descr",
    //        new DateTime(2030, 05, 20),
    //        new DateTime(2030, 06, 20),
    //        100);

    //    await dbContext.Events.AddAsync(someEvent);
    //    var eventId = someEvent.Id;

    //    var someUser = new User (Guid.NewGuid(), "Test user", "hash", Roles.Admin);
    //    await dbContext.Users.AddAsync(someUser);
    //    await dbContext.SaveChangesAsync();

    //    //Act
    //    var result = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser.Id));

    //    //Assert
    //    result.Should().NotBeNull();
    //    result.Id.Should().NotBe(Guid.Empty);
    //    result.Status.Should().Be(BookingStatuses.Pending);
    //    result.EventId.Should().Be(eventId);

    //    var savedBooking = await bookingRepository.GetAsync(result.Id);

    //    savedBooking.Should().NotBeNull();
    //    savedBooking.EventId.Should().Be(eventId);
    //    savedBooking.Status.Should().Be(BookingStatuses.Pending);
        
    //    var updatedEvent = await eventRepository.GetAsync(eventId);

    //    updatedEvent.AvailableSeats.Should().Be(updatedEvent.TotalSeats - 1);
    //}

    //[Fact]
    //public async Task CreateBooking_Positive_SeveralBookingsForOneEvent()
    //{
    //    //Arrange
    //    using var scope = CreateScope();

    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
    //    var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
    //    var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

    //    var someEvent = new Event(
    //        "testEvent",
    //        "descr",
    //        new DateTime(2030, 05, 20),
    //        new DateTime(2030, 06, 20),
    //        2);

    //    await dbContext.Events.AddAsync(someEvent);
    //    var eventId = someEvent.Id;

    //    var someUser = new User(Guid.NewGuid(), "Test user", "hash", Roles.Admin);
    //    await dbContext.Users.AddAsync(someUser);
    //    await dbContext.SaveChangesAsync();

    //    //Act
    //    var firstBookingResult = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser.Id));
    //    var secondBookingResult = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser.Id));

    //    //Assert

    //    firstBookingResult.Should().NotBeNull();
    //    secondBookingResult.Should().NotBeNull();

    //    firstBookingResult.Id.Should().NotBe(secondBookingResult.Id);
    //    firstBookingResult.EventId.Should().Be(secondBookingResult.EventId);

    //    var firstSavedBooking = await bookingRepository.GetAsync(firstBookingResult.Id);
    //    var secondSavedBooking = await bookingRepository.GetAsync(secondBookingResult.Id);

    //    firstSavedBooking.Should().NotBeNull();
    //    secondSavedBooking.Should().NotBeNull();

    //    var updatedEvent = await eventRepository.GetAsync(eventId);

    //    updatedEvent.AvailableSeats.Should().Be(0);
    //}

    //[Fact]
    //public async Task CreateBooking_Negative_EventNotFound()
    //{
    //    //Arrange
    //    using var scope = CreateScope();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var eventId = Guid.NewGuid();
    //    var expectedExceptionMessage = $"Event {eventId} is not found";

    //    //Act
    //    var action = async () => await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, Guid.NewGuid()));

    //    //Assert
    //    await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    //}

    //[Fact]
    //public async Task CreateBooking_Negative_NoAvailableSeats()
    //{
    //    //Arrange
    //    using var scope = CreateScope();

    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var totalSeats = 3;
    //    var someEvent = new Event(
    //        "testEvent",
    //        "descr",
    //        new DateTime(2030, 05, 20),
    //        new DateTime(2030, 06, 20),
    //        2);

    //    await dbContext.Events.AddAsync(someEvent);

    //    var someUser = new User(Guid.NewGuid(), "Test user", "hash", Roles.Admin);
    //    await dbContext.Users.AddAsync(someUser);
    //    await dbContext.SaveChangesAsync();

    //    var eventId = someEvent.Id;

    //    //Act
    //    var action = async () => 
    //    {
    //        for (var i = 0; i < totalSeats + 1; i++)
    //        {
    //            await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser.Id));
    //        }
    //    };

    //    //Assert
    //    await action.Should().ThrowAsync<NoAvailableSeatsException>().WithMessage(ExceptionMessages.NoAvailableSeatsExceptionMsg);
    //}

    //[Fact]
    //public async Task CreateBooking_Negative_TryBookingForStartedEvent()
    //{
    //    //Arrange
    //    using var scope = CreateScope();

    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var someEvent = new Event(
    //        "testEvent",
    //        "descr",
    //        new DateTime(2024, 05, 20),
    //        new DateTime(2024, 06, 20),
    //        2);

    //    await dbContext.Events.AddAsync(someEvent);

    //    var someUser = new User(Guid.NewGuid(), "Test user", "hash", Roles.Admin);
    //    await dbContext.Users.AddAsync(someUser);
    //    await dbContext.SaveChangesAsync();

    //    var eventId = someEvent.Id;

    //    //Act
    //    var action = async () => 
    //        await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser.Id));

    //    //Assert
    //    await action.Should().ThrowAsync<TryBookStartedEventException>()
    //        .WithMessage(ExceptionMessages.TryBookStartedEventExceptionMsg);
    //}

    //[Fact]
    //public async Task CreateBooking_Negative_UserLimitOverflow()
    //{
    //    //Arrange
    //    using var scope = CreateScope();

    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var someEvent = new Event(
    //        "testEvent",
    //        "descr",
    //        new DateTime(2133, 05, 20),
    //        new DateTime(2133, 06, 20),
    //        20);

    //    await dbContext.Events.AddAsync(someEvent);

    //    var someUser = new User(Guid.NewGuid(), "Test user", "hash", Roles.Admin);
    //    await dbContext.Users.AddAsync(someUser);
    //    await dbContext.SaveChangesAsync();

    //    var eventId = someEvent.Id;

    //    //Act
    //    var action = async () =>
    //    {
    //        for (var i = 0; i < Limitations.MaxUserBookingAmount + 1; i++)
    //        {
    //            await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser.Id));
    //        }
    //    };

    //    //Assert
    //    await action.Should().ThrowAsync<BookingLimitOverflowException>()
    //        .WithMessage(ExceptionMessages.BookingLimitOverflowExceptionMsg(Limitations.MaxUserBookingAmount));
    //}

    //[Fact]
    //public async Task CreateBooking_Positive_LimitsOfDifferentUsers()
    //{
    //    //Arrange
    //    using var scope = CreateScope();

    //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

    //    var someEvent = new Event(
    //        "testEvent",
    //        "descr",
    //        new DateTime(2133, 05, 20),
    //        new DateTime(2133, 06, 20),
    //        30);

    //    await dbContext.Events.AddAsync(someEvent);

    //    var someUser1 = new User(Guid.NewGuid(), "Test user", "hash", Roles.Admin);
    //    var someUser2 = new User(Guid.NewGuid(), "Test user 2", "hash2", Roles.Admin);
    //    await dbContext.Users.AddAsync(someUser1);
    //    await dbContext.Users.AddAsync(someUser2);
    //    await dbContext.SaveChangesAsync();

    //    var eventId = someEvent.Id;

    //    //Act
    //    var action = async () =>
    //    {
    //        for (var i = 0; i < Limitations.MaxUserBookingAmount; i++)
    //        {
    //            await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser1.Id));
    //            await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUser2.Id));
    //        }
    //    };

    //    //Assert
    //    await action.Should().NotThrowAsync();
    //}
}
