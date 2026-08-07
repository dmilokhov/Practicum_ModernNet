using Confluent.Kafka;
using EventManager.Common.Core.Constants;
using EventService.Application.Interfaces.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventService.Infrastructure.Messaging.Consumers;

public class BookingConsumerService(
    ILogger<BookingConsumerService> logger,
    ConsumerConfig config,
    IKafkaMessageDispatcher dispatcher)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () => await ConsumeAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(
        [
            TopicNames.BookingConfirmed,
            TopicNames.BookingCancelled
        ]);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    logger.LogInformation("Processing message {Key}", consumeResult.Message.Key);

                    await dispatcher.DispatchAsync(consumeResult.Topic, consumeResult.Message.Value, stoppingToken);

                    logger.LogInformation("Processed message from topic {Topic}", consumeResult.Topic);
                    consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex,"Kafka consume failed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while processing kafka message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Consumer has been stopped.");
        }
        finally
        {
            consumer.Close();
        }
    }
}
