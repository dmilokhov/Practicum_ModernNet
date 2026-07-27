using EventManager.Common.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
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
