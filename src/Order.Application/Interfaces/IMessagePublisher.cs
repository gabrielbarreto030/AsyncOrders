namespace Order.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string queue, T message);
}
