using EventService.Application.Interfaces.Repositories;
using EventService.Domain.Entities;

namespace EventService.Infrastructure.Persistence.Repositories;

public class InboxMessageRepository(AppDbContext context) : IInboxMessageRepository
{
    public async Task AddAsync(InboxMessage entity, CancellationToken ct = default)
    {
        await context.AddAsync(entity, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}
