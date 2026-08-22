using EventService.Application.Interfaces;
using EventService.Application.Interfaces.Cache;
using EventService.Application.Interfaces.Repositories;
using EventService.Application.Interfaces.Services;
using EventService.Application.Model.DTOs;
using EventService.Application.Model.Validators;
using EventService.Application.Services;
using EventService.Domain.Constants;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EventService.UnitTests.EventServiceTests;

public abstract class EventServiceTestsBase : IDisposable
{
    protected static readonly DateTime BaseTestStartDate = new DateTime(2026, 5, 1);
    protected static readonly DateTime BaseTestEndDate = new(2026, 6, 20);
    protected static readonly int BaseTotalSeats = 100;

    protected readonly ServiceProvider ServiceProvider;   

    public static IEnumerable<object[]> GetValidationTestData()
    {
        yield return [ new EventDto
            {
                Title = "",
                Description = "I am new",
                StartAt = BaseTestStartDate,
                EndAt = BaseTestEndDate,
                TotalSeats = BaseTotalSeats
            },
            ValidationMessages.TitleIsRequiredMsg
        ];

        yield return [ new EventDto
            {
                Title = null!,
                Description = "I am new",
                StartAt = BaseTestStartDate,
                EndAt = BaseTestEndDate,
                TotalSeats = BaseTotalSeats
            },
            ValidationMessages.TitleIsRequiredMsg
        ];

        yield return [ new EventDto
            {
                Title = "New Event",
                Description = "I am new",
                StartAt = BaseTestEndDate,
                EndAt = BaseTestStartDate,
                TotalSeats = BaseTotalSeats
            },
            ValidationMessages.EndDateLaterThanStartMsg
        ];

        yield return [ new EventDto
            {
                Title = "New Event",
                Description = "I am new",
                StartAt = BaseTestStartDate,
                EndAt = BaseTestEndDate,
                TotalSeats = 0
            },
            ValidationMessages.TotalSeatsAboveZeroMsg
        ];
    }

    protected EventServiceTestsBase()
    {
        var services = new ServiceCollection();
        var cacheService = new Mock<ICacheService>();
        var eventCacheInvalidator = new Mock<IEventCacheInvalidator>();

        eventCacheInvalidator
            .Setup(x => x.InvalidateAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        var dbName = Guid.NewGuid().ToString();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventFilterValidator, EventFilterValidator>();
        services.AddScoped<ICacheService>(_ => cacheService.Object);
        services.AddScoped<IEventCacheInvalidator>(_ => eventCacheInvalidator.Object);
        services.AddScoped<IEventCrudService, EventCrudService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    protected IServiceScope CreateScope()
    {
        return ServiceProvider.CreateScope();
    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
    }


}
