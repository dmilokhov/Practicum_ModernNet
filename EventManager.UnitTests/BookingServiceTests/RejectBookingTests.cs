using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Domain.Entities;
using EventManager.Domain.Exceptions;
using EventManager.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.UnitTests.BookingServiceTests;

public class RejectBookingTests : BookingServiceTestsBase
{
    [Fact]
    public async Task RejectBooking_Positive()
    {
        //Arrange
        var notUpdatedBooking = BookingFactory.Create(Guid.NewGuid());
        var booking = BookingFactory.Create(Guid.NewGuid());

        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        //Act
        await dbContext.Bookings.AddAsync(booking);
        await dbContext.SaveChangesAsync();
        await bookingService.RejectBooking(booking.Id);

        //Assert
        var updatedBooking = await bookingRepository.GetAsync(booking.Id);

        notUpdatedBooking.Status.Should().Be(BookingStatus.Pending);
        notUpdatedBooking.ProcessedAt.Should().BeNull();

        updatedBooking.Status.Should().Be(BookingStatus.Rejected);
        updatedBooking.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RejectBooking_Negative()
    {
        //Arrange
        var randomGuid = Guid.NewGuid();
        var expectedExceptionMessage = $"{nameof(Booking)} {randomGuid} is not found";

        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        //Act
        var action = async () => await bookingService.RejectBooking(randomGuid);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    }
}
