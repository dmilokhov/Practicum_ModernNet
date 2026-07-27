using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Interfaces.Factories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Model.Factories;
using UserService.Application.Services;
using UserService.Application.Services.Validation;

namespace UserService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IUserFactory, UserFactory>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddValidatorsFromAssemblyContaining<RegistrationCommandValidator>();
        return services;
    }
}
