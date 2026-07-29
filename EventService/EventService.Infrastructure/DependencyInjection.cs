using Confluent.Kafka;
using EventManager.Common.Core.Settings;
using EventService.Application.Interfaces.Handlers;
using EventService.Application.Interfaces.Messaging;
using EventService.Application.Interfaces.Repositories;
using EventService.Infrastructure.Handlers;
using EventService.Infrastructure.Messaging;
using EventService.Infrastructure.Messaging.Consumers;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventRepository, EventRepository>();

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
        services.AddScoped<IKafkaMessageHandler, BookingConfirmedMsgHandler>();
        services.AddScoped<IKafkaMessageHandler, BookingCancelledMsgHandler>();

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;

            return new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.ConsumerGroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false
            };
        });

        services.AddSingleton<IKafkaMessageDispatcher, KafkaMessageDispatcher>();

        services.AddHostedService<KafkaTopicsInitializer>();
        services.AddHostedService<BookingConsumerService>();

        return services;
    }
}
