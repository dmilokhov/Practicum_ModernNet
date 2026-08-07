using EventManager.Common.AspNetCore.Helpers;
using System.Text.Json.Serialization;

namespace BookingService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddSwagger();
        services.AddAuthentication(configuration);

        return services;
    }
}
