namespace Order.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Status { get; private set; } = OrderStatuses.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Order() { } // Required by EF Core

    public static Order Create(string customerName, string productName, decimal amount)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName,
            ProductName = productName,
            Amount = amount,
            Status = OrderStatuses.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessed()
    {
        if (Status != OrderStatuses.Pending)
            throw new InvalidOperationException($"Cannot process an order with status '{Status}'.");

        Status = OrderStatuses.Processed;
        ProcessedAt = DateTime.UtcNow;
    }
}
