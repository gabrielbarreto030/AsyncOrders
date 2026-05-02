namespace Order.Application.DTOs;

public class CreateOrderRequest
{
    public string CustomerName { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
