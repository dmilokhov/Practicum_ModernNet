using EventManager.Application.Interfaces.Factories;
using EventManager.Application.Interfaces.Repositories;
using EventManager.Application.Interfaces.Services;
using EventManager.Application.Interfaces.Services.Security;
using EventManager.Application.Model.Factories;
using EventManager.Application.Services;
using EventManager.Application.Services.Validation;
using EventManager.Domain.Entities;
using EventManager.Infrastructure.Persistence;
using EventManager.Infrastructure.Persistence.Repositories;
using EventManager.Infrastructure.Services.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EventManager.UnitTests.LoginServiceTests;

public abstract class LoginServiceTestsBase : IDisposable
{
    protected const string ValidPassword = "Aa1!aaa";

    protected readonly ServiceProvider ServiceProvider;
    protected readonly Mock<IJwtTokenService> JwtTokenServiceMock = new();

    protected LoginServiceTestsBase()
    {
        var services = new ServiceCollection();
        var dbName = Guid.NewGuid().ToString();

        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(dbName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddValidatorsFromAssemblyContaining<RegistrationCommandValidator>();
        services.AddSingleton(JwtTokenServiceMock.Object);

        JwtTokenServiceMock
            .Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
            .Returns("test-jwt-token");

        ServiceProvider = services.BuildServiceProvider();
    }

    protected IServiceScope CreateScope() => ServiceProvider.CreateScope();

    public void Dispose() => ServiceProvider.Dispose();
}
