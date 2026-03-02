using Bookify.Application.Behaviors;
using Bookify.Domain.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            // Register MediatR handlers from Application assembly
            configuration.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);

            // Register LoggingBehavior as a global pipeline behavior
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

            // Register ValidationBehavior as a global pipeline behavior
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Register domain services
        services.AddTransient<PricingService>();

        return services;
    }
}
