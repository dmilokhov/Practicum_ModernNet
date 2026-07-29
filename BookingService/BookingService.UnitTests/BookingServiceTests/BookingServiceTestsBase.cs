using BookingService.Application.Interfaces;
using BookingService.Application.Interfaces.Factories;
using BookingService.Application.Interfaces.Messaging;
using BookingService.Application.Interfaces.Repositories;
using BookingService.Application.Interfaces.Services;
using BookingService.Application.Model.Factories;
using BookingService.Application.Responses;
using BookingService.Application.Services;
using BookingService.Application.Validation;
using BookingService.Infrastructure.Messaging;
using BookingService.Infrastructure.Persistence;
using BookingService.Infrastructure.Persistence.Repositories;
using BookingService.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Runtime.CompilerServices;

namespace BookingService.UnitTests.BookingServiceTests;

public abstract class BookingServiceTestsBase : IDisposable
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly BookingFactory BookingFactory = new();
    protected readonly IEventBookingLockProvider EventBookingLockProvider = new EventBookingLockProvider();
    protected readonly Mock<IBookingEventsPublisher> BookingEventsPublisherMock = new();

    protected BookingServiceTestsBase()
    {
        var services = new ServiceCollection();

        var dbName = Guid.NewGuid().ToString();

        services.AddDbContext<AppDbContext>(options =>options.UseInMemoryDatabase(dbName));

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingFactory, BookingFactory>();
        services.AddSingleton<ITaskQueue<BookingResponse>, NoOpTaskQueue>();
        services.AddScoped<IBookingOperationsService, BookingOperationsService>();

        services.AddSingleton<IEventBookingLockProvider>(EventBookingLockProvider);
        services.AddValidatorsFromAssemblyContaining<SubmitBookingCommandValidator>();
        services.AddSingleton(BookingEventsPublisherMock.Object);

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
