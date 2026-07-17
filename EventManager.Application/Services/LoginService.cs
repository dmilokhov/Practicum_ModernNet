using EventManager.Application.Commands;
using EventManager.Application.Interfaces.Factories;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Interfaces.Services.Security;
using EventManager.Domain.Constants;
using EventManager.Domain.Exceptions;
using FluentValidation;

namespace EventManager.Application.Services;

public class LoginService(
    IValidator<RegistrationCommand> registrationValidator,
    IValidator<LoginCommand> loginValidator,
    IUserRepository userRepository,
    IPasswordHasherService passwordHasherService,
    IJwtTokenService jwtTokenService,
    IUserFactory userFactory) : ILoginService
{
    public async Task RegisterUserAsync(RegistrationCommand request, CancellationToken ct = default)
    {
        await registrationValidator.ValidateAndThrowAsync(request, ct);
        var passwordHash = passwordHasherService.Hash(request.Password);
        var userEntity = userFactory.Create(request.Login, passwordHash, request.Role);
        await userRepository.AddAsync(userEntity, ct);
        await userRepository.SaveChangesAsync(ct);
    }

    public async Task<string> LoginAsync(LoginCommand request, CancellationToken ct = default)
    {
        await loginValidator.ValidateAndThrowAsync(request, ct);

        var user = await userRepository.GetByLoginAsync(request.Login, ct);
        var isPasswordCorrect = passwordHasherService.Verify(request.Password, user.PasswordHash);

        if (!isPasswordCorrect)
        {
            throw new UnauthorizedException(ExceptionMessages.InvalidLoginOrPasswordMsg);
        }

        return jwtTokenService.GenerateJwtToken(user);
    }
}
