using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Interfaces;
using Order.Domain.Interfaces;
using Order.Infrastructure.Messaging;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Persistence.Repositories;
using Order.Infrastructure.Settings;
using RabbitMQ.Client;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(opts => configuration.GetSection("RabbitMQ").Bind(opts));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }

    // Called only by Order.Api — Worker manages its own consumer connection
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var cfg = configuration.GetSection("RabbitMQ");

        services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                HostName = cfg["Host"],
                UserName = cfg["Username"],
                Password = cfg["Password"]
            };

            for (int attempt = 1; attempt <= 5; attempt++)
            {
                try { return factory.CreateConnection(); }
                catch when (attempt < 5)
                {
                    Console.WriteLine($"[RabbitMQ] Attempt {attempt} failed. Retrying in 3s...");
                    Thread.Sleep(3000);
                }
            }

            return factory.CreateConnection();
        });

        services.AddSingleton<IModel>(sp =>
        {
            var channel = sp.GetRequiredService<IConnection>().CreateModel();
            channel.QueueDeclare(queue: "order-created", durable: true, exclusive: false, autoDelete: false);
            return channel;
        });

        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
}
