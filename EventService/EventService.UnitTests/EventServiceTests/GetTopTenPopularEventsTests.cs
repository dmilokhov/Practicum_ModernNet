using EventService.Application.Interfaces;
using EventService.Application.Interfaces.Cache;
using EventService.Application.Interfaces.Repositories;
using EventService.Application.Model.DTOs;
using EventService.Application.Services;
using EventService.Domain.Constants;
using EventService.Domain.Entities;
using FluentAssertions;
using Moq;

namespace EventService.UnitTests.EventServiceTests;

public class GetTopTenPopularEventsTests
{
    [Fact]
    public async Task GetTopTenPopularEventsAsync_WhenEventsAreCached_DoesNotCallRepository()
    {
        // Arrange
        IReadOnlyList<FullEventDto> cachedEvents =
        [
            CreateFullEventDto(),
            CreateFullEventDto()
        ];
        var repository = new Mock<IEventRepository>(MockBehavior.Strict);
        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        var invalidator = new Mock<IEventCacheInvalidator>(MockBehavior.Strict);
        cache.Setup(x => x.GetAsync<IReadOnlyList<FullEventDto>>(CacheConstants.EventsTop10Key))
            .ReturnsAsync(cachedEvents);
        var service = CreateService(repository, cache, invalidator);

        // Act
        var result = await service.GetTopTenPopularEventsAsync(CancellationToken.None);

        // Assert
        result.Should().BeSameAs(cachedEvents);
        repository.Verify(
            x => x.GetTopBySalesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        cache.Verify(
            x => x.SendAsync(It.IsAny<string>(), It.IsAny<List<FullEventDto>>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTopTenPopularEventsAsync_WhenEventsAreNotCached_GetsThemFromRepositoryAndCachesThem()
    {
        // Arrange
        IReadOnlyList<Event> popularEvents =
        [
            CreateEvent("First popular event"),
            CreateEvent("Second popular event")
        ];
        var repository = new Mock<IEventRepository>(MockBehavior.Strict);
        var cache = new Mock<ICacheService>(MockBehavior.Strict);
        var invalidator = new Mock<IEventCacheInvalidator>(MockBehavior.Strict);
        cache.Setup(x => x.GetAsync<IReadOnlyList<FullEventDto>>(CacheConstants.EventsTop10Key))
            .ReturnsAsync((IReadOnlyList<FullEventDto>?)null);
        repository.Setup(x => x.GetTopBySalesAsync(
                ApplicationConstants.PopularEventsCount,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(popularEvents);
        cache.Setup(x => x.SendAsync(
                CacheConstants.EventsTop10Key,
                It.IsAny<List<FullEventDto>>(),
                TimeSpan.FromMinutes(CacheConstants.CachedTopEventsTtlMinutes)))
            .ReturnsAsync(true);
        var service = CreateService(repository, cache, invalidator);

        // Act
        var result = await service.GetTopTenPopularEventsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(popularEvents, options => options.ExcludingMissingMembers());
        repository.Verify(
            x => x.GetTopBySalesAsync(ApplicationConstants.PopularEventsCount, CancellationToken.None),
            Times.Once);
        cache.Verify(x => x.SendAsync(
            CacheConstants.EventsTop10Key,
            It.Is<List<FullEventDto>>(cached => cached.Select(x => x.Id)
                .SequenceEqual(popularEvents.Select(x => x.Id))),
            TimeSpan.FromMinutes(CacheConstants.CachedTopEventsTtlMinutes)), Times.Once);
    }

    private static EventCrudService CreateService(
        Mock<IEventRepository> repository,
        Mock<ICacheService> cache,
        Mock<IEventCacheInvalidator> invalidator) =>
        new(repository.Object, Mock.Of<IEventFilterValidator>(), cache.Object, invalidator.Object);

    private static Event CreateEvent(string title) =>
        new(title, "Test description", new DateTime(2026, 5, 1), new DateTime(2026, 5, 2), 100);

    private static FullEventDto CreateFullEventDto() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Cached event",
        Description = "Cached description",
        StartAt = new DateTime(2026, 5, 1),
        EndAt = new DateTime(2026, 5, 2),
        TotalSeats = 100,
        AvailableSeats = 100
    };
}
