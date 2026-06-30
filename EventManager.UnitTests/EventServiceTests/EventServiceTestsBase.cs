using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Model.DTOs;
using EventManager.Application.Model.Validators;
using EventManager.Application.Services;
using EventManager.Domain.Constants;
using EventManager.Infrastructure.Persistence;
using EventManager.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.UnitTests.EventServiceTests;

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

        var dbName = Guid.NewGuid().ToString();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventFilterValidator, EventFilterValidator>();
        services.AddScoped<IEventService, EventService>();

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
