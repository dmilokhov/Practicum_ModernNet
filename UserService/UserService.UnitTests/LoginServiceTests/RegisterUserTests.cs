using EventManager.Common.Core.Enums;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Commands;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Constants;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.UnitTests.LoginServiceTests;

public class RegisterUserTests : LoginServiceTestsBase
{
    [Fact]
    public async Task RegisterUser_Positive()
    {
        using var scope = CreateScope();
        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var request = new RegistrationCommand
        {
            Login = "new-user",
            Password = ValidPassword
        };

        await loginService.RegisterUserAsync(request);

        var user = await userRepository.GetByLoginAsync("new-user");
        user.Should().NotBeNull();
        user!.Login.Should().Be("new-user");
        user.Role.Should().Be(Roles.User);
    }

    [Fact]
    public async Task RegisterUser_DuplicateLogin_ThrowsValidationException()
    {
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();

        await dbContext.Users.AddAsync(new User(Guid.NewGuid(), "existing-user", "hash", Roles.User));
        await dbContext.SaveChangesAsync();

        var request = new RegistrationCommand
        {
            Login = "existing-user",
            Password = ValidPassword
        };

        var action = async () => await loginService.RegisterUserAsync(request);

        await action.Should().ThrowAsync<ValidationException>()
            .WithMessage($"*{ValidationMessages.UserAlreadyExistsMsg}*");
    }

    [Fact]
    public async Task RegisterUser_WeakPassword_ThrowsValidationException()
    {
        using var scope = CreateScope();
        var loginService = scope.ServiceProvider.GetRequiredService<ILoginService>();

        var request = new RegistrationCommand
        {
            Login = "new-user",
            Password = "weak"
        };

        var action = async () => await loginService.RegisterUserAsync(request);

        await action.Should().ThrowAsync<ValidationException>()
            .WithMessage($"*{ValidationMessages.PasswordTooWeakMsg}*");
    }
}
