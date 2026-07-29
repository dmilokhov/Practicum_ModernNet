using BookingService.Application.Interfaces.Repositories;
using BookingService.Application.Interfaces.Services;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Infrastructure.Persistence;
using EventManager.Common.Core.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.UnitTests.BookingServiceTests;

public class RejectBookingAndReleaseEventTests : BookingServiceTestsBase
{
    [Fact]
    public async Task RejectBookingTests_Positive()
    {
        // Arrange
        using var scope = CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var updatedBooking = BookingFactory.Create(Guid.NewGuid(), Guid.NewGuid(), 1);

        await dbContext.Bookings.AddAsync(updatedBooking);
        await dbContext.SaveChangesAsync();

        // Act
        await bookingService.RejectBooking(updatedBooking.Id);

        // Assert
        var rejectedBooking = await bookingRepository.GetAsync(updatedBooking.Id);

        rejectedBooking.Status.Should().Be(BookingStatuses.Rejected);
        rejectedBooking.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RejectBookingTests_Negative_NotFoundBooking()
    {
        //Arrange
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var randomGuid = Guid.NewGuid();
        var expectedExceptionMessage = $"{nameof(Booking)} {randomGuid} is not found";

        //Act
        var action = async () => await bookingService.RejectBooking(randomGuid);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    }
}
