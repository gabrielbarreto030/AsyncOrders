using FluentAssertions;
using Moq;
using Order.Application.DTOs;
using Order.Application.UseCases;
using Order.Domain.Interfaces;

namespace Order.UnitTests.Application;

public class GetOrderByIdUseCaseTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly GetOrderByIdUseCase _sut;

    public GetOrderByIdUseCaseTests()
    {
        _sut = new GetOrderByIdUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderExists_ShouldReturnOrderDto()
    {
        var order = OrderEntity.Create("Alice", "Widget", 50m);
        _repositoryMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var result = await _sut.ExecuteAsync(order.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.CustomerName.Should().Be(order.CustomerName);
        result.ProductName.Should().Be(order.ProductName);
        result.Amount.Should().Be(order.Amount);
        result.Status.Should().Be(order.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((OrderEntity?)null);

        var result = await _sut.ExecuteAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldQueryRepositoryWithCorrectId()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((OrderEntity?)null);

        await _sut.ExecuteAsync(id);

        _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }
}
