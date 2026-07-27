using EventManager.Application.Commands;
using EventManager.Domain.Constants;
using FluentValidation;

namespace EventManager.Application.Services.Validation;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage(ValidationMessages.LoginIsRequiredMsg);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ValidationMessages.PasswordIsRequiredMsg);
    }
}
