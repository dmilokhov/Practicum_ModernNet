using EventService.Application.Interfaces.Repositories;
using EventService.Application.Interfaces.Services;
using EventService.Application.Model.DTOs;
using EventService.Domain.Entities;
using EventService.Infrastructure.Persistence;
using EventManager.Common.Core.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.UnitTests.EventServiceTests;

public class UpdateEventTests : EventServiceTestsBase
{
    private readonly EventDto _newEventData = new EventDto
    {
        Title = "Updated Event",
        Description = "Updated Description",
        StartAt = BaseTestStartDate,
        EndAt = BaseTestEndDate,
        TotalSeats = BaseTotalSeats
    };

    [Fact]
    public async Task UpdateEvent_Positive()
    {
        //Arrange
        using var scope = CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventCrudService>();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        List<Event> testEvents =
        [
            new Event("First event", "test", BaseTestStartDate, BaseTestEndDate, BaseTotalSeats),
            new Event("Holiday", "holiday", BaseTestStartDate.AddMonths(-4), BaseTestEndDate, BaseTotalSeats),
            new Event("Event 3", "default", BaseTestStartDate.AddDays(-4), BaseTestEndDate, BaseTotalSeats)
        ];

        await dbContext.Events.AddRangeAsync(testEvents);
        await dbContext.SaveChangesAsync();

        var eventToUpdate = dbContext.Events.First();

        //Act
        var action = () => eventService.UpdateEventAsync(eventToUpdate.Id, _newEventData);

        //Assert
        await action.Should().NotThrowAsync();
        await eventRepository.SaveChangesAsync();

        var firstEvent = dbContext.Events.First();
        firstEvent.Should().BeEquivalentTo(_newEventData, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task UpdateEvent_Negative_NotFound()
    {
        //Arrange
        var someId = Guid.NewGuid();
        using var scope = CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventCrudService>();
        var expectedExceptionMessage = $"{nameof(Event)} {someId} is not found";

        //Act
        var action = async () => await eventService.UpdateEventAsync(someId, _newEventData);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage(expectedExceptionMessage);
    }

    [Theory]
    [MemberData(nameof(GetValidationTestData))]
    public async Task UpdateEvent_Negative_ValidationErrors(EventDto eventDto, string expectedExceptionMessage)
    {
        //Arrange
        var someId = Guid.NewGuid();
        using var scope = CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventCrudService>();

        //Act
        var action = async () => await eventService.UpdateEventAsync(someId, eventDto);

        //Assert
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(expectedExceptionMessage);
    }
}
