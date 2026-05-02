using Order.Application.DTOs;
using Order.Domain.Interfaces;

namespace Order.Application.UseCases;

public class GetOrdersUseCase
{
    private readonly IOrderRepository _repository;

    public GetOrdersUseCase(IOrderRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<OrderDto>> ExecuteAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(OrderDto.FromEntity).ToList();
    }
}
