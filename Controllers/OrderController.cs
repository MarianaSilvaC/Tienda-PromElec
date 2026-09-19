using Microsoft.AspNetCore.Mvc;
using TiendaPromElec.DTOs.Orders;
using TiendaPromElec.Services.Interfaces;

namespace TiendaPromElec.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrderResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetOrders(
        CancellationToken cancellationToken)
    {
        return Ok(await orderService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrder(
        long id,
        CancellationToken cancellationToken)
    {
        return Ok(await orderService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> PostOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await orderService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutOrder(
        long id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        await orderService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(long id, CancellationToken cancellationToken)
    {
        await orderService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
