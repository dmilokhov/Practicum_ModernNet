using BookingService.Application.Interfaces;
using BookingService.Application.Interfaces.Services;
using BookingService.Application.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.Services;

public class BookingBackgroundService(ILogger<BookingBackgroundService> logger,
    ITaskQueue<BookingResponse> bookingQueue,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private const int BookingProcessingTimeoutSec = 10;
    private const int ParallelismDegree = 4;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Booking background service is launched");

        await Parallel.ForEachAsync(
            bookingQueue.ReadAllAsync(stoppingToken),
            new ParallelOptions { MaxDegreeOfParallelism = ParallelismDegree, CancellationToken = stoppingToken },
            async (booking, ct) =>
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(BookingProcessingTimeoutSec));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);
            await using var scope = scopeFactory.CreateAsyncScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingOperationsService>();

            try
            {
                logger.LogInformation("Booking {bookingId} for event {eventId} has been started",
                    booking.Id, booking.EventId);

                await bookingService.ProcessBookingAsync(booking.Id, combinedCts.Token);

                logger.LogInformation("Booking {bookingId} for event {eventId} has been successfully finished",
                    booking.Id, booking.EventId);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                await bookingService.RejectBooking(booking.Id, ct);
                logger.LogWarning("Event Booking Time-out. BookingId: {bookingId}, EventId: {eventId}",
                    booking.Id, booking.EventId);
            }
            catch (Exception ex)
            {
                await bookingService.RejectBooking(booking.Id, CancellationToken.None);
                logger.LogError(ex, "Error during event booking. BookingId: {bookingId}, EventId: {eventId}",
                    booking.Id, booking.EventId);
            }
        });

        logger.LogInformation("Booking background service is stopped");
    }
}
