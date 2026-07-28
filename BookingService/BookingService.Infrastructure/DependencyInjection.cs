using BookingService.Application.Interfaces;
using BookingService.Application.Interfaces.Repositories;
using BookingService.Application.Interfaces.Services;
using BookingService.Application.Responses;
using BookingService.Infrastructure.Persistence;
using BookingService.Infrastructure.Persistence.Repositories;
using BookingService.Infrastructure.Queue;
using BookingService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITaskQueue<BookingResponse>, InMemoryTaskQueue<BookingResponse>>();
        services.AddSingleton<IEventBookingLockProvider, EventBookingLockProvider>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddHostedService<BookingBackgroundService>();

        //Db Context
        var connectionString = configuration.GetConnectionString("Default")
                               ?? throw new InvalidOperationException("Connection string 'Default' not found");
        services.AddDbContext<AppDbContext>(options => options
            .UseNpgsql(connectionString)
            .LogTo(Console.WriteLine)
            .EnableDetailedErrors());

        return services;
    }
}
