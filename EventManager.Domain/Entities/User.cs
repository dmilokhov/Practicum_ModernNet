using EventManager.Domain.Enums;

namespace EventManager.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public string Login { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    public Roles Role { get; init; } = Roles.User;

    public List<Booking> Bookings { get; private set; } = null!;

    public User() {}
    public User(string login, string passwordHash, Roles role)
    {
        Id = Guid.NewGuid();
        Login = login;
        PasswordHash = passwordHash;
        Role = role;
    }
}
