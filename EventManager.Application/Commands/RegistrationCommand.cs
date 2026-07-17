using EventManager.Domain.Constants;
using EventManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Commands;

public class RegistrationCommand
{
    public required string Login { get; init; }
    public required string Password { get; init; }
    public Roles Role { get; init; } = Roles.User;
}
