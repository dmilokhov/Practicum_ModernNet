using BookingService.Application.Interfaces;
using BookingService.Application.Interfaces.Messaging;
using BookingService.Application.Interfaces.Repositories;
using BookingService.Application.Interfaces.Services;
using BookingService.Application.Responses;
using BookingService.Infrastructure.Messaging;
using BookingService.Infrastructure.Persistence;
using BookingService.Infrastructure.Persistence.Repositories;
using BookingService.Infrastructure.Queue;
using BookingService.Infrastructure.Services;
using Confluent.Kafka;
using EventManager.Common.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        //Settings
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));

        //Kafka
        services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;

            var config = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                Acks = Acks.All
            };

            return new ProducerBuilder<string, string>(config).Build();
        });

        services.AddSingleton<IBookingEventsPublisher, BookingEventsPublisher>();

        return services;
    }
}
