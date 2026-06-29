using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Model.DTOs;
using EventManager.Application.Model.Factories;
using EventManager.Application.Services;
using EventManager.Infrastructure.Persistence;
using EventManager.Infrastructure.Persistence.Repositories;
using EventManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.UnitTests.BookingServiceTests;

public abstract class BookingServiceTestsBase : IDisposable
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly BookingFactory BookingFactory = new();
    protected readonly IEventBookingLockProvider EventBookingLockProvider = new EventBookingLockProvider();

    protected BookingServiceTestsBase()
    {
        var services = new ServiceCollection();

        var dbName = Guid.NewGuid().ToString();

        services.AddDbContext<AppDbContext>(options =>options.UseInMemoryDatabase(dbName));

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBookingFactory, BookingFactory>();
        services.AddSingleton<ITaskQueue<BookingDto>, NoOpTaskQueue>();
        services.AddScoped<IBookingService, BookingService>();

        services.AddSingleton<IEventBookingLockProvider>(EventBookingLockProvider);

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

file sealed class NoOpTaskQueue : ITaskQueue<BookingDto>
{
    public ValueTask EnqueueAsync(BookingDto bookingDto, CancellationToken ct = default) =>
        ValueTask.CompletedTask;

    public async IAsyncEnumerable<BookingDto> ReadAllAsync(CancellationToken ct = default)
    {
        await Task.CompletedTask;
        yield break;
    }
}
