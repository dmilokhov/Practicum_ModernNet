using EventManager.Domain.Enums;

namespace EventManager.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public string Login { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    public Roles Role { get; init; } = Roles.User;

    public List<Booking> Bookings { get; private set; } = [];

    public User() {}
    public User(Guid id, string login, string passwordHash, Roles role)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        Role = role;
    }
}
