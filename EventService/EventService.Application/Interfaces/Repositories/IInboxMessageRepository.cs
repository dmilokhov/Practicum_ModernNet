using EventService.Domain.Entities;

namespace EventService.Application.Interfaces.Repositories;

public interface IInboxMessageRepository
{
    Task AddAsync(InboxMessage entity, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
