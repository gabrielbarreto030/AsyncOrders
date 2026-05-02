namespace Order.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(OrderEntity order);
    Task<OrderEntity?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<OrderEntity>> GetAllAsync();
    Task UpdateAsync(OrderEntity order);
}
