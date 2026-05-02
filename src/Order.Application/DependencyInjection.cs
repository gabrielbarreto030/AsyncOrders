using Microsoft.Extensions.DependencyInjection;
using Order.Application.UseCases;

namespace Order.Application;

public static class DependencyInjection
{
    // Registers use cases that don't depend on IMessagePublisher.
    // CreateOrderUseCase is registered separately in Order.Api (requires IMessagePublisher).
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetOrdersUseCase>();
        services.AddScoped<GetOrderByIdUseCase>();
        services.AddScoped<ProcessOrderUseCase>();
        return services;
    }
}
