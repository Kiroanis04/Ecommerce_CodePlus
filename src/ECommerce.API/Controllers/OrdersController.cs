using ECommerce.Application.Orders.DTOs;
using ECommerce.Application.Orders.Queries.GetCustomerOrders;
using ECommerce.Application.Orders.Queries.GetOrder;
using ECommerce.Application.Services;
using ECommerce.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : BaseApiController
{

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id,CancellationToken cancellationToken)
    {
        var order = await Sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<Order>>> GetCustomerOrders(int customerId, CancellationToken cancellationToken)
    {
        var orders = await Sender.Send(new GetCustomerOrdersQuery(customerId),cancellationToken);
        return Ok(orders);
    }

    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var (success, error) = await _orderService.CancelOrderAsync(id);
        if (!success)
        {
            return error == "Order not found" ? NotFound(error) : BadRequest(error);
        }

        return Ok(new { message = "Order cancelled successfully" });
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request)
    {
        var (success, error, result) = await _orderService.CheckoutAsync(request);

        if (!success)
        {
            if (error != null && error.StartsWith("__INTERNAL_ERROR__:"))
                return StatusCode(500, error.Replace("__INTERNAL_ERROR__:", ""));

            if (error != null && error.Contains("not found"))
                return NotFound(error);

            return BadRequest(error);
        }

        return Ok(result);
    }
}