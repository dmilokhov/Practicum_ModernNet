using UserService.Application.Commands;
using UserService.Application.Interfaces.Factories;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.Services.Security;
using UserService.Domain.Constants;
using UserService.Domain.Exceptions;
using FluentValidation;

namespace UserService.Application.Services;

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
        var passwordHash = user?.PasswordHash ?? string.Empty;
        var isPasswordCorrect = passwordHasherService.Verify(request.Password, passwordHash);

        if (user is null || !isPasswordCorrect)
        {
            throw new UnauthorizedException(ExceptionMessages.InvalidLoginOrPasswordMsg);
        }

        return jwtTokenService.GenerateJwtToken(user);
    }
}
