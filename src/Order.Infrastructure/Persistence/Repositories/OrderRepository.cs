using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces;

namespace Order.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(OrderEntity order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<OrderEntity?> GetByIdAsync(Guid id)
        => await _context.Orders.FindAsync(id);

    public async Task<IReadOnlyList<OrderEntity>> GetAllAsync()
        => await _context.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync();

    public async Task UpdateAsync(OrderEntity order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
