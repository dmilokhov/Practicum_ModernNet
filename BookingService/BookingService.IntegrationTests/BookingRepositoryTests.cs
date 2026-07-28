using BookingService.Application.Interfaces;
using BookingService.Application.Responses;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Infrastructure.Persistence.Repositories;
using BookingService.IntegrationTests.Infrastructure;
using EventManager.Common.Core.Enums;
using EventManager.Common.Core.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace BookingService.IntegrationTests;

[Collection("Postgres")]
public class BookingRepositoryTests(PostgreSqlFixture fixture) 
{
    #region Create

    [Fact]
    public async Task AddBooking_Positive()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();
        await using var context = fixture.CreateContext();

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var bookingModel = new Booking(Guid.NewGuid(), eventId, userId, BookingStatuses.Pending, DateTime.UtcNow);

        await using var repositoryContext = fixture.CreateContext();
        var repository = new BookingRepository(repositoryContext);

        // Act
        await repository.AddAsync(bookingModel);
        await repository.SaveChangesAsync();

        // Assert
        await using var verifyContext = fixture.CreateContext();
        var savedBooking = await verifyContext.Bookings.FirstOrDefaultAsync(b => b.EventId == eventId);

        savedBooking.Should().NotBeNull();
        savedBooking.Status.Should().Be(BookingStatuses.Pending);
    }

    #endregion

    #region ReadSingle

    [Fact]
    public async Task GetBooking_Positive()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();
        await using var context = fixture.CreateContext();

        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var bookingModel = new Booking(Guid.NewGuid(), eventId, userId, BookingStatuses.Pending, DateTime.UtcNow);
        await context.Bookings.AddAsync(bookingModel);
        await context.SaveChangesAsync();

        await using var repositoryContext = fixture.CreateContext();
        var repository = new BookingRepository(repositoryContext);

        // Act
        var result = await repository.GetAsync(bookingModel.Id);

        // Assert
        result.Should().NotBeNull();
        result.EventId.Should().Be(eventId);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetBooking_Negative_NotFound()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        var randomGuid = Guid.NewGuid();
        var expectedExceptionMessage = $"{nameof(Booking)} {randomGuid} is not found";

        await using var repositoryContext = fixture.CreateContext();
        var repository = new BookingRepository(repositoryContext);

        // Act
        var action = async () => await repository.GetAsync(randomGuid);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    }

    #endregion
}

file sealed class NoOpTaskQueue : ITaskQueue<BookingResponse>
{
    public ValueTask EnqueueAsync(BookingResponse bookingDto, CancellationToken ct = default) =>
        ValueTask.CompletedTask;

    public async IAsyncEnumerable<BookingResponse> ReadAllAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        await Task.CompletedTask;
        yield break;
    }
}
