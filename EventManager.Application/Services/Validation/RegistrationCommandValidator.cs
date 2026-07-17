using EventManager.Application.Commands;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Domain.Constants;
using FluentValidation;

namespace EventManager.Application.Services.Validation;

public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Login)
            .NotEmpty()
                .WithMessage(ValidationMessages.LoginIsRequiredMsg)
            .MustAsync(async (login, ct) => !await userRepository.IsUserExistAsync(login, ct))
                .WithMessage(ValidationMessages.UserAlreadyExistsMsg);

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(ValidationMessages.PasswordIsRequiredMsg)
            .Matches(RegularExpressions.PasswordPattern)
                .WithMessage(ValidationMessages.PasswordTooWeakMsg);
    }
}
