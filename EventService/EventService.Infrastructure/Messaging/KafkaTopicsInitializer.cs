using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace EventService.Infrastructure.Messaging;

public class KafkaTopicsInitializer(
    ILogger<KafkaTopicsInitializer> logger,
    IOptions<KafkaSettings> settings) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = settings.Value.BootstrapServers
        })
        .Build();

        try
        {
            await admin.CreateTopicsAsync(Topics);

            logger.LogInformation("Kafka topics created.");
        }
        catch (CreateTopicsException ex)
        {
            if (ex.Results.All(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
            {
                logger.LogInformation("Kafka topics already exist.");
                return;
            }

            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static readonly TopicSpecification[] Topics =
    [
        new()
        {
            Name = TopicNames.BookingConfirmed,
            NumPartitions = 3,
            ReplicationFactor = 1
        },
        new()
        {
            Name = TopicNames.BookingCancelled,
            NumPartitions = 3,
            ReplicationFactor = 1
        },
    ];
}
