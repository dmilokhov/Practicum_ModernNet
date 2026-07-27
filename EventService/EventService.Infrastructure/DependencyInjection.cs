using EventService.Application.Interfaces.Repositories;
using EventService.Infrastructure.Persistence;
using EventService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventRepository, EventRepository>();

        //Db Context
        var connectionString = configuration.GetConnectionString("Default")
                               ?? throw new InvalidOperationException("Connection string 'Default' not found");
        services.AddDbContext<AppDbContext>(options => options
            .UseNpgsql(connectionString)
            .LogTo(Console.WriteLine)
            .EnableDetailedErrors());

        return services;
    }
}
