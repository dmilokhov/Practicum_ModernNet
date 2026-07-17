using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByLoginAsync(string login, CancellationToken ct = default);
    Task<bool> IsUserExistAsync(string login, CancellationToken ct = default);
    Task AddAsync(User userEntity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
