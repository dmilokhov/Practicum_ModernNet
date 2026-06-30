using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Domain.Entities;
using EventManager.Domain.Exceptions;
using EventManager.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.UnitTests.BookingServiceTests;

public class RejectBookingAndReleaseEventTests : BookingServiceTestsBase
{
    [Fact]
    public async Task RejectBookingAndReleaseEventTests_Positive()
    {
        // Arrange
        using var scope = CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

        var someEvent = new Event(
            "testEvent",
            "descr",
            new DateTime(2026, 05, 20),
            new DateTime(2026, 06, 20),
            100);

        typeof(Event).GetProperty(nameof(Event.AvailableSeats))!.SetValue(someEvent, 99);

        var updatedBooking = BookingFactory.Create(someEvent.Id);

        await dbContext.Events.AddAsync(someEvent);
        await dbContext.Bookings.AddAsync(updatedBooking);
        await dbContext.SaveChangesAsync();

        // Act
        await bookingService.RejectBookingAndReleaseEvent(updatedBooking.Id);

        // Assert
        var rejectedBooking = await bookingRepository.GetAsync(updatedBooking.Id);

        var updatedEvent =  await eventRepository.GetAsync(updatedBooking.EventId);

        rejectedBooking.Status.Should().Be(BookingStatus.Rejected);
        rejectedBooking.ProcessedAt.Should().NotBeNull();

        updatedEvent.AvailableSeats.Should().Be(updatedEvent.TotalSeats);
    }

    [Fact]
    public async Task RejectBookingAndReleaseEventTests_Negative_NotFoundBooking()
    {
        //Arrange
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        var randomGuid = Guid.NewGuid();
        var expectedExceptionMessage = $"{nameof(Booking)} {randomGuid} is not found";

        //Act
        var action = async () => await bookingService.RejectBookingAndReleaseEvent(randomGuid);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public async Task RejectBookingAndReleaseEventTests_Negative_NotFoundEvent()
    {
        //Arrange
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updatedBooking = BookingFactory.Create(Guid.NewGuid());
        var expectedExceptionMessage = $"{nameof(Event)} {updatedBooking.EventId} is not found";

        await dbContext.Bookings.AddAsync(updatedBooking);
        await dbContext.SaveChangesAsync();

        //Act
        var action = async () => await bookingService.RejectBookingAndReleaseEvent(updatedBooking.Id);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    }
}
