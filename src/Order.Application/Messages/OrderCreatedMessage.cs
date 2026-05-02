namespace Order.Application.Messages;

public class OrderCreatedMessage
{
    public Guid OrderId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
