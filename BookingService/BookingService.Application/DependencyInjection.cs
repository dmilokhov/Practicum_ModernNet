using BookingService.Application.Interfaces.Factories;
using BookingService.Application.Interfaces.Services;
using BookingService.Application.Model.Factories;
using BookingService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IBookingFactory, BookingFactory>();
        services.AddScoped<IBookingOperationsService, BookingOperationsService>();

        return services;
    }
}
