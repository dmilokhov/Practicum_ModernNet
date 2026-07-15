using EventManager.Domain.Constants;
using EventManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Commands;

public class RegistrationCommand
{
    [Required(AllowEmptyStrings = false, ErrorMessage = ValidationMessages.LoginIsRequiredMsg)]
    public required string Login { get; init; }

    [Required(AllowEmptyStrings = false, ErrorMessage = ValidationMessages.PasswordIsRequiredMsg)]
    [RegularExpression(RegularExpressions.PasswordPattern, ErrorMessage = ValidationMessages.PasswordTooWeakMsg)]
    public required string Password { get; init; }
    public Roles Role { get; init; } = Roles.User;
}
