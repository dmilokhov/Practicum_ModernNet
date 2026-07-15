using EventManager.Domain.Entities;
using EventManager.Domain.Enums;

namespace EventManager.Application.Interfaces.Factories;

public interface IUserFactory
{
    User Create(string login, string passwordHash, Roles role);
}
