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
    [Fact]
    public async Task CreateBooking_Positive()
    {
        //Arrange
        using var scope = CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = Guid.NewGuid();
        var someUserId = Guid.NewGuid(); ;

        //Act
        var result = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUserId, 1));

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Status.Should().Be(BookingStatuses.Pending);
        result.EventId.Should().Be(eventId);

        var savedBooking = await bookingRepository.GetAsync(result.Id);

        savedBooking.Should().NotBeNull();
        savedBooking.EventId.Should().Be(eventId);
        savedBooking.Status.Should().Be(BookingStatuses.Pending);
    }

    [Fact]
    public async Task CreateBooking_Positive_SeveralBookingsForOneEvent()
    {
        //Arrange
        using var scope = CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var eventId = Guid.NewGuid();
        var someUserId = Guid.NewGuid();

        //Act
        var firstBookingResult = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUserId, 1));
        var secondBookingResult = await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUserId, 1));

        //Assert

        firstBookingResult.Should().NotBeNull();
        secondBookingResult.Should().NotBeNull();

        firstBookingResult.Id.Should().NotBe(secondBookingResult.Id);
        firstBookingResult.EventId.Should().Be(secondBookingResult.EventId);

        var firstSavedBooking = await bookingRepository.GetAsync(firstBookingResult.Id);
        var secondSavedBooking = await bookingRepository.GetAsync(secondBookingResult.Id);

        firstSavedBooking.Should().NotBeNull();
        secondSavedBooking.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateBooking_Negative_UserLimitOverflow()
    {
        //Arrange
        using var scope = CreateScope();

        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var eventId = Guid.NewGuid();
        var someUserId = Guid.NewGuid();

        //Act
        var action = async () =>
        {
            for (var i = 0; i < Limitations.MaxUserBookingAmount + 1; i++)
            {
                await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUserId, 1));
            }
        };

        //Assert
        await action.Should().ThrowAsync<BookingLimitOverflowException>()
            .WithMessage(ExceptionMessages.BookingLimitOverflowExceptionMsg(Limitations.MaxUserBookingAmount));
    }

    [Fact]
    public async Task CreateBooking_Positive_LimitsOfDifferentUsers()
    {
        //Arrange
        using var scope = CreateScope();

        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

        var eventId = Guid.NewGuid();
        var someUserId1 = Guid.NewGuid();
        var someUserId2 = Guid.NewGuid();

        //Act
        var action = async () =>
        {
            for (var i = 0; i < Limitations.MaxUserBookingAmount; i++)
            {
                await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUserId1, 1));
                await bookingService.SubmitBookingAsync(new SubmitBookingCommand(eventId, someUserId2, 1));
            }
        };

        //Assert
        await action.Should().NotThrowAsync();
    }
}
