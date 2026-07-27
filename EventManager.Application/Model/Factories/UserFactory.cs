using EventManager.Application.Interfaces.Factories;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;

namespace EventManager.Application.Model.Factories;

public class UserFactory : IUserFactory
{
    public User Create(string login, string passwordHash, Roles role) =>
        new(Guid.NewGuid(), login, passwordHash, role);
}
