using FluentAssertions;
using Order.Domain.Entities;

namespace Order.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_ShouldSetAllPropertiesCorrectly()
    {
        var before = DateTime.UtcNow;

        var order = OrderEntity.Create("Alice", "Widget", 99.99m);

        order.Id.Should().NotBeEmpty();
        order.CustomerName.Should().Be("Alice");
        order.ProductName.Should().Be("Widget");
        order.Amount.Should().Be(99.99m);
        order.Status.Should().Be(OrderStatuses.Pending);
        order.CreatedAt.Should().BeOnOrAfter(before);
        order.ProcessedAt.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIdForEachOrder()
    {
        var first = OrderEntity.Create("Alice", "Widget", 1m);
        var second = OrderEntity.Create("Bob", "Gadget", 2m);

        first.Id.Should().NotBe(second.Id);
    }

    [Fact]
    public void MarkAsProcessed_ShouldChangeStatusToProcessed()
    {
        var order = OrderEntity.Create("Alice", "Widget", 99.99m);

        order.MarkAsProcessed();

        order.Status.Should().Be(OrderStatuses.Processed);
    }

    [Fact]
    public void MarkAsProcessed_ShouldSetProcessedAt()
    {
        var order = OrderEntity.Create("Alice", "Widget", 99.99m);
        var before = DateTime.UtcNow;

        order.MarkAsProcessed();

        order.ProcessedAt.Should().NotBeNull();
        order.ProcessedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void MarkAsProcessed_WhenOrderIsAlreadyProcessed_ShouldThrowInvalidOperationException()
    {
        var order = OrderEntity.Create("Alice", "Widget", 99.99m);
        order.MarkAsProcessed();

        var act = () => order.MarkAsProcessed();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*{OrderStatuses.Processed}*");
    }
}
