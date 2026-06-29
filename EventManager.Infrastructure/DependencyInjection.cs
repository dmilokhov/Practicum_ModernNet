using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Model.DTOs;
using EventManager.Infrastructure.Persistence.Repositories;
using EventManager.Infrastructure.Queue;
using EventManager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ITaskQueue<BookingDto>, InMemoryTaskQueue<BookingDto>>();
        services.AddSingleton<IEventBookingLockProvider, EventBookingLockProvider>();

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddHostedService<BookingBackgroundService>();

        return services;
    }
}
