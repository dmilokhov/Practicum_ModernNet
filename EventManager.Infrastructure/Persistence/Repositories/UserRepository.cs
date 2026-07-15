using EventManager.Application.Interfaces.Repositories;
using EventManager.Domain.Entities;
using EventManager.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await TryGetUserAsync(id, ct);
    }

    private async Task<User> TryGetUserAsync(Guid id, CancellationToken ct = default)
    {
        var foundUser = await context.Users.FirstOrDefaultAsync(e => e.Id == id, ct);

        if (foundUser is null)
        {
            throw new EntityNotFoundException(nameof(User), id);
        }

        return foundUser;
    }
}
