using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Order.Api.Controllers;
using Order.Application.DTOs;
using Order.Application.Interfaces;
using Order.Application.UseCases;
using Order.Domain.Interfaces;

namespace Order.UnitTests.Api;

public class OrdersControllerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly OrdersController _sut;

    public OrdersControllerTests()
    {
        var createUseCase = new CreateOrderUseCase(_repositoryMock.Object, _publisherMock.Object);
        var getOrdersUseCase = new GetOrdersUseCase(_repositoryMock.Object);
        var getByIdUseCase = new GetOrderByIdUseCase(_repositoryMock.Object);

        _sut = new OrdersController(
            createUseCase,
            getOrdersUseCase,
            getByIdUseCase,
            NullLogger<OrdersController>.Instance);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturn201CreatedAtAction()
    {
        var request = new CreateOrderRequest { CustomerName = "Alice", ProductName = "Widget", Amount = 50m };

        var result = await _sut.CreateOrder(request);

        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
        created.ActionName.Should().Be(nameof(OrdersController.GetById));
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreatedOrderDtoInBody()
    {
        var request = new CreateOrderRequest { CustomerName = "Alice", ProductName = "Widget", Amount = 50m };

        var result = await _sut.CreateOrder(request);

        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var dto = created.Value.Should().BeOfType<OrderDto>().Subject;
        dto.CustomerName.Should().Be(request.CustomerName);
        dto.ProductName.Should().Be(request.ProductName);
        dto.Amount.Should().Be(request.Amount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithOrderList()
    {
        var orders = new List<OrderEntity>
        {
            OrderEntity.Create("Alice", "Widget", 50m),
            OrderEntity.Create("Bob", "Gadget", 75m)
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders.AsReadOnly());

        var result = await _sut.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<IReadOnlyList<OrderDto>>()
            .Which.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WhenNoOrders_ShouldReturnOkWithEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<OrderEntity>().AsReadOnly());

        var result = await _sut.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<IReadOnlyList<OrderDto>>()
            .Which.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_WhenOrderExists_ShouldReturnOkWithOrderDto()
    {
        var order = OrderEntity.Create("Alice", "Widget", 50m);
        _repositoryMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var result = await _sut.GetById(order.Id);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<OrderDto>()
            .Which.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetById_WhenOrderNotFound_ShouldReturnNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((OrderEntity?)null);

        var result = await _sut.GetById(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }
}
