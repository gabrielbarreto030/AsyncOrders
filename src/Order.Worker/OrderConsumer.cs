using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Order.Application.Messages;
using Order.Application.UseCases;
using Order.Infrastructure.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Order.Worker;

public class OrderConsumer : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<OrderConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public OrderConsumer(IServiceProvider services, IOptions<RabbitMqSettings> settings, ILogger<OrderConsumer> logger)
    {
        _services = services;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectToRabbitMqAsync(stoppingToken);

        if (_connection is null) return;

        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "order-created", durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += HandleMessageAsync;

        _channel.BasicConsume(queue: "order-created", autoAck: false, consumer: consumer);

        _logger.LogInformation("OrderConsumer started. Waiting for messages...");

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task HandleMessageAsync(object _, BasicDeliverEventArgs ea)
    {
        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
        _logger.LogInformation("Message received: {Json}", json);

        var message = JsonSerializer.Deserialize<OrderCreatedMessage>(json);

        if (message is null)
        {
            _logger.LogWarning("Failed to deserialize message. Discarding.");
            _channel!.BasicNack(ea.DeliveryTag, false, false);
            return;
        }

        _logger.LogInformation("Processing Order {OrderId} in 3 seconds...", message.OrderId);
        await Task.Delay(3000);

        using var scope = _services.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<ProcessOrderUseCase>();

        try
        {
            await useCase.ExecuteAsync(message.OrderId);
            _logger.LogInformation("Order {OrderId} marked as Processed.", message.OrderId);
            _channel!.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process Order {OrderId}.", message.OrderId);
            _channel!.BasicNack(ea.DeliveryTag, false, false);
        }
    }

    private async Task ConnectToRabbitMqAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            UserName = _settings.Username,
            Password = _settings.Password,
            DispatchConsumersAsync = true
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();
                _logger.LogInformation("Connected to RabbitMQ.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("RabbitMQ not ready: {Message}. Retrying in 5s...", ex.Message);
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
