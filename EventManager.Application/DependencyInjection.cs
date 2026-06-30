using EventManager.Application.Interfaces;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Model.Factories;
using EventManager.Application.Model.Validators;
using EventManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IBookingFactory, BookingFactory>();

        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IBookingService, BookingService>();

        services.AddTransient<IEventFilterValidator, EventFilterValidator>();
        return services;
    }
}
