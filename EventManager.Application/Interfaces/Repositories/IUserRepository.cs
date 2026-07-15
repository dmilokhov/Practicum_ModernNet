using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id, CancellationToken ct = default);
}
