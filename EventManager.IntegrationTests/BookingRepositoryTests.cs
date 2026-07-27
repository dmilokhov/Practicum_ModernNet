using EventManager.Application.Interfaces;
using EventManager.Application.Model.Factories;
using EventManager.Application.Services;
using EventManager.Domain.Entities;
using EventManager.Domain.Exceptions;
using EventManager.Infrastructure.Persistence.Repositories;
using EventManager.Infrastructure.Services;
using EventManager.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using EventManager.Domain.Enums;
using EventManager.Application.Responses;

namespace EventManager.IntegrationTests;

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

        var eventModel = new Event(
            "Test Event",
            "Test description",
            new DateTime(2025, 02, 02, 0,0,0, DateTimeKind.Utc),
            new DateTime(2025, 04, 04, 0, 0, 0, DateTimeKind.Utc),
            20);

        await context.Events.AddAsync(eventModel);
       
        var user = new User(Guid.NewGuid(),"TestUser1", "Test hash", Roles.Admin);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var bookingModel = new Booking(Guid.NewGuid(), eventModel.Id, user.Id, BookingStatuses.Pending, DateTime.UtcNow);

        await using var repositoryContext = fixture.CreateContext();
        var repository = new BookingRepository(repositoryContext);

        // Act
        await repository.AddAsync(bookingModel);
        await repository.SaveChangesAsync();

        // Assert
        await using var verifyContext = fixture.CreateContext();
        var savedBooking = await verifyContext.Bookings.FirstOrDefaultAsync(b => b.EventId == eventModel.Id);

        savedBooking.Should().NotBeNull();
        savedBooking.Status.Should().Be(BookingStatuses.Pending);
    }

    [Fact]
    public async Task AddBooking_Negative_EventNotFound()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();

        var bookingModel = new Booking(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), BookingStatuses.Pending, DateTime.UtcNow);

        await using var repositoryContext = fixture.CreateContext();
        var repository = new BookingRepository(repositoryContext);

        // Act
        var action = async () =>
        {
            await repository.AddAsync(bookingModel);
            await repository.SaveChangesAsync();
        };

        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }

    #endregion

    #region ReadSingle

    [Fact]
    public async Task GetBooking_Positive()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();
        await using var context = fixture.CreateContext();

        var eventModel = new Event(
            "Test Event",
            "Test description",
            new DateTime(2025, 02, 02, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 04, 04, 0, 0, 0, DateTimeKind.Utc),
            20);

        await context.Events.AddAsync(eventModel);
        
        var user = new User(Guid.NewGuid(), "Test User", "Test hash", Roles.Admin);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var bookingModel = new Booking(Guid.NewGuid(), eventModel.Id, user.Id, BookingStatuses.Pending, DateTime.UtcNow);
        await context.Bookings.AddAsync(bookingModel);
        await context.SaveChangesAsync();

        await using var repositoryContext = fixture.CreateContext();
        var repository = new BookingRepository(repositoryContext);

        // Act
        var result = await repository.GetAsync(bookingModel.Id);

        // Assert
        result.Should().NotBeNull();
        result.EventId.Should().Be(eventModel.Id);
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
