using FluentAssertions;
using Moq;
using Order.Application.DTOs;
using Order.Application.Interfaces;
using Order.Application.Messages;
using Order.Application.UseCases;
using Order.Domain.Entities;
using Order.Domain.Interfaces;

namespace Order.UnitTests.Application;

public class CreateOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly CreateOrderUseCase _sut;

    public CreateOrderUseCaseTests()
    {
        _sut = new CreateOrderUseCase(_repositoryMock.Object, _publisherMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPersistOrderToRepository()
    {
        var request = new CreateOrderRequest { CustomerName = "Alice", ProductName = "Widget", Amount = 50m };

        await _sut.ExecuteAsync(request);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<OrderEntity>(o =>
            o.CustomerName == request.CustomerName &&
            o.ProductName == request.ProductName &&
            o.Amount == request.Amount &&
            o.Status == OrderStatuses.Pending)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPublishMessageToOrderCreatedQueue()
    {
        var request = new CreateOrderRequest { CustomerName = "Bob", ProductName = "Gadget", Amount = 75m };

        await _sut.ExecuteAsync(request);

        _publisherMock.Verify(p => p.PublishAsync(
            "order-created",
            It.Is<OrderCreatedMessage>(m =>
                m.CustomerName == request.CustomerName &&
                m.ProductName == request.ProductName &&
                m.Amount == request.Amount)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnOrderDtoWithMatchingData()
    {
        var request = new CreateOrderRequest { CustomerName = "Carol", ProductName = "Donut", Amount = 12.5m };

        var result = await _sut.ExecuteAsync(request);

        result.Should().NotBeNull();
        result.CustomerName.Should().Be(request.CustomerName);
        result.ProductName.Should().Be(request.ProductName);
        result.Amount.Should().Be(request.Amount);
        result.Status.Should().Be(OrderStatuses.Pending);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPublishMessageWithOrderIdMatchingCreatedOrder()
    {
        var request = new CreateOrderRequest { CustomerName = "Dan", ProductName = "Item", Amount = 20m };
        Guid capturedOrderId = Guid.Empty;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<OrderEntity>()))
            .Callback<OrderEntity>(o => capturedOrderId = o.Id);

        await _sut.ExecuteAsync(request);

        _publisherMock.Verify(p => p.PublishAsync(
            "order-created",
            It.Is<OrderCreatedMessage>(m => m.OrderId == capturedOrderId)), Times.Once);
    }
}
