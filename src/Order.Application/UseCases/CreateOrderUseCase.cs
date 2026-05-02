using Order.Application.DTOs;
using Order.Application.Interfaces;
using Order.Application.Messages;
using Order.Domain.Interfaces;

namespace Order.Application.UseCases;

public class CreateOrderUseCase
{
    private readonly IOrderRepository _repository;
    private readonly IMessagePublisher _publisher;

    public CreateOrderUseCase(IOrderRepository repository, IMessagePublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<OrderDto> ExecuteAsync(CreateOrderRequest request)
    {
        var order = OrderEntity.Create(request.CustomerName, request.ProductName, request.Amount);

        await _repository.AddAsync(order);

        await _publisher.PublishAsync("order-created", new OrderCreatedMessage
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            ProductName = order.ProductName,
            Amount = order.Amount
        });

        return OrderDto.FromEntity(order);
    }
}
