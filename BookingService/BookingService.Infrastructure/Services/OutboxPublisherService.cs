using BookingService.Application.Interfaces.Messaging;
using BookingService.Application.Interfaces.Repositories;
using BookingService.Domain.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.Services;

public class OutboxPublisherService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxPublisherService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while publishing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task PublishBatchAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IOutboxMessagesRepository>();
            var publisher = scope.ServiceProvider.GetRequiredService<IBookingEventsPublisher>();

            var messages = await repository.GetUnprocessedMessagesBatchAsync(
                Limitations.OutboxMessagesBatchCount, cancellationToken);

            foreach (var message in messages)
            {
                try
                {
                    await publisher.PublishAsync(message, cancellationToken);
                    message.IsProcessed = true;
                }
                catch (Exception ex)
                {
                    logger.LogError( ex, "Failed to publish outbox message {MessageId}", message.Id);
                }
            }

            await repository.SaveChangesAsync(cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
