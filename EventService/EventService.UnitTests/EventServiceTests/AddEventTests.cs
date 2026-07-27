using EventService.Application.Interfaces.Repositories;
using EventService.Application.Interfaces.Services;
using EventService.Application.Model.DTOs;
using EventManager.Common.Core.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.UnitTests.EventServiceTests;

public class AddEventTests : EventServiceTestsBase
{
    [Fact]
    public async Task AddEvent_Positive()
    {
        //Arrange
        using var scope = CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventCrudService>();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

        var newEventDto = new EventDto()
        {
            Title = "Some new event",
            Description = "I am new",
            StartAt = BaseTestStartDate,
            EndAt = BaseTestEndDate,
            TotalSeats = BaseTotalSeats
        };

        //Act
        var result = await eventService.AddEventAsync(newEventDto);
        await eventRepository.SaveChangesAsync();
        var savedEvent = await eventRepository.GetAsync(result.Id);

        //Assert
        result.Should().NotBeNull();
        savedEvent.Should().NotBeNull();
        savedEvent.Id.Should().NotBe(Guid.Empty);
        savedEvent.Should().BeEquivalentTo(newEventDto);
        savedEvent.AvailableSeats.Should().Be(result.TotalSeats);
    }    

    [Theory]
    [MemberData(nameof(GetValidationTestData))]
    public async Task AddEvent_Negative(EventDto eventDto, string expectedExceptionMessage)
    {
        //Arrange
        using var scope = CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventCrudService>();

        //Act
        var action = async () => await eventService.AddEventAsync(eventDto);

        //Assert
        await action.Should().ThrowAsync<DomainValidationException>().WithMessage(expectedExceptionMessage);
    }
}
