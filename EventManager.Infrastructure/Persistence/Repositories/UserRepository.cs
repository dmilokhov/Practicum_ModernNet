using EventManager.Application.Interfaces.Repositories;
using EventManager.Domain.Entities;
using EventManager.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var foundUser = await context.Users.Include(u => u.Bookings)
                                           .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (foundUser is null)
        {
            throw new EntityNotFoundException(nameof(User), id);
        }

        return foundUser;
    }

    public async Task<User?> GetByLoginAsync(string login, CancellationToken ct = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Login == login, ct);
    }

    public async Task<bool> IsUserExistAsync(string login, CancellationToken ct = default)
    {
        return await context.Users.AnyAsync(u => u.Login == login, ct);
    }

    public async Task AddAsync(User userEntity, CancellationToken ct = default)
    {
        await context.Users.AddAsync(userEntity, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}
