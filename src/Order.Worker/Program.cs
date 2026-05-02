using Order.Application;
using Order.Infrastructure;
using Order.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<OrderConsumer>();

var host = builder.Build();

host.Run();
