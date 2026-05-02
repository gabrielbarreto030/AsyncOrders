using Microsoft.AspNetCore.Mvc;
using Order.Application.DTOs;
using Order.Application.UseCases;

namespace Order.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderUseCase _createOrder;
    private readonly GetOrdersUseCase _getOrders;
    private readonly GetOrderByIdUseCase _getOrderById;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        CreateOrderUseCase createOrder,
        GetOrdersUseCase getOrders,
        GetOrderByIdUseCase getOrderById,
        ILogger<OrdersController> logger)
    {
        _createOrder = createOrder;
        _getOrders = getOrders;
        _getOrderById = getOrderById;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = await _createOrder.ExecuteAsync(request);
        _logger.LogInformation("Order {OrderId} created for {CustomerName}.", order.Id, order.CustomerName);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _getOrders.ExecuteAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _getOrderById.ExecuteAsync(id);
        return order is null ? NotFound() : Ok(order);
    }
}
