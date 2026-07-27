using EventManager.Common.Core.Enums;
using UserService.Application.Interfaces.Factories;
using UserService.Domain.Entities;

namespace UserService.Application.Model.Factories;

public class UserFactory : IUserFactory
{
    public User Create(string login, string passwordHash, Roles role) =>
        new(Guid.NewGuid(), login, passwordHash, role);
}
