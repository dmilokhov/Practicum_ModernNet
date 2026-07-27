using UserService.Application.Commands;
using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Constants;
using FluentValidation;

namespace UserService.Application.Services.Validation;

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
