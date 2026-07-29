using Confluent.Kafka;
using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using EventManager.Common.Core.Settings;
using EventService.Application.Interfaces.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventService.Infrastructure.Messaging.Consumers;

public class BookingConsumerService(
    ILogger<BookingConsumerService> logger,
    ConsumerConfig config,
    IServiceScopeFactory scopeFactory,
    IOptions<KafkaSettings> settings)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () => await ConsumeAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(TopicNames.BookingConfirmed);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var bookingMsg = JsonSerializer.Deserialize<BookingConfirmedMsg>(consumeResult.Message.Value);

                if (bookingMsg is null)
                {
                    logger.LogWarning("Received invalid message from topic {Topic}", consumeResult.Topic);
                    consumer.Commit(consumeResult);
                    continue;
                }

                using var scope = scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IBookingConfirmedMsgHandler>();

                await handler.HandleAsync(bookingMsg, stoppingToken);

                consumer.Commit(consumeResult);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Consumer has been stopped.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing kafka message");
        }
        finally
        {
            consumer.Close();
        }
    }
}
