using System.Text;
using System.Text.Json;
using Order.Application.Interfaces;
using RabbitMQ.Client;

namespace Order.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IModel _channel;

    public RabbitMqPublisher(IModel channel) => _channel = channel;

    public Task PublishAsync<T>(string queue, T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: props, body: body);

        return Task.CompletedTask;
    }
}
