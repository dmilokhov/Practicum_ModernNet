using Confluent.Kafka;
using EventManager.Common.Core.Constants;
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
using StackExchange.Redis;

namespace EventService.Infrastructure;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructureAsync(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IInboxMessageRepository, InboxMessageRepository>();

        //Db Context
        var connectionString = configuration.GetConnectionString("Default")
                               ?? throw new InvalidOperationException("Connection string 'Default' not found");
        services.AddDbContext<AppDbContext>(options => options
            .UseNpgsql(connectionString)
            .LogTo(Console.WriteLine)
            .EnableDetailedErrors());

        //Settings
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));

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

        //Redis
        var redisSettings = configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
        if (redisSettings == null)
        {
            throw new InvalidOperationException(CommonExceptionMessages.SettingAreNotConfiguredMsg(RedisSettings.SectionName));
        }

        var redisOptions = new ConfigurationOptions
        {
            EndPoints = { redisSettings.EndPoint },
            ConnectTimeout = redisSettings.ConnectTimeout,
            SyncTimeout = redisSettings.SyncTimeout,
            AbortOnConnectFail = false
        };
        services.AddSingleton<IConnectionMultiplexer>(await ConnectionMultiplexer.ConnectAsync(redisOptions));

        return services;
    }
}
