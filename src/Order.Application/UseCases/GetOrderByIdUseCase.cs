using Order.Application.DTOs;
using Order.Domain.Interfaces;

namespace Order.Application.UseCases;

public class GetOrderByIdUseCase
{
    private readonly IOrderRepository _repository;

    public GetOrderByIdUseCase(IOrderRepository repository) => _repository = repository;

    public async Task<OrderDto?> ExecuteAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order is null ? null : OrderDto.FromEntity(order);
    }
}
