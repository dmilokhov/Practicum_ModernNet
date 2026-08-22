using EventService.Application.Interfaces;
using EventService.Application.Interfaces.Cache;
using EventService.Application.Interfaces.Repositories;
using EventService.Application.Model.DTOs;
using EventService.Application.Services;
using EventService.Domain.Constants;
using EventService.Domain.Entities;
using EventManager.Common.Core.Exceptions;
using FluentAssertions;
using Moq;

namespace EventService.UnitTests.EventServiceTests;

public class GetEventTests
{
    [Fact]
    public async Task GetEventAsync_WhenEventIsCached_DoesNotCallRepository()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var cachedEvent = CreateFullEventDto(eventId);
        var repository = new Mock<IEventRepository>(MockBehavior.Strict);
        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        var invalidator = new Mock<IEventCacheInvalidator>(MockBehavior.Strict);
        cache.Setup(x => x.GetAsync<FullEventDto>(CacheConstants.EventKey(eventId)))
            .ReturnsAsync(cachedEvent);
        var service = CreateService(repository, cache, invalidator);

        // Act
        var result = await service.GetEventAsync(eventId);

        // Assert
        result.Should().BeSameAs(cachedEvent);
        repository.Verify(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        cache.Verify(x => x.SendAsync(It.IsAny<string>(), It.IsAny<FullEventDto>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public async Task GetEventAsync_WhenEventIsNotCached_GetsItFromRepositoryAndCachesIt()
    {
        // Arrange
        var eventEntity = CreateEvent();
        var repository = new Mock<IEventRepository>(MockBehavior.Strict);
        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        var invalidator = new Mock<IEventCacheInvalidator>(MockBehavior.Strict);
        cache.Setup(x => x.GetAsync<FullEventDto>(CacheConstants.EventKey(eventEntity.Id)))
            .ReturnsAsync((FullEventDto?)null);
        repository.Setup(x => x.GetAsync(eventEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);
        cache.Setup(x => x.SendAsync(
                CacheConstants.EventKey(eventEntity.Id),
                It.IsAny<FullEventDto>(),
                TimeSpan.FromMinutes(CacheConstants.CachedEventByIdTtlMinutes)))
            .ReturnsAsync(true);
        var service = CreateService(repository, cache, invalidator);

        // Act
        var result = await service.GetEventAsync(eventEntity.Id);

        // Assert
        result.Should().BeEquivalentTo(eventEntity, options => options.ExcludingMissingMembers());
        repository.Verify(x => x.GetAsync(eventEntity.Id, It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(x => x.SendAsync(
            CacheConstants.EventKey(eventEntity.Id),
            It.Is<FullEventDto>(cached => cached.Id == eventEntity.Id),
            TimeSpan.FromMinutes(CacheConstants.CachedEventByIdTtlMinutes)), Times.Once);
    }

    [Fact]
    public async Task GetEventAsync_WhenEventDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var repository = new Mock<IEventRepository>(MockBehavior.Strict);
        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        var invalidator = new Mock<IEventCacheInvalidator>(MockBehavior.Strict);
        cache.Setup(x => x.GetAsync<FullEventDto>(CacheConstants.EventKey(eventId)))
            .ReturnsAsync((FullEventDto?)null);
        repository.Setup(x => x.GetAsync(eventId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Event), eventId));
        var service = CreateService(repository, cache, invalidator);

        // Act
        var action = () => service.GetEventAsync(eventId);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"{nameof(Event)} {eventId} is not found");
    }

    [Fact]
    public async Task UpdateEventAsync_WhenEventIsUpdated_InvalidatesItsCache()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var update = CreateEventDto();
        var repository = new Mock<IEventRepository>(MockBehavior.Strict);
        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        var invalidator = new Mock<IEventCacheInvalidator>(MockBehavior.Strict);
        repository.Setup(x => x.UpdateAsync(eventId, It.IsAny<Event>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        invalidator.Setup(x => x.InvalidateAsync(eventId)).Returns(Task.CompletedTask);
        var service = CreateService(repository, cache, invalidator);

        // Act
        await service.UpdateEventAsync(eventId, update);

        // Assert
        invalidator.Verify(x => x.InvalidateAsync(eventId), Times.Once);
    }

    [Fact]
    public async Task DeleteEventAsync_WhenEventIsDeleted_InvalidatesItsCache()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var repository = new Mock<IEventRepository>(MockBehavior.Strict);
        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        var invalidator = new Mock<IEventCacheInvalidator>(MockBehavior.Strict);
        repository.Setup(x => x.DeleteAsync(eventId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        invalidator.Setup(x => x.InvalidateAsync(eventId)).Returns(Task.CompletedTask);
        var service = CreateService(repository, cache, invalidator);

        // Act
        await service.DeleteEventAsync(eventId);

        // Assert
        invalidator.Verify(x => x.InvalidateAsync(eventId), Times.Once);
    }

    private static EventCrudService CreateService(
        Mock<IEventRepository> repository,
        Mock<ICacheService> cache,
        Mock<IEventCacheInvalidator> invalidator) =>
        new(repository.Object, Mock.Of<IEventFilterValidator>(), cache.Object, invalidator.Object);

    private static Event CreateEvent() =>
        new("Test event", "Test description", new DateTime(2026, 5, 1), new DateTime(2026, 5, 2), 100);

    private static EventDto CreateEventDto() => new()
    {
        Title = "Updated event",
        Description = "Updated description",
        StartAt = new DateTime(2026, 5, 1),
        EndAt = new DateTime(2026, 5, 2),
        TotalSeats = 100
    };

    private static FullEventDto CreateFullEventDto(Guid id) => new()
    {
        Id = id,
        Title = "Cached event",
        Description = "Cached description",
        StartAt = new DateTime(2026, 5, 1),
        EndAt = new DateTime(2026, 5, 2),
        TotalSeats = 100,
        AvailableSeats = 100
    };
}
