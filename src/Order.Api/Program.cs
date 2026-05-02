using Microsoft.EntityFrameworkCore;
using Order.Application;
using Order.Application.UseCases;
using Order.Infrastructure;
using Order.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddRabbitMq(builder.Configuration);

// CreateOrderUseCase needs IMessagePublisher, so it's registered after AddRabbitMq
builder.Services.AddScoped<CreateOrderUseCase>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();
