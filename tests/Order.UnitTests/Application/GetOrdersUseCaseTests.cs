using FluentAssertions;
using Moq;
using Order.Application.DTOs;
using Order.Application.UseCases;
using Order.Domain.Interfaces;

namespace Order.UnitTests.Application;

public class GetOrdersUseCaseTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly GetOrdersUseCase _sut;

    public GetOrdersUseCaseTests()
    {
        _sut = new GetOrdersUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllOrdersMappedToDto()
    {
        var orders = new List<OrderEntity>
        {
            OrderEntity.Create("Alice", "Widget", 50m),
            OrderEntity.Create("Bob", "Gadget", 75m)
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders.AsReadOnly());

        var result = await _sut.ExecuteAsync();

        result.Should().HaveCount(2);
        result.Should().AllBeOfType<OrderDto>();
        result.Select(o => o.CustomerName).Should().BeEquivalentTo(["Alice", "Bob"]);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoOrdersExist_ShouldReturnEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<OrderEntity>().AsReadOnly());

        var result = await _sut.ExecuteAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapOrderFieldsCorrectly()
    {
        var order = OrderEntity.Create("Alice", "Widget", 99m);
        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<OrderEntity> { order }.AsReadOnly());

        var result = await _sut.ExecuteAsync();

        var dto = result.Single();
        dto.Id.Should().Be(order.Id);
        dto.CustomerName.Should().Be(order.CustomerName);
        dto.ProductName.Should().Be(order.ProductName);
        dto.Amount.Should().Be(order.Amount);
        dto.Status.Should().Be(order.Status);
    }
}
