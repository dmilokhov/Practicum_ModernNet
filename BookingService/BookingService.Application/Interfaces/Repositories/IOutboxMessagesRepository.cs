using BookingService.Domain.Entities;

namespace BookingService.Application.Interfaces.Repositories;

public interface IOutboxMessagesRepository
{
    Task AddAsync<TMessage>(string key, TMessage message, CancellationToken ct = default);
    Task<List<OutboxMessage>> GetUnprocessedMessagesBatchAsync(int batchCount, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
