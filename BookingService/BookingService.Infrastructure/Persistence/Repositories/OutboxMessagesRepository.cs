using BookingService.Application.Interfaces.Factories;
using BookingService.Application.Interfaces.Repositories;
using BookingService.Domain.Constants;
using EventManager.Common.Core.Constants;
using EventManager.Common.Core.Contracts;
using System.Text.Json;
using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Persistence.Repositories;

public class OutboxMessagesRepository(AppDbContext context, IOutboxMessageFactory outboxMessageFactory) 
    : IOutboxMessagesRepository
{
    private static readonly Dictionary<Type, string> Topics = new()
    {
        { typeof(BookingConfirmedMsg), TopicNames.BookingConfirmed },
        { typeof(BookingCancelledMsg), TopicNames.BookingCancelled }
    };

    public async Task AddAsync<TMessage>(string key, TMessage message, CancellationToken ct = default)
    {
        if (!Topics.TryGetValue(typeof(TMessage), out var topic))
        {
            throw new InvalidOperationException(ExceptionMessages.KafkaTopicNotFound(typeof(TMessage).Name));
        }

        var outbox = outboxMessageFactory.Create(
            topic,
            key,
            typeof(TMessage).AssemblyQualifiedName!,
            JsonSerializer.Serialize(message));

        await context.AddAsync(outbox, ct);
    }

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesBatchAsync(int batchCount, CancellationToken ct = default)
    {
        return await context.OutboxMessages
            .Where(m => !m.IsProcessed)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(batchCount)
            .ToListAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}
