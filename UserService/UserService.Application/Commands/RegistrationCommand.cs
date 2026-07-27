using EventManager.Common.Core.Enums;

namespace UserService.Application.Commands;

public class RegistrationCommand
{
    public required string Login { get; init; }
    public required string Password { get; init; }
    public Roles Role { get; init; } = Roles.User;
}
