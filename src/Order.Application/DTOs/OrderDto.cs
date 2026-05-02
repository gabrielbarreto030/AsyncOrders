namespace Order.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }

    public static OrderDto FromEntity(OrderEntity order) => new()
    {
        Id = order.Id,
        CustomerName = order.CustomerName,
        ProductName = order.ProductName,
        Amount = order.Amount,
        Status = order.Status,
        CreatedAt = order.CreatedAt,
        ProcessedAt = order.ProcessedAt
    };
}
