using FluentAssertions;
using Moq;
using Order.Application.UseCases;
using Order.Domain.Entities;
using Order.Domain.Interfaces;

namespace Order.UnitTests.Application;

public class ProcessOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly ProcessOrderUseCase _sut;

    public ProcessOrderUseCaseTests()
    {
        _sut = new ProcessOrderUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMarkOrderAsProcessedAndPersistUpdate()
    {
        var order = OrderEntity.Create("Alice", "Widget", 50m);
        _repositoryMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        await _sut.ExecuteAsync(order.Id);

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<OrderEntity>(o =>
            o.Id == order.Id &&
            o.Status == OrderStatuses.Processed &&
            o.ProcessedAt.HasValue)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((OrderEntity?)null);

        var act = async () => await _sut.ExecuteAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderAlreadyProcessed_ShouldThrowInvalidOperationException()
    {
        var order = OrderEntity.Create("Alice", "Widget", 50m);
        order.MarkAsProcessed();
        _repositoryMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var act = async () => await _sut.ExecuteAsync(order.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
