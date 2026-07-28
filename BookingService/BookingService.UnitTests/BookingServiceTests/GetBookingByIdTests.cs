using BookingService.Application.Interfaces.Services;
using BookingService.Domain.Constants;
using BookingService.Domain.Entities;
using BookingService.Domain.Exceptions;
using BookingService.Infrastructure.Persistence;
using EventManager.Common.Core.Enums;
using EventManager.Common.Core.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.UnitTests.BookingServiceTests;

public class GetBookingByIdTests : BookingServiceTestsBase
{
    [Fact]
    public async Task GetBookingById_Positive()
    {
        //Arrange
        var bookingToFind = BookingFactory.Create(Guid.NewGuid(), Guid.NewGuid());
        
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
        
        await dbContext.Bookings.AddAsync(bookingToFind);
        await dbContext.SaveChangesAsync();
        
        //Act
        var result =  await bookingService.GetBookingByIdAsync(
            new(bookingToFind.Id, bookingToFind.UserId, Roles.User));

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(bookingToFind, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetBookingById_Negative()
    {
        //Arrange
        using var scope = CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var randomGuid = Guid.NewGuid();
        var expectedExceptionMessage = $"{nameof(Booking)} {randomGuid} is not found";

        //Act
        var action = async () => await bookingService.GetBookingByIdAsync(
            new(randomGuid, Guid.NewGuid(), Roles.User));

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public async Task GetBookingById_UserCannotAccessOthersBooking()
    {
        //Arrange
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var bookingToFind = BookingFactory.Create(Guid.NewGuid(), ownerId);

        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        await dbContext.Bookings.AddAsync(bookingToFind);
        await dbContext.SaveChangesAsync();

        //Act
        var action = async () => await bookingService.GetBookingByIdAsync(
            new(bookingToFind.Id, otherUserId, Roles.User));

        //Assert
        await action.Should().ThrowAsync<OperationNotAllowedException>()
            .WithMessage(ExceptionMessages.UserCanCancelOnlyHisBookingsMsg);
    }

    [Fact]
    public async Task GetBookingById_AdminCanAccessAnyBooking()
    {
        //Arrange
        var ownerId = Guid.NewGuid();
        var bookingToFind = BookingFactory.Create(Guid.NewGuid(), ownerId);

        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        await dbContext.Bookings.AddAsync(bookingToFind);
        await dbContext.SaveChangesAsync();

        //Act
        var result = await bookingService.GetBookingByIdAsync(
            new(bookingToFind.Id, Guid.NewGuid(), Roles.Admin));

        //Assert
        result.Id.Should().Be(bookingToFind.Id);
    }
}
