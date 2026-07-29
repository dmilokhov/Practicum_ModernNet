using UserService.Application.Commands;
using UserService.Domain.Constants;
using FluentValidation;

namespace UserService.Application.Validation;

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
