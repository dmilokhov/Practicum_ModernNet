using EventManager.Common.Core.Enums;

namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public string Login { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    public Roles Role { get; init; } = Roles.User;

    public User() {}
    public User(Guid id, string login, string passwordHash, Roles role)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        Role = role;
    }
}
