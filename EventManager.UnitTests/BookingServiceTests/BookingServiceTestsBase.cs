using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Factories;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Interfaces.Services.Validation;
using EventManager.Application.Model.Factories;
using EventManager.Application.Responses;
using EventManager.Application.Services;
using EventManager.Application.Services.Validation;
using EventManager.Infrastructure.Persistence;
using EventManager.Infrastructure.Persistence.Repositories;
using EventManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookingFactory, BookingFactory>();
        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<ISubmitBookingValidationService, SubmitBookingValidationService>();
        services.AddSingleton<ITaskQueue<BookingResponse>, NoOpTaskQueue>();
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
