using EventManager.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Application.Commands;

public class LoginCommand
{
    [Required(AllowEmptyStrings = false, ErrorMessage = ValidationMessages.LoginIsRequiredMsg)]
    public required string Login { get; init; }

    [Required(AllowEmptyStrings = false, ErrorMessage = ValidationMessages.PasswordIsRequiredMsg)]
    public required string Password { get; init; }
}
