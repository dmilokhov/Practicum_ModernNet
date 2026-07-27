using EventManager.Common.Core.Enums;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Factories;

public interface IUserFactory
{
    User Create(string login, string passwordHash, Roles role);
}
