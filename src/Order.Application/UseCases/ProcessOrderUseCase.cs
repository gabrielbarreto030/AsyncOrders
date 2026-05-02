using Order.Domain.Interfaces;

namespace Order.Application.UseCases;

public class ProcessOrderUseCase
{
    private readonly IOrderRepository _repository;

    public ProcessOrderUseCase(IOrderRepository repository) => _repository = repository;

    public async Task ExecuteAsync(Guid orderId)
    {
        var order = await _repository.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        order.MarkAsProcessed();

        await _repository.UpdateAsync(order);
    }
}
